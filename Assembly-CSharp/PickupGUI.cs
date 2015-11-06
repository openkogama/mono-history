using System;
using MV.Common;
using UnityEngine;

public class PickupGUI
{
	private IGUICrossHair crossHair;

	private MVPickupOwner pickupOwner;

	private bool canBeVisible;

	public static bool ShowEquipableUI { get; private set; }

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
			Color magenta = Color.magenta;
			magenta = pickupOwner.CurrentItem.CrossHairColor;
			float chargeState = pickupOwner.CurrentItem.ChargeState;
			bool andResetFiredThisFrame = pickupOwner.CurrentItem.GetAndResetFiredThisFrame();
			crossHair.UpdateCrossHair(num, magenta, chargeState, andResetFiredThisFrame);
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
		ShowEquipableUI = false;
	}

	private void OnEquipItem(PickupItem item)
	{
		if (item.ActivateGunModeOnEquip)
		{
			canBeVisible = true;
			UpdateCrossHairVisibility();
		}
		if (item.Type != AvatarItemType.Hand)
		{
			ShowEquipableUI = true;
		}
	}

	private void OnUnequipItem(PickupItem item)
	{
		canBeVisible = false;
		UpdateCrossHairVisibility();
		ShowEquipableUI = false;
		Debug.Log("Unequip");
	}
}
