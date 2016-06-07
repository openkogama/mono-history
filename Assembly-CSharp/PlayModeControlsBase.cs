using UnityEngine;

public abstract class PlayModeControlsBase : MonoBehaviour
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
	private CrossHair crossHair;

	[SerializeField]
	private GameObject crossHairGO;

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
