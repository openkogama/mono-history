using MV.WorldObject;
using UnityEngine;

public struct MVTerrainLOD(IntVector localPos, Vector3 worldPos)
{
	public IntVector localPos = localPos;

	public Vector3 worldPos = worldPos;
}
