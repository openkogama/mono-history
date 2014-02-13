using MV.WorldObject;
using UnityEngine;

public class MVMaterial
{
	public Material material;

	public string name;

	public string description;

	public PhysicalProperties physicalProperties;

	public MaterialSound materialSound;

	public AvatarModifierPackageType modifierPackageType;

	public int unlockPriceGold;

	public int unlockPriceSilver;

	public bool isUnlocked;

	public MVMaterial()
	{
	}

	public MVMaterial(string name, string description, Material material, PhysicalProperties physicalProperties, MaterialSound materialSound, AvatarModifierPackageType modifierPackageType, int priceGold, int priceSilver, bool isUnlocked)
	{
		this.name = name;
		this.description = description;
		this.material = material;
		this.physicalProperties = physicalProperties;
		this.materialSound = materialSound;
		this.modifierPackageType = modifierPackageType;
		unlockPriceGold = priceGold;
		unlockPriceSilver = priceSilver;
		this.isUnlocked = isUnlocked;
	}

	public MVMaterial(Material material, PhysicalProperties physicalProperties, MaterialSound materialSound, AvatarModifierPackageType modifierPackageType)
	{
		this.material = material;
		this.physicalProperties = physicalProperties;
		this.materialSound = materialSound;
		this.modifierPackageType = modifierPackageType;
		unlockPriceGold = 0;
		unlockPriceSilver = 0;
	}
}
