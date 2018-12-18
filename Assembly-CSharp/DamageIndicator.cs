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

		private DamageArrow arrow;

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

		public IndicatorArrow(int numberOfArrows, DamageArrow arrowBase, float indicationRadius)
			: this(numberOfArrows, arrowBase, null)
		{
			IndicatorArrow.indicationRadius = indicationRadius;
			_nextArrow = this;
		}

		private IndicatorArrow(int numberOfArrows, DamageArrow arrowBase, IndicatorArrow firstArrow)
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

		public void Update()
		{
			for (IndicatorArrow indicatorArrow = nextArrow; indicatorArrow != this; indicatorArrow = indicatorArrow.nextArrow)
			{
				indicatorArrow.InternalUpdate();
			}
			InternalUpdate();
		}

		public void Reset()
		{
			for (IndicatorArrow indicatorArrow = nextArrow; indicatorArrow != this; indicatorArrow = indicatorArrow.nextArrow)
			{
				indicatorArrow.InternalReset();
			}
			arrow.enabled = false;
		}

		public void SetSprite(Sprite sprite)
		{
			arrow.Sprite = sprite;
		}

		private void UpdateArrowPosition()
		{
			Vector3 vector = MVGameControllerBase.CameraController.transform.worldToLocalMatrix.MultiplyPoint(damageOrigin.position);
			Vector2 normalized = new Vector2(vector.x, vector.y).normalized;
			arrow.RectTransform.localRotation = Quaternion.LookRotation(new Vector3(0f, 0f, 1f), new Vector3(normalized.x, normalized.y, 0f));
			arrow.RectTransform.anchoredPosition = new Vector2(normalized.x, normalized.y) * indicationRadius;
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

		private void InternalReset()
		{
			arrow.enabled = false;
			idle = true;
		}
	}

	[Header("Configuration")]
	[SerializeField]
	[Tooltip("Distance from center, for indicator arrow to appear.")]
	private float indicationRadius = 35f;

	[SerializeField]
	[Tooltip("Transparency [0..1] by time [0..1] remaining")]
	private AnimationCurve fade;

	[SerializeField]
	private float durationPerPointOfDamage = 0.03f;

	[SerializeField]
	private int maxNumberOfArrows = 3;

	[Header("Dependencies")]
	[SerializeField]
	private Image damageOverlay;

	[SerializeField]
	private StreamedSpriteToCallback arrowSpriteStream;

	[SerializeField]
	private DamageArrow directionArrowBase;

	private IndicatorArrow directionArrow;

	private float damageOverlayTimer = float.NegativeInfinity;

	private float timeNormalizationFactor;

	private float initialAlpha;

	private void Awake()
	{
		timeNormalizationFactor = 100f * durationPerPointOfDamage;
		transform.SetParent(null, worldPositionStays: false);
		directionArrow = new IndicatorArrow(maxNumberOfArrows, directionArrowBase, indicationRadius);
		arrowSpriteStream.onAssetSet = SetArrowSprites;
		initialAlpha = damageOverlay.color.a;
		ResetIndicators();
	}

	private void SetArrowSprites(Sprite sprite)
	{
		for (int i = 0; i < maxNumberOfArrows; i++)
		{
			directionArrow.SetSprite(sprite);
			directionArrow = IndicatorArrow.NextArrow;
		}
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
		if (damageType != PlayerKilledByType.Environmental)
		{
			damageOverlayTimer = Mathf.Max(damageOverlayTimer, damageAmount * durationPerPointOfDamage);
			Debug.Log(damageOverlayTimer);
			damageOverlay.enabled = true;
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
