using System;
using MV.WorldObject;
using UnityEngine;

public class CellTraverser
{
	private Ray intersectRay;

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
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_0189: Unknown result type (might be due to invalid IL or missing references)
		//IL_019c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_020a: Unknown result type (might be due to invalid IL or missing references)
		//IL_020f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0225: Unknown result type (might be due to invalid IL or missing references)
		//IL_022a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0234: Unknown result type (might be due to invalid IL or missing references)
		//IL_0239: Unknown result type (might be due to invalid IL or missing references)
		//IL_030e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0313: Unknown result type (might be due to invalid IL or missing references)
		//IL_0326: Unknown result type (might be due to invalid IL or missing references)
		//IL_032b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0342: Unknown result type (might be due to invalid IL or missing references)
		//IL_0347: Unknown result type (might be due to invalid IL or missing references)
		//IL_035e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0363: Unknown result type (might be due to invalid IL or missing references)
		//IL_036d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0372: Unknown result type (might be due to invalid IL or missing references)
		chunkSize = CubeModelChunk.ChunkSize;
		intersectRay.origin = localOrigin;
		intersectRay.direction = collisionState.localDirection;
		voxelPos = CubeMathFunctions.LocalPosToLocalIntVector(intersectRay.origin);
		voxelPos[collisionState.scanAxis] = (short)Mathf.Clamp((int)voxelPos[collisionState.scanAxis], (int)collisionState.minBounds[collisionState.scanAxis], (int)collisionState.maxBounds[collisionState.scanAxis]);
		localChunkSpaceVoxelPos = new IntVector(voxelPos.x, voxelPos.y, voxelPos.z);
		collisionState.cmb.CubePosToChunkPos(ref localChunkSpaceVoxelPos);
		stepX = Math.Sign(intersectRay.direction.x);
		stepY = Math.Sign(intersectRay.direction.y);
		stepZ = Math.Sign(intersectRay.direction.z);
		Vector3 val = new Vector3((float)((int)voxelPos.x + ((stepX > 0) ? 1 : 0)), (float)((int)voxelPos.y + ((stepY > 0) ? 1 : 0)), (float)((int)voxelPos.z + ((stepZ > 0) ? 1 : 0)));
		tMax = new Vector3((val.x - (intersectRay.origin.x + 0.5f)) / intersectRay.direction.x, (val.y - (intersectRay.origin.y + 0.5f)) / intersectRay.direction.y, (val.z - (intersectRay.origin.z + 0.5f)) / intersectRay.direction.z);
		if (float.IsNaN(tMax.x) || float.IsNegativeInfinity(tMax.x))
		{
			tMax.x = float.PositiveInfinity;
		}
		if (float.IsNaN(tMax.y) || float.IsNegativeInfinity(tMax.y))
		{
			tMax.y = float.PositiveInfinity;
		}
		if (float.IsNaN(tMax.z) || float.IsNegativeInfinity(tMax.z))
		{
			tMax.z = float.PositiveInfinity;
		}
		initialtMax = new Vector3(tMax.x, tMax.y, tMax.z);
		tDelta = new Vector3((float)stepX / intersectRay.direction.x, (float)stepY / intersectRay.direction.y, (float)stepZ / intersectRay.direction.z);
		if (float.IsNaN(tDelta.x))
		{
			tDelta.x = float.PositiveInfinity;
		}
		if (float.IsNaN(tDelta.y))
		{
			tDelta.y = float.PositiveInfinity;
		}
		if (float.IsNaN(tDelta.z))
		{
			tDelta.z = float.PositiveInfinity;
		}
	}

	public void InitSphereDeprecated(Vector3 localOrigin, Vector3 localDirection, RuntimePrototypeCubeModel rpcm, int scanAxis, int min, int max)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		//IL_0186: Unknown result type (might be due to invalid IL or missing references)
		//IL_018b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01de: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0203: Unknown result type (might be due to invalid IL or missing references)
		//IL_0208: Unknown result type (might be due to invalid IL or missing references)
		//IL_02dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0311: Unknown result type (might be due to invalid IL or missing references)
		//IL_0316: Unknown result type (might be due to invalid IL or missing references)
		//IL_032d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0332: Unknown result type (might be due to invalid IL or missing references)
		//IL_033c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0341: Unknown result type (might be due to invalid IL or missing references)
		debugRpcm = rpcm;
		chunkSize = CubeModelChunk.ChunkSize;
		intersectRay.origin = localOrigin;
		intersectRay.direction = localDirection;
		voxelPos = CubeMathFunctions.LocalPosToLocalIntVector(intersectRay.origin);
		voxelPos[scanAxis] = (short)Mathf.Clamp((int)voxelPos[scanAxis], min, max);
		localChunkSpaceVoxelPos = new IntVector(voxelPos.x, voxelPos.y, voxelPos.z);
		rpcm.CubePosToChunkPos(ref localChunkSpaceVoxelPos);
		stepX = Math.Sign(intersectRay.direction.x);
		stepY = Math.Sign(intersectRay.direction.y);
		stepZ = Math.Sign(intersectRay.direction.z);
		Vector3 val = new Vector3((float)((int)voxelPos.x + ((stepX > 0) ? 1 : 0)), (float)((int)voxelPos.y + ((stepY > 0) ? 1 : 0)), (float)((int)voxelPos.z + ((stepZ > 0) ? 1 : 0)));
		tMax = new Vector3((val.x - (intersectRay.origin.x + 0.5f)) / intersectRay.direction.x, (val.y - (intersectRay.origin.y + 0.5f)) / intersectRay.direction.y, (val.z - (intersectRay.origin.z + 0.5f)) / intersectRay.direction.z);
		if (float.IsNaN(tMax.x) || float.IsNegativeInfinity(tMax.x))
		{
			tMax.x = float.PositiveInfinity;
		}
		if (float.IsNaN(tMax.y) || float.IsNegativeInfinity(tMax.y))
		{
			tMax.y = float.PositiveInfinity;
		}
		if (float.IsNaN(tMax.z) || float.IsNegativeInfinity(tMax.z))
		{
			tMax.z = float.PositiveInfinity;
		}
		initialtMax = new Vector3(tMax.x, tMax.y, tMax.z);
		tDelta = new Vector3((float)stepX / intersectRay.direction.x, (float)stepY / intersectRay.direction.y, (float)stepZ / intersectRay.direction.z);
		if (float.IsNaN(tDelta.x))
		{
			tDelta.x = float.PositiveInfinity;
		}
		if (float.IsNaN(tDelta.y))
		{
			tDelta.y = float.PositiveInfinity;
		}
		if (float.IsNaN(tDelta.z))
		{
			tDelta.z = float.PositiveInfinity;
		}
	}

	public void DebugAll()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		Debug.Log((object)("intersectRay " + intersectRay));
		Debug.Log((object)("tMax " + tMax));
		Debug.Log((object)("localChunkSpaceVoxelPos " + localChunkSpaceVoxelPos));
		Debug.Log((object)("stepX " + stepX));
		Debug.Log((object)("stepY " + stepY));
		Debug.Log((object)("stepZ " + stepZ));
		Debug.Log((object)("tDelta " + tDelta));
		Debug.Log((object)("stepDir " + stepDir));
		Debug.Log((object)("initialtMax " + initialtMax));
	}

	public bool Step()
	{
		bool result = true;
		if (tMax.x < tMax.y && tMax.x < tMax.z)
		{
			localChunkSpaceVoxelPos.x += (short)stepX;
			voxelPos.x += (short)stepX;
			ref Vector3 reference = ref tMax;
			reference.x += tDelta.x;
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
			ref Vector3 reference2 = ref tMax;
			reference2.y += tDelta.y;
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
			ref Vector3 reference3 = ref tMax;
			reference3.z += tDelta.z;
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
