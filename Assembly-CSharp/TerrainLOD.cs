using MV.WorldObject;
using UnityEngine;

internal struct TerrainLOD(IntVector localPos, Vector3 worldPos)
{
	public IntVector localPos = localPos;

	public Vector3 worldPos = worldPos;

	public int lodId = 0;
}
