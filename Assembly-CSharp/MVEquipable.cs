using System.Collections;
using MV.Common;

public abstract class MVEquipable : MVComponent
{
	public abstract void Equip(AvatarItemType type, Hashtable itemData, int variantID = 0);

	public abstract void Unequip();
}
