using System.Collections;
using MV.Common;
using UnityEngine;

public class MVEquipableProxy : MVEquipable
{
	private MVEquipable equipable;

	public void Init(MVEquipable equipable)
	{
		this.equipable = equipable;
	}

	public override void Equip(AvatarItemType type, Hashtable itemData, int variantID = 0)
	{
		Debug.Log((object)"Equip");
		equipable.Equip(type, itemData, variantID);
	}

	public override void Unequip()
	{
		equipable.Unequip();
	}
}
