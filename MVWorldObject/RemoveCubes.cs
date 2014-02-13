using System;
using System.Collections.Generic;
using MV.WorldObject;
using UnityEngine;

public static class RemoveCubes
{
	private enum RemoveStyle
	{
		OnEdgeLeftUp,
		OnEdgeRightUp,
		OnEdgeRightDown,
		OnEdgeLeftDown,
		Completely
	}

	private enum DestructionState
	{
		CompletelyDestroyed,
		OnEdgeOfDestruction,
		NotDestroyed
	}

	private const float SQRT_3_DIV_2 = 0.8660254f;

	private static Matrix4x4 worldToLocal = default;

	private static ICubeModel cm;

	private static float localRadiusExtendedSquared;

	private static float localRadiusReducedSquared;

	private static IntVector iterationBounds;

	private static Vector3 localPosition;

	private static float centerDamage;

	private static DamageFallOffType damageFallOffType;

	private static float radius;

	private static Vector3 worldPos;

	public static void HandleRemoveCubes(List<CommonOverlapArg> overlapArgs, float radius, Vector3 worldPos, float centerDamage, DamageFallOffType damageFallOffType, MVWorldObject fineGrainedTerrainWorldObject, Func<byte, PhysicalProperties> getPhysicalProperites)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		RemoveCubes.radius = radius;
		RemoveCubes.worldPos = worldPos;
		RemoveCubes.centerDamage = centerDamage;
		RemoveCubes.damageFallOffType = damageFallOffType;
		bool flag = false;
		foreach (CommonOverlapArg overlapArg in overlapArgs)
		{
			if (overlapArg.wo.WorldObjectType == WorldObjectType.CubeModelPrototypeTerrain)
			{
				flag = true;
			}
			if (overlapArg.wo.WorldObjectType == WorldObjectType.CubeModelTerrainFineGrained)
			{
				flag = true;
			}
			else
			{
				RemoveCubesWo(overlapArg.wo, (ICubeModel)fineGrainedTerrainWorldObject, getPhysicalProperites);
			}
		}
		if (flag)
		{
			RemoveCubesWo(new CommonOverlapArg(fineGrainedTerrainWorldObject).wo, (ICubeModel)fineGrainedTerrainWorldObject, getPhysicalProperites);
		}
	}

	private static void RemoveCubesWo(MVWorldObject wo, ICubeModel fineGrainedTerrain, Func<byte, PhysicalProperties> getPhysicalProperites)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		cm = (ICubeModel)wo;
		worldToLocal = MatrixMath.MakeInverseTransform(wo.Position, wo.Scale, wo.Rotation);
		localPosition = worldToLocal.MultiplyPoint(worldPos);
		float num = radius / wo.Scale.x;
		float num2 = Math.Max(0f, num - 0.8660254f);
		if (num2 < 0.8660254f)
		{
			num2 = 0f;
		}
		float num3 = num + 0.8660254f;
		localRadiusExtendedSquared = num3 * num3;
		localRadiusReducedSquared = num2 * num2;
		Vector3 val = Vector3.one * num3;
		IntVector intVector = CubeMathFunctions.LocalPosToLocalIntVector(localPosition - val);
		IntVector intVector2 = CubeMathFunctions.LocalPosToLocalIntVector(localPosition + val);
		iterationBounds = intVector2 - intVector + IntVector.One;
		for (int i = 0; i < iterationBounds.x; i++)
		{
			for (int j = 0; j < iterationBounds.y; j++)
			{
				DestructionState destructionState = DestructionState.NotDestroyed;
				CubeBase cubeBase = null;
				IntVector intVector3 = default;
				bool flag = false;
				for (int k = 0; k < iterationBounds.z; k++)
				{
					IntVector intVector4 = new IntVector(intVector.x + i, intVector.y + j, intVector.z + k);
					CubeBase cubeBase2 = cm.GetCubeBase(intVector4);
					if (cubeBase2 == null)
					{
						flag = false;
						continue;
					}
					DestructionState destructionState2 = CalculateCubeDestruction(intVector4, cubeBase2, getPhysicalProperites);
					if (wo.WorldObjectType == WorldObjectType.CubeModelPrototypeTerrain)
					{
						if (destructionState2 != DestructionState.NotDestroyed)
						{
							if (destructionState2 == DestructionState.OnEdgeOfDestruction)
							{
								MoveCubeFromCoarseToFine.MoveCube(cm, fineGrainedTerrain, intVector4);
							}
							cm.RemoveCubeNetworkUpdate(intVector4);
						}
						continue;
					}
					bool flag2 = false;
					bool flag3 = destructionState == DestructionState.OnEdgeOfDestruction && destructionState2 == DestructionState.NotDestroyed;
					if (destructionState == DestructionState.NotDestroyed && destructionState2 == DestructionState.OnEdgeOfDestruction && flag)
					{
						RemoveStyle removeStyle = HandleCubeOnRadiusLimit(i, intVector4, fromDestroyToNotDestroy: false, getPhysicalProperites);
						if (removeStyle != RemoveStyle.Completely)
						{
							byte[] cornerCube = CubeBase.GetCornerCube((int)removeStyle);
							if (cubeBase2.UnIndentedSides == 63 || CubeCornersEqual(cornerCube, cubeBase2.ByteCorners))
							{
								cm.AddCubeNetworkUpdate(intVector4, new CubeBase(cornerCube, (byte[])cubeBase2.FaceMaterials.Clone()));
							}
							else
							{
								flag2 = true;
							}
						}
						else
						{
							flag2 = true;
						}
					}
					else if (flag3)
					{
						IntVector intVector5 = new IntVector(intVector4.x, intVector4.y, intVector4.z - 1);
						if (intVector3 == intVector5)
						{
							RemoveStyle removeStyle2 = HandleCubeOnRadiusLimit(i, intVector3, fromDestroyToNotDestroy: true, getPhysicalProperites);
							if (removeStyle2 != RemoveStyle.Completely)
							{
								byte[] cornerCube2 = CubeBase.GetCornerCube((int)removeStyle2);
								if (cubeBase.UnIndentedSides == 63 || CubeCornersEqual(cornerCube2, cubeBase.ByteCorners))
								{
									cm.AddCubeNetworkUpdate(intVector3, new CubeBase(CubeBase.GetCornerCube((int)removeStyle2), (byte[])cubeBase.FaceMaterials.Clone()));
								}
							}
						}
					}
					else if (destructionState2 != DestructionState.NotDestroyed)
					{
						flag2 = true;
					}
					if (flag2)
					{
						cubeBase = cubeBase2;
						intVector3 = intVector4;
						cm.RemoveCubeNetworkUpdate(intVector4);
					}
					flag = destructionState2 == DestructionState.NotDestroyed;
					destructionState = destructionState2;
				}
			}
		}
	}

	private static bool CubeCornersEqual(byte[] corners0, byte[] corners1)
	{
		for (int i = 0; i < 8; i++)
		{
			if (corners0[i] != corners1[i])
			{
				return false;
			}
		}
		return true;
	}

	private static RemoveStyle HandleCubeOnRadiusLimit(int x, IntVector pos, bool fromDestroyToNotDestroy, Func<byte, PhysicalProperties> getPhysicalProperites)
	{
		IntVector intVector = new IntVector(0, 0, 0);
		if (x < iterationBounds.x / 2)
		{
			intVector.x = -1;
		}
		else
		{
			intVector.x = 1;
		}
		pos += intVector;
		CubeBase cubeBase = cm.GetCubeBase(pos);
		if (cubeBase == null)
		{
			return RemoveStyle.Completely;
		}
		if (CalculateCubeDestruction(pos, cubeBase, getPhysicalProperites) != DestructionState.NotDestroyed)
		{
			return RemoveStyle.Completely;
		}
		if (!fromDestroyToNotDestroy && intVector.x == 1)
		{
			return RemoveStyle.OnEdgeLeftUp;
		}
		if (fromDestroyToNotDestroy && intVector.x == 1)
		{
			return RemoveStyle.OnEdgeRightUp;
		}
		if (fromDestroyToNotDestroy && intVector.x == -1)
		{
			return RemoveStyle.OnEdgeRightDown;
		}
		if (!fromDestroyToNotDestroy && intVector.x == -1)
		{
			return RemoveStyle.OnEdgeLeftDown;
		}
		return RemoveStyle.Completely;
	}

	private static DestructionState CalculateCubeDestruction(IntVector cubePos, CubeBase cubeBase, Func<byte, PhysicalProperties> getPhysicalProperites)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = CubeMathFunctions.LocalIntVectorToLocalPos(cubePos) - localPosition;
		float sqrMagnitude = val.sqrMagnitude;
		return damageFallOffType switch
		{
			DamageFallOffType.NoFallOff => NoFallOffDestruction(sqrMagnitude, cubeBase, getPhysicalProperites), 
			DamageFallOffType.Linear => LinearDestruction(sqrMagnitude, cubeBase, getPhysicalProperites), 
			_ => DestructionState.NotDestroyed, 
		};
	}

	private static DestructionState NoFallOffDestruction(float testDistSqr, CubeBase cubeBase, Func<byte, PhysicalProperties> getPhysicalProperites)
	{
		if (getPhysicalProperites(cubeBase.FaceMaterials[0]).toughness < centerDamage)
		{
			if (testDistSqr < localRadiusReducedSquared)
			{
				return DestructionState.CompletelyDestroyed;
			}
			if (testDistSqr < localRadiusExtendedSquared)
			{
				return DestructionState.OnEdgeOfDestruction;
			}
		}
		return DestructionState.NotDestroyed;
	}

	private static DestructionState LinearDestruction(float testDistSqr, CubeBase cubeBase, Func<byte, PhysicalProperties> getPhysicalProperites)
	{
		float num = (localRadiusExtendedSquared - localRadiusReducedSquared) / 2f;
		float num2 = (1f - (testDistSqr - num) / localRadiusExtendedSquared) * centerDamage;
		float num3 = (1f - (testDistSqr + num) / localRadiusExtendedSquared) * centerDamage;
		float toughness = getPhysicalProperites(cubeBase.FaceMaterials[0]).toughness;
		if (num3 > toughness)
		{
			if (localRadiusReducedSquared == 0f)
			{
				return DestructionState.OnEdgeOfDestruction;
			}
			return DestructionState.CompletelyDestroyed;
		}
		if (num2 > toughness && num3 < toughness)
		{
			return DestructionState.OnEdgeOfDestruction;
		}
		return DestructionState.NotDestroyed;
	}

	static RemoveCubes()
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
	}
}
