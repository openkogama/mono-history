using MV.WorldObject;
using UnityEngine;

public struct MVMaterial(Material material, PhysicalProperties physicalProperties)
{
	public Material material = material;

	public PhysicalProperties physicalProperties = physicalProperties;
}
