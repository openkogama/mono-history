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
	private RectTransform point;

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
		HandleFireVisibility();
		if (PickupGUI.ShowEquipableUI != showingEquipableUI)
		{
			if (holsterButton != null)
			{
				holsterButton.gameObject.SetActive((PickupGUI.ShowEquipableUI & PickupGUIFlags.CanHolster) != 0);
			}
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

	private void HandleFireVisibility()
	{
		bool flag = (PickupGUI.ShowEquipableUI & PickupGUIFlags.CanFire) != 0;
		bool flag2 = (PickupGUI.ShowEquipableUI & PickupGUIFlags.IsHolstered) != 0;
		if (flag && !flag2)
		{
			fire.gameObject.SetActive(value: true);
			point.gameObject.SetActive(value: false);
			return;
		}
		PickupItem currentItem = MVGameControllerBase.WOCM.AvatarLocal.PickupOwner.CurrentItem;
		if (currentItem.IsHolstered || currentItem == null || currentItem.Type == AvatarItemType.Hand)
		{
			fire.gameObject.SetActive(value: false);
			point.gameObject.SetActive(value: true);
		}
		else
		{
			fire.gameObject.SetActive(value: false);
			point.gameObject.SetActive(value: false);
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
