using System;
using UnityEngine;

public class PickupGUI
{
	private UXGroup crossHairGroup;

	private UXPlane crossHairPlane;

	private UXText ammoText;

	private UXText chargeText;

	private MVPickupOwner pickupOwner;

	public bool Visible
	{
		get
		{
			return crossHairGroup.Visible;
		}
		set
		{
			crossHairGroup.SetVisible(value);
		}
	}

	public PickupGUI(MVPickupOwner pickupOwner)
	{
		this.pickupOwner = pickupOwner;
		crossHairGroup = GameObject.Find("CrossHair").GetComponent<UXGroup>();
		crossHairPlane = ((Component)((Component)crossHairGroup).transform.FindChild("CrossHairPlane")).GetComponentInChildren<UXPlane>();
		ammoText = ((Component)((Component)crossHairGroup).transform.FindChild("Ammo")).GetComponentInChildren<UXText>();
		chargeText = ((Component)((Component)crossHairGroup).transform.FindChild("Charge")).GetComponentInChildren<UXText>();
		crossHairGroup.SetVisible(visible: false);
		pickupOwner.onEquipItem = (MVPickupOwner.OnEquipItemDelegate)Delegate.Combine(pickupOwner.onEquipItem, new MVPickupOwner.OnEquipItemDelegate(OnEquipItem));
		pickupOwner.onUnequipItem = (MVPickupOwner.OnUnequipItemDelegate)Delegate.Combine(pickupOwner.onUnequipItem, new MVPickupOwner.OnUnequipItemDelegate(OnUnequipItem));
	}

	public void Update()
	{
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		if (!((Object)(object)pickupOwner.CurrentItem == (Object)null))
		{
			int num = 0;
			num = pickupOwner.CurrentItem.Quantity;
			if (num == 0 && ammoText.Visible)
			{
				ammoText.SetVisible(visible: false);
			}
			if (num > 0 && !ammoText.Visible)
			{
				ammoText.SetVisible(visible: true);
			}
			if (ammoText.Visible)
			{
				ammoText.Text = string.Empty + num;
			}
			Color color = Color.magenta;
			if ((Object)(object)pickupOwner.CurrentItem != (Object)null)
			{
				color = pickupOwner.CurrentItem.CrossHairColor;
			}
			crossHairPlane.SetColor(color, string.Empty);
			chargeText.SetVisible(pickupOwner.CurrentItem.ChargeState > 0f);
			if (chargeText.Visible)
			{
				chargeText.Text = string.Empty + Mathf.Round(pickupOwner.CurrentItem.ChargeState * 100f);
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
		if (Object.op_Implicit((Object)(object)crossHairGroup) && item.ActivateGunModeOnEquip)
		{
			crossHairGroup.SetVisible(visible: true);
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
		if (Object.op_Implicit((Object)(object)crossHairGroup))
		{
			crossHairGroup.SetVisible(visible: false);
		}
	}
}
