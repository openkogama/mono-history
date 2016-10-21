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

	private float timeSinceLastToggle;

	private bool isFillOn = true;

	public bool Visible
	{
		get
		{
			return gameObject.activeSelf;
		}
		set
		{
			gameObject.SetActive(value);
		}
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
		if (ammoCount.isActiveAndEnabled)
		{
			ammoCount.text = quantity.ToString();
		}
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
}
