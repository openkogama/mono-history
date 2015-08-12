using System;
using MV.Common;
using MV.WorldObject;
using UnityEngine;

public static class RemoveCubes
{
	public static class RemoveCubesWithinRadius
	{
		private enum DestructionState
		{
			CompletelyDestroyed,
			OnEdgeOfDestruction,
			NotDestroyed
		}

		private enum RemoveStyle
		{
			OnEdgeLeftUp,
			OnEdgeRightUp,
			OnEdgeRightDown,
			OnEdgeLeftDown,
			Completely
		}

		private static class FallOffValues
		{
			private struct FallOffValue
			{
				public float squaredDistance;

				public float damage;

				public override string ToString()
				{
					return $"squareDistance: {squaredDistance}. damage {damage}.";
				}
			}

			private static int maxNumFallOffValues = 20;

			private static int numFallOffValues = maxNumFallOffValues;

			private static FallOffValue[] fallOffValues = new FallOffValue[maxNumFallOffValues];

			private static int noDamageSquaredDistance;

			public static void TestFallOffValues(float radius, float centerDamage)
			{
				SetFallOffValues(radius, centerDamage);
				for (int i = 0; i < numFallOffValues; i++)
				{
					Debug.Log(fallOffValues[i]);
				}
			}

			public static void SetFallOffValues(float localRadius, float centerDamage)
			{
				float num = 1.7320508f;
				float num2 = localRadius + num;
				numFallOffValues = Mathf.CeilToInt(num2 / 1.7320508f);
				float num3 = centerDamage / (float)numFallOffValues;
				for (int i = 0; i < numFallOffValues; i++)
				{
					float num4 = 1.7320508f * (float)(i + 1);
					if (num4 > num2)
					{
						float num5 = 1.7320508f * (float)i;
						float num6 = num2 - num5;
						num4 = num5 + num6;
					}
					float damage = centerDamage - num3 * (float)i;
					fallOffValues[i].squaredDistance = num4 * num4;
					fallOffValues[i].damage = damage;
				}
			}

			public static DestructionState GetDestructionState(float squaredDistance, float toughness)
			{
				for (int num = numFallOffValues - 1; num >= 0; num--)
				{
					if (squaredDistance < fallOffValues[num].squaredDistance)
					{
						float damage = fallOffValues[num].damage;
						float num2 = 0f;
						if (num < numFallOffValues - 1)
						{
							num2 = fallOffValues[num + 1].damage;
						}
						if (toughness < num2)
						{
							return DestructionState.CompletelyDestroyed;
						}
						if (toughness < damage)
						{
							return DestructionState.OnEdgeOfDestruction;
						}
					}
				}
				return DestructionState.NotDestroyed;
			}
		}

		private static class CornerCubes
		{
			private static byte[][] cornerCubes = new byte[4][]
			{
				new byte[8] { 20, 120, 124, 124, 104, 104, 100, 0 },
				new byte[8] { 120, 120, 124, 24, 4, 104, 100, 100 },
				new byte[8] { 20, 20, 124, 24, 4, 104, 0, 0 },
				new byte[8] { 20, 120, 24, 24, 4, 4, 100, 0 }
			};

			public static byte[] GetCornerCube(int index)
			{
				return (byte[])cornerCubes[index].Clone();
			}
		}

		private const float SQRT_3_DIV_2 = 0.8660254f;

		private const float SQRT_3 = 1.7320508f;

		private static float localRadiusExtendedSquared;

		private static float localRadiusReducedSquared;

		private static IntVector iterationBounds;

		private static IntVector localMin;

		private static Vector3 localPosition;

		private static float centerDamage;

		private static DamageFallOffType damageFallOffType;

		public static void TestFallOffValues(float radius, float centerDamage)
		{
			FallOffValues.TestFallOffValues(radius, centerDamage);
		}

		public static bool HandleRemoveCubes(MVCubeModelBase cm, float radius, IntVector fineGrainedTerrainLocalPos, float centerDamage, DamageFallOffType damageFallOffType, MVCubeModelBase fineGrainedTerrainWorldObject, Func<byte, PhysicalProperties> getPhysicalProperites)
		{
			RemoveCubesWithinRadius.centerDamage = centerDamage;
			RemoveCubesWithinRadius.damageFallOffType = damageFallOffType;
			if (cm.WorldObjectType == WorldObjectType.CubeModelPrototypeTerrain)
			{
				localPosition = CubeMathFunctions.FineGrainedLocalPosToTerrainLocalPos(fineGrainedTerrainLocalPos);
				IntVector localCenterPosition = CubeMathFunctions.FromLocalPosToLocalPos(fineGrainedTerrainLocalPos, cm, fineGrainedTerrainWorldObject);
				CalculateLocalValues(cm, radius, localCenterPosition);
				if (!RemoveCubesTerrain(cm, fineGrainedTerrainWorldObject, getPhysicalProperites))
				{
				}
			}
			bool result = false;
			localPosition = CubeMathFunctions.LocalIntVectorToLocalPos(fineGrainedTerrainLocalPos);
			CalculateLocalValues(fineGrainedTerrainWorldObject, radius, fineGrainedTerrainLocalPos);
			if (RemoveCubesSmooth(fineGrainedTerrainWorldObject, getPhysicalProperites))
			{
				result = true;
			}
			return result;
		}

