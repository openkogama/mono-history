using System;
using UnityEngine;

namespace MV.WorldObject;

public static class CubeMathFunctions
{
	public static IntVector LocalPosToLocalIntVector(Vector3 localPos)
	{
		Vector3 vector = localPos + Vector3.one * 0.5f;
		vector.x = (float)Math.Floor(vector.x);
		vector.y = (float)Math.Floor(vector.y);
		vector.z = (float)Math.Floor(vector.z);
		return new IntVector((short)vector.x, (short)vector.y, (short)vector.z);
	}

	public static Vector3 LocalIntVectorToLocalPos(IntVector localIntVector)
	{
		return new Vector3(localIntVector.x, localIntVector.y, localIntVector.z);
	}

	public static IntVector WorldPosToFineGrainedLocalPos(Vector3 worldPos)
	{
		worldPos += Vector3.one * 1.5f;
		worldPos.x = (float)Math.Round(worldPos.x, 0);
		worldPos.y = (float)Math.Round(worldPos.y, 0);
		worldPos.z = (float)Math.Round(worldPos.z, 0);
		return new IntVector(worldPos.x, worldPos.y, worldPos.z);
	}

	public static IntVector WorldPosToFineGrainedLocalPos(Vector3 worldPos, Vector3 normal)
	{
		worldPos += Vector3.one * 1.5f;
		worldPos -= normal * 0.01f;
		worldPos.x = (float)Math.Round(worldPos.x, 0);
		worldPos.y = (float)Math.Round(worldPos.y, 0);
		worldPos.z = (float)Math.Round(worldPos.z, 0);
		return new IntVector(worldPos.x, worldPos.y, worldPos.z);
	}

	public static Vector3 FineGrainedLocalPosToWorldPos(IntVector intVector)
	{
		return new Vector3(intVector.x, intVector.y, intVector.z) - Vector3.one * 1.5f;
	}

	public static Vector3 FineGrainedLocalPosToTerrainLocalPos(IntVector intVector)
	{
		Vector3 vector = FineGrainedLocalPosToWorldPos(intVector);
		return vector / 4f;
	}

	public static IntVector FromLocalPosToLocalPos(IntVector fineGrainedPosition, ICubeModel terrainWorldObject, ICubeModel fineGrainedTerrainWorldObject)
	{
		float num = fineGrainedTerrainWorldObject.Scale[0];
		float num2 = terrainWorldObject.Scale[0];
		float num3 = num / num2;
		return new IntVector(Mathf.Floor((float)fineGrainedPosition.x * num3), Mathf.Floor((float)fineGrainedPosition.y * num3), Mathf.Floor((float)fineGrainedPosition.z * num3));
	}
}
