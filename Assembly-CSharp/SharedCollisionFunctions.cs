using System;
using System.Collections.Generic;
using MV.WorldObject;
using UnityEngine;

public static class SharedCollisionFunctions
{
	public static PhysicsCollisionData[] GetPhysicsCollisionData(Collider[] overlapResult, RaycastHit[] hits, Vector3 origin)
	{
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		Array.Sort(hits, (RaycastHit hit0, RaycastHit hit1) => hit0.distance.CompareTo(hit1.distance));
		PhysicsCollisionData[] array = new PhysicsCollisionData[overlapResult.Length + hits.Length];
		for (int num = 0; num < overlapResult.Length; num++)
		{
			ref PhysicsCollisionData reference = ref array[num];
			reference = new PhysicsCollisionData(((Component)overlapResult[num]).collider, origin);
		}
		for (int num2 = overlapResult.Length; num2 < hits.Length + overlapResult.Length; num2++)
		{
			ref PhysicsCollisionData reference2 = ref array[num2];
			reference2 = new PhysicsCollisionData(hits[num2 - overlapResult.Length]);
		}
		return array;
	}

	public static void GetVoxelBounds(ref IntVector min, ref IntVector max, Bounds localSpaceBounds)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		Vector3 vector = localSpaceBounds.min + Vector3.one * 0.51f;
		Vector3 vector2 = localSpaceBounds.max + Vector3.one * 0.49f;
		MathFunctions.FloorVector(ref vector);
		MathFunctions.FloorVector(ref vector2);
		for (int i = 0; i < 3; i++)
		{
			min[i] = (short)vector[i];
			max[i] = (short)vector2[i];
		}
	}

	public static void SetToNoneVoxelHit(ref VoxelHit voxelHit, PhysicsCollisionData hit, int woId)
	{
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		voxelHit.collider = hit.collider;
		voxelHit.transform = hit.transform;
		voxelHit.distance = hit.distance;
		voxelHit.point = hit.point;
		voxelHit.normal = hit.normal;
		voxelHit.woId = woId;
		voxelHit.isCubeHit = false;
		voxelHit.interactionFlags = MVGameController.Instance.WOCM.GetWorldObjectClient(woId).InteractionFlags;
	}

	public static void SetToVoxelHit(ref VoxelHit voxelHit0, ref VoxelHit voxelHit1)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		voxelHit0.point = voxelHit1.point;
		voxelHit0.transform = voxelHit1.transform;
		voxelHit0.normal = voxelHit1.normal;
		voxelHit0.distance = voxelHit1.distance;
		voxelHit0.face = voxelHit1.face;
		voxelHit0.cubePos = voxelHit1.cubePos;
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
