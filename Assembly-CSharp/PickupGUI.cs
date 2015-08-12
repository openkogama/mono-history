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
		guiCrossHair = (UnityEngine.Object.Instantiate(Resources.Load("Prefabs/GUI/CrossHair")) as GameObject).GetComponent<MVGUICrossHair>();
		guiCrossHair.group.SetVisible(visible: false);
		pickupOwner.onEquipItem = (MVPickupOwner.OnEquipItemDelegate)Delegate.Combine(pickupOwner.onEquipItem, new MVPickupOwner.OnEquipItemDelegate(OnEquipItem));
		pickupOwner.onUnequipItem = (MVPickupOwner.OnUnequipItemDelegate)Delegate.Combine(pickupOwner.onUnequipItem, new MVPickupOwner.OnUnequipItemDelegate(OnUnequipItem));
	}

	public void Update()
	{
		if (!(pickupOwner.CurrentItem == null))
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
			if (pickupOwner.CurrentItem != null)
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
		if ((bool)guiCrossHair.group && item.ActivateGunModeOnEquip)
		{
			guiCrossHair.group.SetVisible(visible: true);
		}
	}

	public void Enter()
	{
		if (pickupOwner.CurrentItem != null)
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
		if ((bool)guiCrossHair.group)
		{
			guiCrossHair.group.SetVisible(visible: false);
		}
	}
}
