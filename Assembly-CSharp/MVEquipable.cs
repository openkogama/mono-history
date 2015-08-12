using System.Collections.Generic;
using MV.Common;

public abstract class MVEquipable : MVComponent
{
	public abstract void Equip(AvatarItemType type, Dictionary<object, object> itemData, int variantID = 0);

	public abstract void Unequip();
}
