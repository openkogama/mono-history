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

	[SerializeField]
	private RectTransform jumpButton;

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
		if (MVGameControllerBase.SpawnRoleDataMediatorLocal.IsSeated != leaveVehicle.gameObject.activeInHierarchy)
		{
			leaveVehicle.gameObject.SetActive(MVGameControllerBase.SpawnRoleDataMediatorLocal.IsSeated);
		}
		if (MVGameControllerBase.SpawnRoleDataMediatorLocal.SpawnRoleModeTypeWrapper.IsInMode(SpawnRoleModeType.Playing))
		{
			HandleInPlayMode();
		}
		else if (!MVGameControllerBase.SpawnRoleDataMediatorLocal.SpawnRoleModeTypeWrapper.IsInMode(SpawnRoleModeType.Playing))
		{
			HandleNotInPlayMode();
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
		}
		else if (!MVGameControllerBase.SpawnRoleDataMediatorLocal.PickupItemIsInHand)
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

	private void HandleInPlayMode()
	{
		if (!respawnButton.gameObject.activeSelf)
		{
			respawnButton.gameObject.SetActive(value: true);
		}
		if (!jumpButton.gameObject.activeSelf)
		{
			jumpButton.gameObject.SetActive(value: true);
		}
	}

	private void HandleNotInPlayMode()
	{
		if (use.gameObject.activeSelf)
		{
			use.gameObject.SetActive(value: false);
		}
		if (fire.gameObject.activeSelf)
		{
			fire.gameObject.SetActive(value: false);
		}
		if (point.gameObject.activeSelf)
		{
			point.gameObject.SetActive(value: false);
		}
		if (dropWeapon.gameObject.activeSelf)
		{
			dropWeapon.gameObject.SetActive(value: false);
		}
		if (leaveVehicle.gameObject.activeSelf)
		{
			leaveVehicle.gameObject.SetActive(value: false);
		}
		if (respawnButton.gameObject.activeSelf)
		{
			respawnButton.gameObject.SetActive(value: false);
		}
		if (holsterButton.gameObject.activeSelf)
		{
			holsterButton.gameObject.SetActive(value: false);
		}
		if (jumpButton.gameObject.activeSelf)
		{
			jumpButton.gameObject.SetActive(value: false);
		}
	}
}
