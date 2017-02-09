using System.Collections.Generic;
using MV.Common;

public class MVEquipableProxy : MVEquipable
{
	private MVEquipable equipable;

	public void Init(MVEquipable equipable)
	{
		this.equipable = equipable;
	}

	public override bool Equip(AvatarItemType type, AvatarEquipableType equipType, Dictionary<object, object> itemData, int variantID = 0, bool holsterable = true)
	{
		return equipable.Equip(type, equipType, itemData, variantID);
	}

	public override void Unequip()
	{
		equipable.Unequip();
	}

	public override void Holster()
	{
	}

	public override void Unholster()
	{
	}
}
