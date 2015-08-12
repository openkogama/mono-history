using System;
using System.Collections.Generic;
using MV.WorldObject;
using UnityEngine;

public static class SharedCollisionFunctions
{
	private static PhysicsCollisionDatasWrapper physicsCollisionWrapper = new PhysicsCollisionDatasWrapper();

	public static PhysicsCollisionDatasWrapper GetPhysicsCollisionData(Collider[] overlapResult, RaycastHit[] hits, Vector3 origin)
	{
		physicsCollisionWrapper.Clear();
		Array.Sort(hits, (RaycastHit hit0, RaycastHit hit1) => hit0.distance.CompareTo(hit1.distance));
		for (int num = 0; num < overlapResult.Length; num++)
		{
			Collider component = overlapResult[num].GetComponent<Collider>();
			physicsCollisionWrapper.Add(component, origin);
		}
		for (int num2 = 0; num2 < hits.Length; num2++)
		{
			physicsCollisionWrapper.Add(hits[num2]);
		}
		return physicsCollisionWrapper;
	}

	public static void GetVoxelBounds(ref IntVector min, ref IntVector max, Bounds localSpaceBounds)
	{
		Vector3 vector = localSpaceBounds.min + Vector3.one * 0.51f;
		Vector3 vector2 = localSpaceBounds.max + Vector3.one * 0.49f;
		vector = MathFunctions.FloorVector(vector);
		vector2 = MathFunctions.FloorVector(vector2);
		for (int i = 0; i < 3; i++)
		{
			min[i] = (short)vector[i];
			max[i] = (short)vector2[i];
		}
	}

	public static void SetToNoneVoxelHit(ref VoxelHit voxelHit, PhysicsCollisionData hit, int woId)
	{
		voxelHit.collider = hit.collider;
		voxelHit.transform = hit.transform;
		voxelHit.distance = hit.distance;
		voxelHit.point = hit.point;
		voxelHit.normal = hit.normal;
		voxelHit.woId = woId;
		voxelHit.isCubeHit = false;
		voxelHit.interactionFlags = MVGameController.WOCM.GetWorldObjectClient(woId).InteractionFlags;
	}

	public static void SetToVoxelHit(ref VoxelHit voxelHit0, ref VoxelHit voxelHit1)
	{
		voxelHit0.point = voxelHit1.point;
		voxelHit0.transform = voxelHit1.transform;
		voxelHit0.normal = voxelHit1.normal;
		voxelHit0.distance = voxelHit1.distance;
		voxelHit0.face = voxelHit1.face;
		voxelHit0.cubePos = voxelHit1.cubePos;
		voxelHit0.cube = voxelHit1.cube;
		voxelHit0.woId = voxelHit1.woId;
		voxelHit0.isCubeHit = true;
		voxelHit0.collider = voxelHit1.collider;
		voxelHit0.interactionFlags = voxelHit1.interactionFlags;
	}

	public static bool IgnoreCollision(MVWorldObjectClient wo, HashSet<int> ignoreWoIds)
	{
		if (wo == null)
		{
			return true;
		}
		if (ignoreWoIds != null && ignoreWoIds.Contains(wo.Id))
		{
			return true;
		}
		return false;
	}
}
