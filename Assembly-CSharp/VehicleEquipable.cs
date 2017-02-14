using System.Collections.Generic;
using MV.Common;
using UnityEngine;

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
				interactableLocal.TakeDamage(-50f, null, PlayerKilledByType.None);
				return true;
			}
			Debug.Log(string.Concat("ignoring ", type, " on car pickup"));
			return false;
		}
		Dictionary<object, object> dictionary = new Dictionary<object, object>();
		dictionary.Add("type", (int)type);
		dictionary.Add("variantId", variantID);
		dictionary.Add("updateItemState", 4);
		Dictionary<object, object> dictionary2 = dictionary;
		if (holsterable)
		{
			dictionary2.Add("holstered", false);
		}
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
			dictionary["holstered"] = true;
			currentItem.Value = dictionary;
		}
	}

	public override void Unholster()
	{
		if (currentItem.Value is Dictionary<object, object> dictionary)
		{
			dictionary["updateItemState"] = 2;
			dictionary["holstered"] = false;
			currentItem.Value = dictionary;
		}
	}

	public override void Unequip()
	{
		if (currentItem.Value is Dictionary<object, object> dictionary && dictionary.ContainsKey("holstered") && (bool)dictionary["holstered"])
		{
			Debug.Log("Not unequipping holstered weapon");
		}
		else
		{
			currentItem.Value = new Dictionary<object, object>();
		}
	}
}
