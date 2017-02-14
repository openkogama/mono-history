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

	public override bool Equip(AvatarItemType type, AvatarEquipableType equipType, Dictionary<object, object> itemData, int variantID = 0, bool holsterable = true)
	{
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
		currentItem.Value = new Dictionary<object, object>
		{
			{ "type", 5 },
			{ "variantId", 0 }
		};
		return false;
	}

	public override void Holster()
	{
		if (currentItem.Value is Dictionary<object, object> dictionary && dictionary.ContainsKey("holstered"))
		{
			dictionary["updateItemState"] = 1;
			dictionary["holstered"] = true;
			currentItem.Value = dictionary;
		}
	}

	public override void Unholster()
	{
		if (currentItem.Value is Dictionary<object, object> dictionary && dictionary.ContainsKey("holstered"))
		{
			dictionary["updateItemState"] = 2;
			dictionary["holstered"] = false;
			currentItem.Value = dictionary;
		}
	}

	public bool GetIsEquipped(AvatarItemType type)
	{
		Dictionary<object, object> dictionary = (Dictionary<object, object>)currentItem.Value;
		if (dictionary.ContainsKey("type") && (int)dictionary["type"] == (int)type)
		{
			return true;
		}
		return false;
	}

	public void EquipSlapGun(object sender, EventArgs e)
	{
		currentItem.Value = new Dictionary<object, object> { { "type", 65 } };
	}

	public override void Unequip()
	{
		Equip(AvatarItemType.Hand, AvatarEquipableType.Weapon, null);
	}
}
