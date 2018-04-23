using System.Collections.Generic;
using MV.WorldObject;

public class MVMaterialRepository
{
	public const int DEFAULT_MATERIAL_ID = 21;

	private readonly List<MVMaterial> materials = new List<MVMaterial>();

	private readonly MVMaterial noMaterial;

	private readonly MVMaterial inAirMaterial;

	private readonly PhysicalProperties physicalPropertiesDefault = new PhysicalProperties(0.43f, 0f, 1f, 20f, 0f);

	public int MaterialCount => materials.Count;

	public MVMaterial InAirMaterial => inAirMaterial;

	public static bool AllowDestructibleMaterialSelection { get; set; }

	public MVMaterialRepository()
	{
		noMaterial = new MVMaterial(physicalPropertiesDefault, MaterialSound.None, AvatarModifierPackageType.None);
		inAirMaterial = new MVMaterial(MVPhysics.airPhysicalProperties, MaterialSound.None, AvatarModifierPackageType.None);
	}

	public void SetMaterialPrice(int materialID, int materialUnlockPriceGold, int materialUnlockPriceSilver)
	{
		materials[materialID].unlockPriceGold = materialUnlockPriceGold;
	}

	public void SetMaterialUnlocked(int materialId, bool unlocked)
	{
		materials[materialId].isUnlocked = unlocked;
	}

	private bool AllMaterialUnlocked()
	{
		int num = 0;
		foreach (MVMaterial material in materials)
		{
			if (material.isUnlocked)
			{
				num++;
			}
		}
		if (materials.Count == num)
		{
			return true;
		}
		return false;
	}

	public void AddMaterial(string name, string description, string path, MaterialSound materialSound, AvatarModifierPackageType modifierPackageType, int priceGold, int priceSilver, bool isUnlocked, float[] physicalProperties, MaterialButtonTextureGenerator materialButtonTextureGenerator)
	{
		materials.Add(new MVMaterial(materials.Count, name, description, new PhysicalProperties(physicalProperties[0], physicalProperties[1], physicalProperties[2], physicalProperties[3], physicalProperties[4]), materialSound, modifierPackageType, priceGold, priceSilver, isUnlocked, materialButtonTextureGenerator));
	}

	public MVMaterial GetMaterial(byte materialId)
	{
		if (materialId >= materials.Count)
		{
			return noMaterial;
		}
		return materials[materialId];
	}

	public PhysicalProperties GetMaterialPhysicalProperties(byte materialId)
	{
		if (materialId >= materials.Count)
		{
			return noMaterial.physicalProperties;
		}
		return materials[materialId].physicalProperties;
	}
}
