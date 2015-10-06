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

	public override bool Equip(AvatarItemType type, AvatarEquipableType equipType, Dictionary<object, object> itemData, int variantID = 0)
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
		if (itemData != null)
		{
			currentItem.Value = new Dictionary<object, object>
			{
				{
					"type",
					(int)type
				},
				{ "variantId", variantID },
				{ "itemData", itemData }
			};
		}
		else
		{
			currentItem.Value = new Dictionary<object, object>
			{
				{
					"type",
					(int)type
				},
				{ "variantId", variantID }
			};
		}
		return true;
	}

	public override void Unequip()
	{
		currentItem.Value = new Dictionary<object, object>();
	}
}
