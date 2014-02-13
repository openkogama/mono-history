using System;
using System.Collections.Generic;
using MV.WorldObject;
using UnityEngine;

public class MVMaterialRepository
{
	public const int DEFAULT_MATERIAL_ID = 21;

	private readonly List<MVMaterial> materials = new List<MVMaterial>();

	private MVMaterial noMaterial;

	private MVMaterial inAirMaterial = new MVMaterial(null, MVPhysics.airPhysicalProperties, MaterialSound.None, AvatarModifierPackageType.None);

	private PhysicalProperties physicalPropertiesDefault = new PhysicalProperties(0.43f, 0f, 1f, 20f, 100000f);

	public int MaterialCount => materials.Count;

	public MVMaterial InAirMaterial => inAirMaterial;

	public MVMaterialRepository()
	{
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Expected Obj, but got Unknown
		noMaterial = new MVMaterial((Material)Resources.Load("Cube/Materials/notexture"), physicalPropertiesDefault, MaterialSound.None, AvatarModifierPackageType.None);
	}

	public void SetMaterialPrice(int materialID, int materialUnlockPriceGold, int materialUnlockPriceSilver)
	{
		materials[materialID].unlockPriceGold = materialUnlockPriceGold;
	}

	public void SetMaterialUnlocked(int materialId, bool unlocked)
	{
		materials[materialId].isUnlocked = unlocked;
	}

	public void AddMaterial(string name, string description, string path, MaterialSound materialSound, AvatarModifierPackageType modifierPackageType, int priceGold, int priceSilver, bool isUnlocked, float[] physicalProperties, Type materialAnimatorType = null)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Expected Obj, but got Unknown
		Material material = (Material)Resources.Load(path);
		if ((object)materialAnimatorType != null)
		{
			CreateMaterialAnimator(material, materialAnimatorType);
		}
		materials.Add(new MVMaterial(name, description, material, new PhysicalProperties(physicalProperties[0], physicalProperties[1], physicalProperties[2], physicalProperties[3], physicalProperties[4]), materialSound, modifierPackageType, priceGold, priceSilver, isUnlocked));
	}

	private void CreateMaterialAnimator(Material material, Type materialAnimatorType)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Expected Obj, but got Unknown
		GameObject val = new GameObject($"Material Animator ({materialAnimatorType.Name}).");
		Object.DontDestroyOnLoad((Object)(object)val);
		MaterialAnimator materialAnimator = val.AddComponent(materialAnimatorType) as MaterialAnimator;
		materialAnimator.TargetMaterial = material;
	}

	public MVMaterial GetMaterial(byte materialId)
	{
		if (materialId >= materials.Count)
		{
			Debug.LogError((object)"Material out of range");
			return noMaterial;
		}
		return materials[materialId];
	}

	public PhysicalProperties GetMaterialPhysicalProperties(byte materialId)
	{
		if (materialId >= materials.Count)
		{
			Debug.LogError((object)"Material out of range");
			return noMaterial.physicalProperties;
		}
		return materials[materialId].physicalProperties;
	}
}