		private static void CalculateLocalValues(MVCubeModelBase cm, float radius, IntVector localCenterPosition)
		{
			float num = radius / cm.Scale.x;
			float num2 = Math.Max(0f, num - 0.8660254f);
			if (num2 < 0.8660254f)
			{
				num2 = 0f;
			}
			float num3 = num + 0.8660254f;
			if (damageFallOffType == DamageFallOffType.Linear)
			{
				FallOffValues.SetFallOffValues(num, centerDamage);
			}
			localRadiusExtendedSquared = num3 * num3;
			localRadiusReducedSquared = num2 * num2;
			IntVector intVector = (int)num3 * IntVector.One;
			localMin = localCenterPosition - intVector;
			iterationBounds = 2 * intVector + IntVector.One;
		}

		private static bool RemoveCubesTerrain(MVCubeModelBase wo, MVCubeModelBase fineGrainedTerrain, Func<byte, PhysicalProperties> getPhysicalProperites)
		{
			bool result = false;
			for (int i = 0; i < iterationBounds.x; i++)
			{
				for (int j = 0; j < iterationBounds.y; j++)
				{
					for (int k = 0; k < iterationBounds.z; k++)
					{
						IntVector intVector = new IntVector(localMin.x + i, localMin.y + j, localMin.z + k);
						CubeBase cubeBase = wo.GetCubeBase(intVector);
						if (!(cubeBase == null))
						{
							DestructionState destructionState = CalculateCubeDestruction(intVector, cubeBase, getPhysicalProperites);
							if (RemoveCube(destructionState, wo, fineGrainedTerrain, intVector))
							{
								result = true;
							}
						}
					}
				}
			}
			return result;
		}

		private static bool RemoveCube(DestructionState destructionState, MVCubeModelBase wo, MVCubeModelBase fineGrainedTerrain, IntVector testPosition)
		{
			if (destructionState != DestructionState.NotDestroyed)
			{
				if (destructionState == DestructionState.OnEdgeOfDestruction)
				{
					MoveCubeFromCoarseToFine.MoveCube(wo, fineGrainedTerrain, testPosition);
				}
				wo.RemoveCubeNetworkUpdate(testPosition);
				return true;
			}
			return false;
		}

