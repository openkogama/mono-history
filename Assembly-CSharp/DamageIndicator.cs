using MV.Common;
using UnityEngine;
using UnityEngine.UI;

public class DamageIndicator : MonoBehaviour
{
	private class IndicatorArrow
	{
		private static IndicatorArrow _nextArrow;

		private static float indicationRadius;

		private IndicatorArrow nextArrow;

		private Image arrow;

		private Transform damageOrigin;

		private float timer;

		private bool idle = true;

		public static IndicatorArrow NextArrow
		{
			get
			{
				_nextArrow = _nextArrow.nextArrow;
				return _nextArrow;
			}
		}

		public IndicatorArrow(int numberOfArrows, Image arrowBase, float indicationRadius)
			: this(numberOfArrows, arrowBase, null)
		{
			IndicatorArrow.indicationRadius = indicationRadius;
			_nextArrow = this;
		}

		private IndicatorArrow(int numberOfArrows, Image arrowBase, IndicatorArrow firstArrow)
		{
			arrow = Object.Instantiate(arrowBase);
			arrow.transform.SetParent(arrowBase.transform.parent, worldPositionStays: false);
			if (firstArrow == null)
			{
				firstArrow = this;
			}
			numberOfArrows--;
			if (numberOfArrows == 0)
			{
				nextArrow = firstArrow;
			}
			else
			{
				nextArrow = new IndicatorArrow(numberOfArrows, arrowBase, firstArrow);
			}
		}

		public void Show(Transform damageOrigin, float time, float indicationRadius)
		{
			this.damageOrigin = damageOrigin;
			timer = time;
			arrow.enabled = true;
			idle = false;
		}

		private void UpdateArrowPosition()
		{
			Vector3 vector = MVGameControllerBase.CameraController.transform.worldToLocalMatrix.MultiplyPoint(damageOrigin.position);
			Vector2 normalized = new Vector2(vector.x, vector.y).normalized;
			arrow.rectTransform.localRotation = Quaternion.LookRotation(new Vector3(0f, 0f, 1f), new Vector3(normalized.x, normalized.y, 0f));
			arrow.rectTransform.anchoredPosition = new Vector2(normalized.x, normalized.y) * indicationRadius;
		}

		private void InternalUpdate()
		{
			if (!idle)
			{
				timer -= Time.deltaTime;
				idle = timer <= 0f;
				arrow.enabled = !idle;
				UpdateArrowPosition();
			}
		}

		public void Update()
		{
			for (IndicatorArrow indicatorArrow = nextArrow; indicatorArrow != this; indicatorArrow = indicatorArrow.nextArrow)
			{
				indicatorArrow.InternalUpdate();
			}
			InternalUpdate();
		}

		private void InternalReset()
		{
			arrow.enabled = false;
			idle = true;
		}

		public void Reset()
		{
			for (IndicatorArrow indicatorArrow = nextArrow; indicatorArrow != this; indicatorArrow = indicatorArrow.nextArrow)
			{
				indicatorArrow.InternalReset();
			}
			arrow.enabled = false;
		}
	}

	[Header("Configuration")]
	[SerializeField]
	[Tooltip("Distance from center, for indicator arrow to appear.")]
	private float indicationRadius = 35f;

	[Tooltip("Transparency [0..1] by time [0..1] remaining")]
	[SerializeField]
	private AnimationCurve fade;

	[SerializeField]
	private float durationPerPointOfDamage = 0.03f;

	[SerializeField]
	[Header("Dependencies")]
	private Image damageOverlay;

	[SerializeField]
	private Image directionArrowBase;

	private IndicatorArrow directionArrow;

	private float damageOverlayTimer = float.NegativeInfinity;

	private float timeNormalizationFactor;

	private float initialAlpha;

	private void Awake()
	{
		timeNormalizationFactor = 100f * durationPerPointOfDamage;
		transform.SetParent(null, worldPositionStays: false);
		directionArrow = new IndicatorArrow(3, directionArrowBase, indicationRadius);
		initialAlpha = damageOverlay.color.a;
		ResetIndicators();
	}

	public void ResetIndicators()
	{
		damageOverlay.enabled = false;
		directionArrow.Reset();
	}

	public void ShowDamage(float damageAmount, MVPlayer damageDealer, PlayerKilledByType damageType)
	{
		if (damageDealer != null && damageDealer.ActorNr != MVGameControllerBase.Game.LocalPlayer.ActorNr)
		{
			directionArrow = IndicatorArrow.NextArrow;
			if (!damageDealer.Avatar.GameObject.activeInHierarchy)
			{
				directionArrow.Show(damageDealer.Avatar.Transform, damageAmount * durationPerPointOfDamage, indicationRadius);
			}
		}
		damageOverlay.enabled = true;
		if (damageType != PlayerKilledByType.Environmental)
		{
			damageOverlayTimer = Mathf.Max(damageOverlayTimer, damageAmount * durationPerPointOfDamage);
		}
	}

	private void Update()
	{
		directionArrow.Update();
		if (damageOverlayTimer <= 0f)
		{
			damageOverlay.enabled = false;
			return;
		}
		float num = fade.Evaluate(damageOverlayTimer / timeNormalizationFactor);
		SetTransparency(damageOverlay, initialAlpha * num);
		damageOverlayTimer -= Time.deltaTime;
	}

	private void SetTransparency(Image i, float alpha)
	{
		Color color = i.color;
		color.a = alpha;
		i.color = color;
	}
}
