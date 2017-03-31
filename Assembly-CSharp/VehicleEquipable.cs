using System.Collections.Generic;
using MV.Common;

public class VehicleEquipable : MVEquipable
{
	private MVInteractableBase interactableLocal;

	private MVRuntimeDataVariable currentItem;

	public void Init(MVInteractableBase interactableLocal, MVRuntimeDataVariable currentItem)
	{
		this.interactableLocal = interactableLocal;
		this.currentItem = currentItem;
	}

	public override bool Equip(AvatarItemType type, AvatarEquipableType equipType, Dictionary<object, object> itemData, int variantID = 0, bool holsterable = true)
	{
		if (equipType == AvatarEquipableType.Modifier)
		{
			if (type == AvatarItemType.Health)
			{
				interactableLocal.TakeDamage(float.NegativeInfinity, null, PlayerKilledByType.None);
				return true;
			}
			return false;
		}
		Dictionary<object, object> dictionary = new Dictionary<object, object>();
		dictionary.Add("type", (int)type);
		dictionary.Add("variantId", variantID);
		dictionary.Add("updateItemState", 4);
		Dictionary<object, object> dictionary2 = dictionary;
		if (itemData != null)
		{
			dictionary2.Add("itemData", itemData);
		}
		currentItem.Value = dictionary2;
		return true;
	}

	public override void Holster()
	{
		if (currentItem.Value is Dictionary<object, object> dictionary)
		{
			dictionary["updateItemState"] = 1;
			currentItem.Value = dictionary;
		}
	}

	public override void Unholster()
	{
		if (currentItem.Value is Dictionary<object, object> dictionary)
		{
			dictionary["updateItemState"] = 2;
			currentItem.Value = dictionary;
		}
	}

	public override void Unequip()
	{
		currentItem.Value = new Dictionary<object, object>();
	}
}
