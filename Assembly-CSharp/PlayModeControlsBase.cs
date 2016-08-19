using UnityEngine;

public abstract class PlayModeControlsBase : MonoBehaviour
{
	private PickupGUIFlags showingEquipableUI;

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

	private void Update()
	{
		if (PickupGUI.ShowEquipableUI != showingEquipableUI)
		{
			fire.gameObject.SetActive((PickupGUI.ShowEquipableUI & PickupGUIFlags.CanFire) != 0);
			dropWeapon.gameObject.SetActive((PickupGUI.ShowEquipableUI & PickupGUIFlags.CanUnequip) != 0);
			crossHairGO.SetActive((PickupGUI.ShowEquipableUI & PickupGUIFlags.ShowCrosshair) != 0);
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
