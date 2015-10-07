using System;
using System.Collections.Generic;
using MV.Common;
using UnityEngine;

public class AvatarEquipable : MVEquipable
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
		Debug.Log(string.Concat("Trying to equip type: ", type, ", as a: ", equipType));
		if (equipType == AvatarEquipableType.Modifier)
		{
			switch (type)
			{
			case AvatarItemType.Health:
				interactableLocal.RemoveModifier(AvatarModifierPackageType.Poison);
				interactableLocal.TakeDamage(-50f, null, PlayerKilledByType.None);
				break;
			case AvatarItemType.Mutant:
				interactableLocal.RemoveModifier(AvatarModifierPackageType.NinjaRun);
				interactableLocal.AddModifier(AvatarModifierPackageType.Mutant);
				break;
			case AvatarItemType.NinjaRun:
				interactableLocal.RemoveModifier(AvatarModifierPackageType.Mutant);
				interactableLocal.RemoveModifier(AvatarModifierPackageType.NinjaRun);
				interactableLocal.AddModifier(AvatarModifierPackageType.NinjaRun);
				break;
			case AvatarItemType.MousePack:
				interactableLocal.AddModifier(AvatarModifierPackageType.Shrunken);
				break;
			case AvatarItemType.GrowthPack:
				interactableLocal.AddModifier(AvatarModifierPackageType.Enlarged);
				break;
			default:
				Debug.LogError(string.Concat("AvatarItemType ", type, " does not exist in the switch case, it has not been accounted for yet! AvatarEquipable.cs"));
				return false;
			}
			return true;
		}
		if (!interactableLocal.HasModifierEffect(AvatarModifierEffect.DisableWeapons))
		{
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
		return false;
	}

	public bool GetIsEquipped(AvatarItemType type)
	{
		Dictionary<object, object> dictionary = (Dictionary<object, object>)currentItem.Value;
		foreach (object key in dictionary.Keys)
		{
			if ((string)key == "type" && (int)dictionary[(string)key] == (int)type)
			{
				return true;
			}
		}
		return false;
	}

	public void EquipSlapGun(object sender, EventArgs e)
	{
		currentItem.Value = new Dictionary<object, object> { { "type", 65 } };
	}

	public override void Unequip()
	{
		currentItem.Value = new Dictionary<object, object> { { "type", 5 } };
	}
}
