using UnityEngine;
using UnityEngine.UI;

public class CrossHair : MonoBehaviour, IGUICrossHair
{
	[SerializeField]
	private Image crossHair;

	[SerializeField]
	private Text ammoCount;

	[SerializeField]
	private Image chargeFill;

	[SerializeField]
	private float toggleInterval = 0.1f;

	[SerializeField]
	private Image crossHairHitEnemyIndicator;

	[SerializeField]
	private AnimationCurve fadeCurve;

	private HolsterTip holsterTip = new HolsterTip();

	private float timeSinceLastToggle;

	private bool isFillOn = true;

	private bool shouldShowTipOnVisibleChanged = true;

	private bool isHolstered;

	private float timer = float.PositiveInfinity;

	private bool hitEffectActive;

	public bool Visible
	{
		get
		{
			return gameObject.activeSelf;
		}
		set
		{
			if (value != gameObject.activeSelf && shouldShowTipOnVisibleChanged)
			{
				holsterTip.SetHolsterState(isHolstered);
				shouldShowTipOnVisibleChanged = false;
			}
			gameObject.SetActive(value);
		}
	}

	public void ShowHasHitEffect()
	{
		if (!(crossHairHitEnemyIndicator == null))
		{
			Color color = crossHair.color;
			color.a = 0f;
			crossHairHitEnemyIndicator.color = color;
			hitEffectActive = true;
			timer = 0f;
		}
	}

	public void HolsterStateChanged(bool isHolstered)
	{
		shouldShowTipOnVisibleChanged = true;
		this.isHolstered = isHolstered;
	}

	public void UpdateCrossHair(PickupItem pickupItem)
	{
		int quantity = pickupItem.Quantity;
		Color crossHairColor = pickupItem.CrossHairColor;
		float chargeState = pickupItem.ChargeState;
		if (quantity == 0 && ammoCount.isActiveAndEnabled)
		{
			ammoCount.gameObject.SetActive(value: false);
		}
		if (quantity > 0 && !ammoCount.isActiveAndEnabled)
		{
			ammoCount.gameObject.SetActive(value: true);
		}
		if (chargeState <= 0f)
		{
			isFillOn = true;
			if (chargeFill.isActiveAndEnabled)
			{
				chargeFill.gameObject.SetActive(value: false);
			}
		}
		if (chargeState > 0f && !chargeFill.isActiveAndEnabled)
		{
			chargeFill.gameObject.SetActive(value: true);
		}
		ammoCount.text = quantity.ToString();
		if (crossHair != null)
		{
			crossHair.color = crossHairColor;
		}
		if (!(chargeState > 0f))
		{
			return;
		}
		chargeFill.fillAmount = chargeState;
		chargeFill.enabled = isFillOn;
		if (chargeState >= 0.99f)
		{
			timeSinceLastToggle += Time.deltaTime;
			if (timeSinceLastToggle > toggleInterval)
			{
				isFillOn = !isFillOn;
				timeSinceLastToggle = 0f;
			}
		}
	}

	private void Update()
	{
		if (hitEffectActive)
		{
			timer += Time.deltaTime;
			Color color = crossHair.color;
			color.a = fadeCurve.Evaluate(timer);
			crossHairHitEnemyIndicator.color = color;
			if (timer >= fadeCurve.keys[fadeCurve.length - 1].time)
			{
				hitEffectActive = false;
			}
		}
	}
}
