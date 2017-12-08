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

	[SerializeField]
	private RectTransform holsterButton;

	private void Update()
	{
		if (PickupGUI.ShowEquipableUI != showingEquipableUI)
		{
			if (holsterButton != null)
			{
				holsterButton.gameObject.SetActive((PickupGUI.ShowEquipableUI & PickupGUIFlags.CanHolster) != 0);
			}
			bool flag = (PickupGUI.ShowEquipableUI & PickupGUIFlags.CanFire) != 0;
			bool flag2 = (PickupGUI.ShowEquipableUI & PickupGUIFlags.IsHolstered) != 0;
			fire.gameObject.SetActive(flag && !flag2);
			dropWeapon.gameObject.SetActive((PickupGUI.ShowEquipableUI & PickupGUIFlags.CanUnequip) != 0);
			showingEquipableUI = PickupGUI.ShowEquipableUI;
		}
		if (MVGameControllerBase.WOCM.AvatarLocal.IsSeated != leaveVehicle.gameObject.activeInHierarchy)
		{
			leaveVehicle.gameObject.SetActive(MVGameControllerBase.WOCM.AvatarLocal.IsSeated);
		}
		if (!MVGameControllerBase.WOCM.AvatarLocal.IsInMode(AvatarModeTypes.Playing) && respawnButton.gameObject.activeSelf)
		{
			respawnButton.gameObject.SetActive(value: false);
		}
		else if (MVGameControllerBase.WOCM.AvatarLocal.IsInMode(AvatarModeTypes.Playing) && !respawnButton.gameObject.activeSelf)
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