		private static bool RemoveCubesSmooth(MVCubeModelBase wo, Func<byte, PhysicalProperties> getPhysicalProperites)
		{
			bool result = false;
			for (int i = 0; i < iterationBounds.x; i++)
			{
				for (int j = 0; j < iterationBounds.y; j++)
				{
					DestructionState destructionState = DestructionState.NotDestroyed;
					CubeBase cubeBase = null;
					IntVector intVector = default;
					bool flag = false;
					for (int k = 0; k < iterationBounds.z; k++)
					{
						IntVector intVector2 = new IntVector(localMin.x + i, localMin.y + j, localMin.z + k);
						CubeBase cubeBase2 = wo.GetCubeBase(intVector2);
						if (cubeBase2 == null)
						{
							flag = false;
							continue;
						}
						DestructionState destructionState2 = CalculateCubeDestruction(intVector2, cubeBase2, getPhysicalProperites);
						bool flag2 = false;
						bool flag3 = destructionState == DestructionState.OnEdgeOfDestruction && destructionState2 == DestructionState.NotDestroyed;
						if (destructionState == DestructionState.NotDestroyed && destructionState2 == DestructionState.OnEdgeOfDestruction && flag)
						{
							RemoveStyle removeStyle = HandleCubeOnRadiusLimit(wo, i, intVector2, fromDestroyToNotDestroy: false, getPhysicalProperites);
							if (removeStyle != RemoveStyle.Completely)
							{
								byte[] cornerCube = CornerCubes.GetCornerCube((int)removeStyle);
								if (cubeBase2.UnIndentedSides == 63 || CubeCornersEqual(cornerCube, cubeBase2.ByteCorners))
								{
									wo.AddCubeNetworkUpdate(intVector2, new CubeBase(cornerCube, (byte[])cubeBase2.FaceMaterials.Clone()));
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
							IntVector intVector3 = new IntVector(intVector2.x, intVector2.y, intVector2.z - 1);
							if (intVector == intVector3)
							{
								RemoveStyle removeStyle2 = HandleCubeOnRadiusLimit(wo, i, intVector, fromDestroyToNotDestroy: true, getPhysicalProperites);
								if (removeStyle2 != RemoveStyle.Completely)
								{
									byte[] cornerCube2 = CornerCubes.GetCornerCube((int)removeStyle2);
									if (cubeBase.UnIndentedSides == 63 || CubeCornersEqual(cornerCube2, cubeBase.ByteCorners))
									{
										wo.AddCubeNetworkUpdate(intVector, new CubeBase(CornerCubes.GetCornerCube((int)removeStyle2), (byte[])cubeBase.FaceMaterials.Clone()));
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
							intVector = intVector2;
							wo.RemoveCubeNetworkUpdate(intVector2);
							result = true;
						}
						flag = destructionState2 == DestructionState.NotDestroyed;
						destructionState = destructionState2;
					}
				}
			}
			return result;
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

		private static RemoveStyle HandleCubeOnRadiusLimit(MVCubeModelBase wo, int x, IntVector pos, bool fromDestroyToNotDestroy, Func<byte, PhysicalProperties> getPhysicalProperites)
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
			CubeBase cubeBase = wo.GetCubeBase(pos);
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
			float toughness = getPhysicalProperites(cubeBase.FaceMaterials[0]).toughness;
			if (toughness == 0f)
			{
				return DestructionState.NotDestroyed;
			}
			float sqrMagnitude = (CubeMathFunctions.LocalIntVectorToLocalPos(cubePos) - localPosition).sqrMagnitude;
			return damageFallOffType switch
			{
				DamageFallOffType.NoFallOff => NoFallOffDestruction(sqrMagnitude, toughness), 
				DamageFallOffType.Linear => LinearDestruction(sqrMagnitude, toughness), 
				_ => DestructionState.NotDestroyed, 
			};
		}

		private static DestructionState NoFallOffDestruction(float testDistSqr, float toughness)
		{
			if (toughness < centerDamage)
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

		private static DestructionState LinearDestruction(float testDistSqr, float toughness)
		{
			return FallOffValues.GetDestructionState(testDistSqr, toughness);
		}
	}

	public static class RemoveOneCube
	{
		public static bool HandleRemoveOneCube(IntVector fineGrainedPosition, ICubeModel terrainWorldObject, ICubeModel fineGrainedTerrainWorldObject)
		{
			if (TryRemoveCubeFromFineGrainedTerrain(fineGrainedPosition, fineGrainedTerrainWorldObject))
			{
				return true;
			}
			if (TryRemoveCubeFromTerrain(fineGrainedPosition, terrainWorldObject, fineGrainedTerrainWorldObject))
			{
				return true;
			}
			return false;
		}

		public static CubeDamageState CanRemoveCube(CubeBase cubeBase, float damage, Func<byte, PhysicalProperties> getPhysicalProperites)
		{
			if (cubeBase != null)
			{
				float toughness = getPhysicalProperites(cubeBase.FaceMaterials[0]).toughness;
				if (toughness != 0f)
				{
					if (toughness <= damage)
					{
						return CubeDamageState.Destroyed;
					}
					if (toughness > damage)
					{
						return CubeDamageState.ReceivedDamage;
					}
				}
			}
			return CubeDamageState.NoDamage;
		}

		private static bool TryRemoveCubeFromTerrain(IntVector fineGrainedPosition, ICubeModel terrainWorldObject, ICubeModel fineGrainedTerrainWorldObject)
		{
			IntVector intVector = CubeMathFunctions.FromLocalPosToLocalPos(fineGrainedPosition, terrainWorldObject, fineGrainedTerrainWorldObject);
			if (terrainWorldObject.GetCubeBase(intVector) != null)
			{
				MoveCubeFromCoarseToFine.MoveCube(terrainWorldObject, fineGrainedTerrainWorldObject, intVector);
				terrainWorldObject.RemoveCubeNetworkUpdate(intVector);
				TryRemoveCubeFromFineGrainedTerrain(fineGrainedPosition, fineGrainedTerrainWorldObject);
				return true;
			}
			return false;
		}

		private static bool TryRemoveCubeFromFineGrainedTerrain(IntVector fineGrainedPosition, ICubeModel fineGrainedTerrainWorldObject)
		{
			if (fineGrainedTerrainWorldObject.GetCubeBase(fineGrainedPosition) != null)
			{
				fineGrainedTerrainWorldObject.RemoveCubeNetworkUpdate(fineGrainedPosition);
				return true;
			}
			return false;
		}
	}
}
