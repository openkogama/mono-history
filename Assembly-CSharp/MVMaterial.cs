using System;
using CodeStage.AntiCheat.ObscuredTypes;
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

	private ObscuredString textureHashCode = string.Empty;

	public bool IsAvailable
	{
		get
		{
			if (MVMaterialRepository.AllowDestructibleMaterialSelection)
			{
				return true;
			}
			if (physicalProperties.toughness == 0f)
			{
				return true;
			}
			return false;
		}
	}

	public bool IsDestructible
	{
		get
		{
			if (physicalProperties.toughness == 0f)
			{
				return false;
			}
			return true;
		}
	}

	public MVMaterial()
	{
	}

	public MVMaterial(string name, string description, Material material, PhysicalProperties physicalProperties, MaterialSound materialSound, AvatarModifierPackageType modifierPackageType, int priceGold, int priceSilver, bool isUnlocked)
		: this(material, physicalProperties, materialSound, modifierPackageType)
	{
		unlockPriceGold = priceGold;
		unlockPriceSilver = priceSilver;
		this.isUnlocked = isUnlocked;
		this.name = name;
		this.description = description;
	}

	public MVMaterial(Material material, PhysicalProperties physicalProperties, MaterialSound materialSound, AvatarModifierPackageType modifierPackageType)
	{
		this.material = material;
		this.physicalProperties = physicalProperties;
		this.materialSound = materialSound;
		this.modifierPackageType = modifierPackageType;
		textureHashCode = TextureHash.CreateHashCode(material.mainTexture);
	}

	public void Validate()
	{
		string text = TextureHash.CreateHashCode(material.mainTexture);
		if (textureHashCode != (ObscuredString)text)
		{
			throw new Exception("Material texture has been tampered with");
		}
	}
}
