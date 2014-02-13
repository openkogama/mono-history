using System.Collections;
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

	public override void Equip(AvatarItemType type, Hashtable itemData, int variantID = 0)
	{
		switch (type)
		{
		case AvatarItemType.Health:
			interactableLocal.RemoveModifier(AvatarModifierPackageType.Poison);
			interactableLocal.RemoveModifier(AvatarModifierPackageType.Mutant);
			interactableLocal.TakeDamage(-50f, null, PlayerKilledByType.None);
			return;
		case AvatarItemType.Mutant:
			interactableLocal.AddModifier(AvatarModifierPackageType.Mutant);
			Unequip();
			return;
		}
		if (!interactableLocal.HasModifier(AvatarModifierPackageType.Mutant))
		{
			if (itemData != null)
			{
				currentItem.Value = new Hashtable
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
				currentItem.Value = new Hashtable
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
		currentItem.Value = new Hashtable { { "type", 5 } };
	}
}
