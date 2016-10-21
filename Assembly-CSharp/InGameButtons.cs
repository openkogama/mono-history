using MV.Common;
using UnityEngine;

public class InGameButtons : MonoBehaviour
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
	private GameObject respawnButton;

	private void Update()
	{
		if (PickupGUI.ShowEquipableUI != showingEquipableUI)
		{
			fire.gameObject.SetActive((PickupGUI.ShowEquipableUI & PickupGUIFlags.CanFire) != 0);
			dropWeapon.gameObject.SetActive((PickupGUI.ShowEquipableUI & PickupGUIFlags.CanUnequip) != 0);
			showingEquipableUI = PickupGUI.ShowEquipableUI;
		}
		if (MVGameControllerBase.WOCM.AvatarLocal.IsSeated != leaveVehicle.gameObject.activeInHierarchy)
		{
			leaveVehicle.gameObject.SetActive(MVGameControllerBase.WOCM.AvatarLocal.IsSeated);
		}
		if (MVGameControllerBase.WOCM.AvatarLocal.AvatarRuntimeState != AvatarRuntimeState.Playing && respawnButton.gameObject.activeSelf)
		{
			respawnButton.gameObject.SetActive(value: false);
		}
		else if (MVGameControllerBase.WOCM.AvatarLocal.AvatarRuntimeState == AvatarRuntimeState.Playing && !respawnButton.gameObject.activeSelf)
		{
			respawnButton.gameObject.SetActive(value: true);
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
}
