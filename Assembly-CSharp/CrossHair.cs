using UnityEngine;
using UnityEngine.UI;

public class CrossHair : MonoBehaviour, IGUICrossHair
{
	[SerializeField]
	private Image crossHair;

	[SerializeField]
	private Text ammoCount;

	[SerializeField]
	private Text chargeCount;

	private Vector3 origin;

	private Vector3 direction = Vector3.right;

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
			return crossHair.isActiveAndEnabled;
		}
		set
		{
			crossHair.enabled = value;
			crossHair.gameObject.SetActive(value);
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
		if (ammoCount.isActiveAndEnabled)
		{
			ammoCount.text = ammo.ToString();
		}
		crossHair.color = color;
		chargeCount.gameObject.SetActive(chargeState > 0f);
		if (chargeCount.isActiveAndEnabled)
		{
			chargeCount.text = Mathf.Round(chargeState * 100f).ToString();
		}
	}
}
