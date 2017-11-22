using System;
using MV.WorldObject;
using UnityEngine;

public class CellTraverser
{
	private Ray intersectRay = default;

	private Vector3 tMax;

	private Vector3 initialtMax;

	private IntVector localChunkSpaceVoxelPos;

	private int stepX;

	private int stepY;

	private int stepZ;

	private int chunkSize;

	private IntVector voxelPos = new IntVector(0, 0, 0);

	private IntVector stepDir = default;

	private Vector3 tDelta;

	public RuntimePrototypeCubeModel debugRpcm;

	public IntVector VoxelPos => voxelPos;

	public IntVector StepDir => stepDir;

	public void Init(Vector3 localOrigin, CollisionState collisionState)
	{
		chunkSize = collisionState.cmb.PrototypeCubeModel.ChunkSize;
		intersectRay.origin = localOrigin;
		intersectRay.direction = collisionState.localDirection;
		voxelPos = CubeMathFunctions.LocalPosToLocalIntVector(intersectRay.origin);
		voxelPos[collisionState.scanAxis] = (short)Mathf.Clamp(voxelPos[collisionState.scanAxis], collisionState.minBounds[collisionState.scanAxis], collisionState.maxBounds[collisionState.scanAxis]);
		localChunkSpaceVoxelPos = new IntVector(voxelPos.x, voxelPos.y, voxelPos.z);
		collisionState.cmb.CubePosToChunkPos(ref localChunkSpaceVoxelPos);
		stepX = Math.Sign(intersectRay.direction.x);
		stepY = Math.Sign(intersectRay.direction.y);
		stepZ = Math.Sign(intersectRay.direction.z);
		Vector3 vector = new Vector3((int)voxelPos.x + ((stepX > 0) ? 1 : 0), (int)voxelPos.y + ((stepY > 0) ? 1 : 0), (int)voxelPos.z + ((stepZ > 0) ? 1 : 0));
		tMax = new Vector3(float.PositiveInfinity, float.PositiveInfinity, float.PositiveInfinity);
		if (intersectRay.direction.x != 0f)
		{
			tMax.x = (vector.x - (intersectRay.origin.x + 0.5f)) / intersectRay.direction.x;
		}
		if (intersectRay.direction.y != 0f)
		{
			tMax.y = (vector.y - (intersectRay.origin.y + 0.5f)) / intersectRay.direction.y;
		}
		if (intersectRay.direction.z != 0f)
		{
			tMax.z = (vector.z - (intersectRay.origin.z + 0.5f)) / intersectRay.direction.z;
		}
		initialtMax = new Vector3(tMax.x, tMax.y, tMax.z);
		tDelta = new Vector3(float.PositiveInfinity, float.PositiveInfinity, float.PositiveInfinity);
		if (intersectRay.direction.x != 0f)
		{
			tDelta.x = (float)stepX / intersectRay.direction.x;
		}
		if (intersectRay.direction.y != 0f)
		{
			tDelta.y = (float)stepY / intersectRay.direction.y;
		}
		if (intersectRay.direction.z != 0f)
		{
			tDelta.z = (float)stepZ / intersectRay.direction.z;
		}
	}

	public void DebugAll()
	{
		Debug.Log("intersectRay " + intersectRay);
		Debug.Log("tMax " + tMax);
		Debug.Log("localChunkSpaceVoxelPos " + localChunkSpaceVoxelPos);
		Debug.Log("stepX " + stepX);
		Debug.Log("stepY " + stepY);
		Debug.Log("stepZ " + stepZ);
		Debug.Log("tDelta " + tDelta);
		Debug.Log("stepDir " + stepDir);
		Debug.Log("initialtMax " + initialtMax);
	}

	public bool Step()
	{
		bool result = true;
		if (tMax.x < tMax.y && tMax.x < tMax.z)
		{
			localChunkSpaceVoxelPos.x += (short)stepX;
			voxelPos.x += (short)stepX;
			tMax.x += tDelta.x;
			if (localChunkSpaceVoxelPos.x == chunkSize || localChunkSpaceVoxelPos.x < 0)
			{
				result = false;
			}
			stepDir.x = (short)stepX;
			stepDir.y = 0;
			stepDir.z = 0;
		}
		else if (tMax.y < tMax.z)
		{
			localChunkSpaceVoxelPos.y += (short)stepY;
			voxelPos.y += (short)stepY;
			tMax.y += tDelta.y;
			if (localChunkSpaceVoxelPos.y == chunkSize || localChunkSpaceVoxelPos.y < 0)
			{
				result = false;
			}
			stepDir.x = 0;
			stepDir.y = (short)stepY;
			stepDir.z = 0;
		}
		else
		{
			localChunkSpaceVoxelPos.z += (short)stepZ;
			voxelPos.z += (short)stepZ;
			tMax.z += tDelta.z;
			if (localChunkSpaceVoxelPos.z == chunkSize || localChunkSpaceVoxelPos.z < 0)
			{
				result = false;
			}
			stepDir.x = 0;
			stepDir.y = 0;
			stepDir.z = (short)stepZ;
		}
		return result;
	}
}
