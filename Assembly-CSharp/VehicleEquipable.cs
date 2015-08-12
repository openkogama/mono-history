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

	public override void Equip(AvatarItemType type, Dictionary<object, object> itemData, int variantID = 0)
	{
		switch (type)
		{
		case AvatarItemType.Health:
			interactableLocal.TakeDamage(-50f, null, PlayerKilledByType.None);
			return;
		case AvatarItemType.Mutant:
			Debug.Log("Ignoring mutant on car pick up");
			return;
		case AvatarItemType.NinjaRun:
			Debug.Log("Ignoring Ninjarun on car pickup");
			return;
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
	}

	public override void Unequip()
	{
		currentItem.Value = new Dictionary<object, object>();
	}
}
