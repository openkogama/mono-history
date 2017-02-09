using System;
using MV.Common;
using UnityEngine;

public class PickupGUI : MonoBehaviour
{
	private IGUICrossHair crossHair;

	private MVPickupOwner pickupOwner;

	private bool canBeVisible;

	public static PickupGUIFlags ShowEquipableUI { get; private set; }

	public void Initialize(MVPickupOwner pickupOwner)
	{
		this.pickupOwner = pickupOwner;
		crossHair = MVGameControllerBase.IPlayModeUI.GetCrossHair();
		crossHair.Visible = false;
		pickupOwner.onEquipItem = (MVPickupOwner.OnEquipItemDelegate)Delegate.Combine(pickupOwner.onEquipItem, new MVPickupOwner.OnEquipItemDelegate(OnEquipItem));
		pickupOwner.onUnequipItem = (MVPickupOwner.OnUnequipItemDelegate)Delegate.Combine(pickupOwner.onUnequipItem, new MVPickupOwner.OnUnequipItemDelegate(OnUnequipItem));
		pickupOwner.OnHolsteredChanged = (Action<bool>)Delegate.Combine(pickupOwner.OnHolsteredChanged, new Action<bool>(OnHolstered));
	}

	public void OnHolstered(bool isHolstered)
	{
		if (!MVGameControllerBase.WOCM.AvatarLocal.IsSeated)
		{
			crossHair.HolsterStateChanged(isHolstered);
		}
		if (isHolstered)
		{
			ShowEquipableUI |= PickupGUIFlags.IsHolstered;
		}
		else
		{
			ShowEquipableUI &= ~PickupGUIFlags.IsHolstered;
		}
		crossHair.Visible = false;
	}

	private void LateUpdate()
	{
		if (pickupOwner.InGunMode)
		{
			crossHair.UpdateCrossHair(pickupOwner.CurrentItem);
			UpdateCrossHairVisibility();
		}
	}

	private void OnDestroy()
	{
		MVPickupOwner mVPickupOwner = pickupOwner;
		mVPickupOwner.onEquipItem = (MVPickupOwner.OnEquipItemDelegate)Delegate.Remove(mVPickupOwner.onEquipItem, new MVPickupOwner.OnEquipItemDelegate(OnEquipItem));
		MVPickupOwner mVPickupOwner2 = pickupOwner;
		mVPickupOwner2.onUnequipItem = (MVPickupOwner.OnUnequipItemDelegate)Delegate.Remove(mVPickupOwner2.onUnequipItem, new MVPickupOwner.OnUnequipItemDelegate(OnUnequipItem));
		MVPickupOwner mVPickupOwner3 = pickupOwner;
		mVPickupOwner3.OnHolsteredChanged = (Action<bool>)Delegate.Remove(mVPickupOwner3.OnHolsteredChanged, new Action<bool>(OnHolstered));
	}

	public void Enter()
	{
		if (pickupOwner.CurrentItem == null)
		{
			ShowEquipableUI = PickupGUIFlags.None;
			return;
		}
		enabled = true;
		OnEquipItem(pickupOwner.CurrentItem);
		pickupOwner.CurrentItem.OnEnterVehicleWithWeapon();
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
		enabled = false;
		GameObject gameObject = MVGameControllerBase.WOCM.AvatarLocal.GameObject;
		MVPickupOwner component = gameObject.GetComponent<MVPickupOwner>();
		if (component != null)
		{
			if (component.CurrentItem != null && component.CurrentItem.Type != AvatarItemType.Hand)
			{
				OnEquipItem(component.CurrentItem);
				crossHair.UpdateCrossHair(component.CurrentItem);
				UpdateCrossHairVisibility();
				ShowEquipableUI &= ~PickupGUIFlags.IsHolstered;
			}
			else
			{
				canBeVisible = false;
				UpdateCrossHairVisibility();
				ShowEquipableUI = PickupGUIFlags.None;
			}
		}
		if (pickupOwner.CurrentItem != null)
		{
			pickupOwner.CurrentItem.OnLeaveVehicleWithWeapon();
		}
	}

	private void OnEquipItem(PickupItem item)
	{
		if (item.CanHolster)
		{
			if (item.IsHolstered)
			{
				ShowEquipableUI |= PickupGUIFlags.IsHolstered;
			}
			ShowEquipableUI |= PickupGUIFlags.CanHolster;
		}
		if (item.ActivateGunModeOnEquip)
		{
			canBeVisible = true;
			ShowEquipableUI |= PickupGUIFlags.ShowCrosshair;
		}
		UpdateCrossHairVisibility();
		crossHair.UpdateCrossHair(item);
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
