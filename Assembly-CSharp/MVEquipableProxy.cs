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

	public override bool Equip(AvatarItemType type, AvatarEquipableType equipType, Dictionary<object, object> itemData, int variantID = 0)
	{
		Debug.Log("Equip");
		return equipable.Equip(type, equipType, itemData, variantID);
	}

	public override void Unequip()
	{
		equipable.Unequip();
	}
}
