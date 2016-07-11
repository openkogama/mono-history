using UnityEngine;
using UnityEngine.EventSystems;

public abstract class PlayModeControlsBase : MonoBehaviour, IEventSystemHandler, ILowerResolution
{
	private bool showingEquipableUI;

	[SerializeField]
	private RectTransform use;

	[SerializeField]
	private RectTransform fire;

	[SerializeField]
	private RectTransform dropWeapon;

	[SerializeField]
	private RectTransform leaveVehicle;

	[SerializeField]
	private CrossHairAndroid crossHair;

	[SerializeField]
	private GameObject crossHairGO;

	public void LowerResolution()
	{
		int num = Screen.width / 2;
		if (num >= 800)
		{
			Screen.SetResolution(num, Screen.height / 2, fullscreen: true);
		}
	}

	private void Update()
	{
		if (PickupGUI.ShowEquipableUI != showingEquipableUI)
		{
			fire.gameObject.SetActive(PickupGUI.ShowEquipableUI);
			dropWeapon.gameObject.SetActive(PickupGUI.ShowEquipableUI);
			crossHairGO.SetActive(PickupGUI.ShowEquipableUI);
			showingEquipableUI = PickupGUI.ShowEquipableUI;
		}
		if (MVGameControllerBase.WOCM.AvatarLocal.IsSeated != leaveVehicle.gameObject.activeInHierarchy)
		{
			leaveVehicle.gameObject.SetActive(MVGameControllerBase.WOCM.AvatarLocal.IsSeated);
		}
	}

	public void ShowEUseIcon(ShowUseOption option)
	{
		use.gameObject.SetActive(value: true);
	}

	public void HideEUseIcon()
	{
		use.gameObject.SetActive(value: false);
	}

	public IGUICrossHair GetCrossHair()
	{
		return crossHair;
	}
}
