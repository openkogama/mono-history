using System;
using UnityEngine;

public class PickupGUI
{
	private MVGUICrossHair guiCrossHair;

	private MVPickupOwner pickupOwner;

	public bool Visible
	{
		get
		{
			return guiCrossHair.group.Visible;
		}
		set
		{
			guiCrossHair.group.SetVisible(value);
		}
	}

	public PickupGUI(MVPickupOwner pickupOwner)
	{
		this.pickupOwner = pickupOwner;
		guiCrossHair = UXUtils.FindGUIObjectOfType<MVGUICrossHair>();
		guiCrossHair.group.SetVisible(visible: false);
		pickupOwner.onEquipItem = (MVPickupOwner.OnEquipItemDelegate)Delegate.Combine(pickupOwner.onEquipItem, new MVPickupOwner.OnEquipItemDelegate(OnEquipItem));
		pickupOwner.onUnequipItem = (MVPickupOwner.OnUnequipItemDelegate)Delegate.Combine(pickupOwner.onUnequipItem, new MVPickupOwner.OnUnequipItemDelegate(OnUnequipItem));
	}

	public void Update()
	{
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		if (!((Object)(object)pickupOwner.CurrentItem == (Object)null))
		{
			int num = 0;
			num = pickupOwner.CurrentItem.Quantity;
			if (num == 0 && guiCrossHair.ammoText.Visible)
			{
				guiCrossHair.ammoText.SetVisible(visible: false);
			}
			if (num > 0 && !guiCrossHair.ammoText.Visible)
			{
				guiCrossHair.ammoText.SetVisible(visible: true);
			}
			if (guiCrossHair.ammoText.Visible)
			{
				guiCrossHair.ammoText.Text = string.Empty + num;
			}
			Color color = Color.magenta;
			if ((Object)(object)pickupOwner.CurrentItem != (Object)null)
			{
				color = pickupOwner.CurrentItem.CrossHairColor;
			}
			guiCrossHair.crossHairPlane.SetColor(color, string.Empty);
			guiCrossHair.chargeText.SetVisible(pickupOwner.CurrentItem.ChargeState > 0f);
			if (guiCrossHair.chargeText.Visible)
			{
				guiCrossHair.chargeText.Text = string.Empty + Mathf.Round(pickupOwner.CurrentItem.ChargeState * 100f);
			}
		}
	}

	public void Destroy()
	{
		MVPickupOwner mVPickupOwner = pickupOwner;
		mVPickupOwner.onEquipItem = (MVPickupOwner.OnEquipItemDelegate)Delegate.Remove(mVPickupOwner.onEquipItem, new MVPickupOwner.OnEquipItemDelegate(OnEquipItem));
		MVPickupOwner mVPickupOwner2 = pickupOwner;
		mVPickupOwner2.onUnequipItem = (MVPickupOwner.OnUnequipItemDelegate)Delegate.Remove(mVPickupOwner2.onUnequipItem, new MVPickupOwner.OnUnequipItemDelegate(OnUnequipItem));
	}

	private void OnEquipItem(PickupItem item)
	{
		if (Object.op_Implicit((Object)(object)guiCrossHair.group) && item.ActivateGunModeOnEquip)
		{
			guiCrossHair.group.SetVisible(visible: true);
		}
	}

	public void Enter()
	{
		if ((Object)(object)pickupOwner.CurrentItem != (Object)null)
		{
			OnEquipItem(pickupOwner.CurrentItem);
		}
	}

	public void Leave()
	{
		Visible = false;
	}

	private void OnUnequipItem(PickupItem item)
	{
		if (Object.op_Implicit((Object)(object)guiCrossHair.group))
		{
			guiCrossHair.group.SetVisible(visible: false);
		}
	}
}
