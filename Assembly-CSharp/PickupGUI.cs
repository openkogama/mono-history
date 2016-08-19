using System;
using UnityEngine;

public class PickupGUI
{
	private IGUICrossHair crossHair;

	private MVPickupOwner pickupOwner;

	private bool canBeVisible;

	public static PickupGUIFlags ShowEquipableUI { get; private set; }

	public PickupGUI(MVPickupOwner pickupOwner)
	{
		this.pickupOwner = pickupOwner;
		crossHair = MVGameControllerBase.IPlayModeUI.GetCrossHair();
		crossHair.Visible = false;
		pickupOwner.onEquipItem = (MVPickupOwner.OnEquipItemDelegate)Delegate.Combine(pickupOwner.onEquipItem, new MVPickupOwner.OnEquipItemDelegate(OnEquipItem));
		pickupOwner.onUnequipItem = (MVPickupOwner.OnUnequipItemDelegate)Delegate.Combine(pickupOwner.onUnequipItem, new MVPickupOwner.OnUnequipItemDelegate(OnUnequipItem));
	}

	public void Update()
	{
		if (!(pickupOwner.CurrentItem == null))
		{
			int num = 0;
			num = pickupOwner.CurrentItem.Quantity;
			Color crossHairColor = pickupOwner.CurrentItem.CrossHairColor;
			float chargeState = pickupOwner.CurrentItem.ChargeState;
			bool andResetFiredThisFrame = pickupOwner.CurrentItem.GetAndResetFiredThisFrame();
			crossHair.UpdateCrossHair(num, crossHairColor, chargeState, andResetFiredThisFrame);
			UpdateCrossHairVisibility();
		}
	}

	public void Destroy()
	{
		MVPickupOwner mVPickupOwner = pickupOwner;
		mVPickupOwner.onEquipItem = (MVPickupOwner.OnEquipItemDelegate)Delegate.Remove(mVPickupOwner.onEquipItem, new MVPickupOwner.OnEquipItemDelegate(OnEquipItem));
		MVPickupOwner mVPickupOwner2 = pickupOwner;
		mVPickupOwner2.onUnequipItem = (MVPickupOwner.OnUnequipItemDelegate)Delegate.Remove(mVPickupOwner2.onUnequipItem, new MVPickupOwner.OnUnequipItemDelegate(OnUnequipItem));
	}

	public void Enter()
	{
		if (pickupOwner.CurrentItem != null)
		{
			OnEquipItem(pickupOwner.CurrentItem);
		}
	}

	private void UpdateCrossHairVisibility()
	{
		bool flag = !MVGameControllerBase.IPlayModeUI.InLobbyState && canBeVisible;
		if (crossHair.Visible != flag)
		{
			crossHair.Visible = flag;
		}
	}

	public void Leave()
	{
		canBeVisible = false;
		UpdateCrossHairVisibility();
		ShowEquipableUI = PickupGUIFlags.None;
	}

	private void OnEquipItem(PickupItem item)
	{
		if (item.ActivateGunModeOnEquip)
		{
			canBeVisible = true;
			UpdateCrossHairVisibility();
			ShowEquipableUI |= PickupGUIFlags.ShowCrosshair;
		}
		if (item.CanFire())
		{
			ShowEquipableUI |= PickupGUIFlags.CanFire;
		}
		if (item.CanUnequip)
		{
			ShowEquipableUI |= PickupGUIFlags.CanUnequip;
		}
	}

	private void OnUnequipItem(PickupItem item)
	{
		canBeVisible = false;
		crossHair.Visible = false;
		ShowEquipableUI = PickupGUIFlags.None;
	}
}
