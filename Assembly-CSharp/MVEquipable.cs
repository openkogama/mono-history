using System.Collections.Generic;
using MV.Common;

public abstract class MVEquipable : MVComponent
{
	public abstract bool Equip(AvatarItemType type, AvatarEquipableType equipType, Dictionary<object, object> itemData, int variantID = 0);

	public abstract void Unequip();

	public abstract void Holster();

	public abstract void Unholster();
}
