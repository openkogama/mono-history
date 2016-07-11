using UnityEngine;
using UnityEngine.UI;

public class CrossHairAndroid : MonoBehaviour, IGUICrossHair
{
	[SerializeField]
	private Image crossHair;

	[SerializeField]
	private Text ammoCount;

	[SerializeField]
	private Image chargeFill;

	[SerializeField]
	private GameObject ammoRoot;

	[SerializeField]
	private float toggleInterval = 0.1f;

	private Vector3 origin;

	private Vector3 direction = Vector3.right;

	private float timeSinceLastToggle;

	private bool isFillOn = true;

	public Vector3 Direction
	{
		get
		{
			return direction;
		}
		set
		{
			direction = value;
		}
	}

	public Vector3 Origin
	{
		get
		{
			return origin;
		}
		set
		{
			origin = value;
		}
	}

	public bool FiredThisFrame { get; private set; }

	public bool Visible
	{
		get
		{
			return gameObject.activeSelf;
		}
		set
		{
			gameObject.SetActive(value);
			ammoRoot.SetActive(value);
		}
	}

	public void UpdateCrossHair(int ammo, Color color, float chargeState, bool firedThisFrame)
	{
		FiredThisFrame = firedThisFrame;
		if (ammo == 0 && ammoCount.isActiveAndEnabled)
		{
			ammoCount.gameObject.SetActive(value: false);
		}
		if (ammo > 0 && !ammoCount.isActiveAndEnabled)
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
			ammoCount.text = ammo.ToString();
		}
		if (crossHair != null)
		{
			crossHair.color = color;
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
