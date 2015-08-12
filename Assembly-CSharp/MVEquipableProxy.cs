using System.Collections.Generic;
using MV.Common;
using UnityEngine;

public class MVEquipableProxy : MVEquipable
{
	private MVEquipable equipable;

	public void Init(MVEquipable equipable)
	{
		this.equipable = equipable;
	}

	public override void Equip(AvatarItemType type, Dictionary<object, object> itemData, int variantID = 0)
	{
		Debug.Log("Equip");
		equipable.Equip(type, itemData, variantID);
	}

	public override void Unequip()
	{
		equipable.Unequip();
	}
}
