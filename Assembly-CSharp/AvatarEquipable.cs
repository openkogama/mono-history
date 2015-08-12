using System.Collections.Generic;
using MV.Common;

public class AvatarEquipable : MVEquipable
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
			interactableLocal.RemoveModifier(AvatarModifierPackageType.Poison);
			interactableLocal.RemoveModifier(AvatarModifierPackageType.Mutant);
			interactableLocal.TakeDamage(-50f, null, PlayerKilledByType.None);
			return;
		case AvatarItemType.Mutant:
			interactableLocal.RemoveModifier(AvatarModifierPackageType.NinjaRun);
			interactableLocal.AddModifier(AvatarModifierPackageType.Mutant);
			Unequip();
			return;
		case AvatarItemType.NinjaRun:
			interactableLocal.RemoveModifier(AvatarModifierPackageType.Mutant);
			interactableLocal.RemoveModifier(AvatarModifierPackageType.NinjaRun);
			interactableLocal.AddModifier(AvatarModifierPackageType.NinjaRun);
			return;
		}
		if (!interactableLocal.HasModifier(AvatarModifierPackageType.Mutant))
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
		}
	}

	public override void Unequip()
	{
		currentItem.Value = new Dictionary<object, object> { { "type", 5 } };
	}
}
