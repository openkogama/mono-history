using System;
using System.Collections.Generic;
using MV.WorldObject;
using UnityEngine;

public class MVMaterialRepository
{
	public const int DEFAULT_MATERIAL_ID = 21;

	private readonly List<MVMaterial> materials = new List<MVMaterial>();

	private MVMaterial noMaterial;

	public int MaterialCount => materials.Count;

	public MVMaterialRepository()
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Expected Obj, but got Unknown
		noMaterial = new MVMaterial((Material)Resources.Load("Materials/CubeMaterialBasics/notexture"), MaterialPhysicalProperties.PhysicalPropertiesDefault);
		AddMaterial("Materials/CubeMaterialBasics/scarletredmat00");
		AddMaterial("Materials/CubeMaterialBasics/scarletredmat01");
		AddMaterial("Materials/CubeMaterialBasics/scarletredmat02");
		AddMaterial("Materials/CubeMaterialBasics/chocolatemat00");
		AddMaterial("Materials/CubeMaterialBasics/plummat00");
		AddMaterial("Materials/CubeMaterialBasics/skybluemat00");
		AddMaterial("Materials/CubeMaterialBasics/skybluemat01");
		AddMaterial("Materials/CubeMaterialBasics/skybluemat02");
		AddMaterial("Materials/CubeMaterialBasics/chocolatemat01");
		AddMaterial("Materials/CubeMaterialBasics/plummat01");
		AddMaterial("Materials/CubeMaterialBasics/chameleonmat00");
		AddMaterial("Materials/CubeMaterialBasics/chameleonmat01");
		AddMaterial("Materials/CubeMaterialBasics/chameleonmat02");
		AddMaterial("Materials/CubeMaterialBasics/chocolatemat02");
		AddMaterial("Materials/CubeMaterialBasics/plummat02");
		AddMaterial("Materials/CubeMaterialBasics/orangemat00");
		AddMaterial("Materials/CubeMaterialBasics/orangemat01");
		AddMaterial("Materials/CubeMaterialBasics/orangemat02");
		AddMaterial("Materials/CubeMaterialBasics/buttermat00");
		AddMaterial("Materials/CubeMaterialBasics/buttermat01");
		AddMaterial("Materials/CubeMaterialBasics/aluminiummat00");
		AddMaterial("Materials/CubeMaterialBasics/aluminiummat01");
		AddMaterial("Materials/CubeMaterialBasics/aluminiummat02");
		AddMaterial("Materials/CubeMaterialBasics/aluminiummat03");
		AddMaterial("Materials/CubeMaterialBasics/buttermat02");
		AddMaterial("Materials/CubeMaterialBasics/func_ice00");
		AddMaterial("Materials/CubeMaterialBasics/func_lava00", typeof(LavaAnimator));
		AddMaterial("Materials/CubeMaterialBasics/func_bouncy00");
	}

	private void AddMaterial(string materialResourceFileName, Type materialAnimatorType = null)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Expected Obj, but got Unknown
		Material material = (Material)Resources.Load(materialResourceFileName);
		if ((object)materialAnimatorType != null)
		{
			CreateMaterialAnimator(material, materialAnimatorType);
		}
		materials.Add(new MVMaterial(material, MaterialPhysicalProperties.GetPhysicalProperty(materials.Count)));
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
			return noMaterial;
		}
		return materials[materialId];
	}
}
