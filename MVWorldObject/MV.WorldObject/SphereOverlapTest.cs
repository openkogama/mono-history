using System.Collections.Generic;
using UnityEngine;

namespace MV.WorldObject;

public static class SphereOverlapTest
{
	private const float SQRT_3 = 1.7320508f;

	private const float SQRT_3_DIV_2 = 0.8660254f;

	private static Matrix4x4 worldToLocal = default;

	public static CommonOverlapResult[] OverlapTest(float radius, Vector3 worldPos, CommonOverlapArg[] overlapArgs)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		List<CommonOverlapResult> list = new List<CommonOverlapResult>();
		foreach (CommonOverlapArg overlapArg in overlapArgs)
		{
			if (OverlapWo(overlapArg, radius, worldPos, out var overlapResult))
			{
				list.Add(overlapResult);
			}
		}
		return list.ToArray();
	}

	public static bool OverlapWo(CommonOverlapArg overlapArg, float worldRadius, Vector3 worldPos, out CommonOverlapResult overlapResult)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_0220: Unknown result type (might be due to invalid IL or missing references)
		MVWorldObject wo = overlapArg.wo;
		Vector3 val = default;
		worldToLocal = MatrixMath.MakeInverseTransform(wo.Position, wo.Scale, wo.Rotation);
		val = worldToLocal.MultiplyPoint(worldPos);
		float num = worldRadius / wo.Scale.x;
		float num2 = num - 0.8660254f;
		num += 0.8660254f;
		float num3 = num * num;
		float num4 = num2 * num2;
		Vector3 val2 = Vector3.one * num;
		IntVector intVector = CubeMathFunctions.LocalPosToLocalIntVector(val - val2);
		IntVector intVector2 = CubeMathFunctions.LocalPosToLocalIntVector(val + val2);
		IntVector iterationBounds = intVector2 - intVector + IntVector.One;
		overlapResult = new CommonOverlapResult(wo.Id, new List<OverlapCubeData>());
		for (int i = 0; i < iterationBounds.x; i++)
		{
			for (int j = 0; j < iterationBounds.y; j++)
			{
				bool flag = false;
				bool flag2 = false;
				for (int k = 0; k < iterationBounds.z; k++)
				{
					IntVector intVector3 = new IntVector(intVector.x + i, intVector.y + j, intVector.z + k);
					Vector3 val3 = CubeMathFunctions.LocalIntVectorToLocalPos(intVector3) - val;
					float sqrMagnitude = val3.sqrMagnitude;
					bool flag3 = false;
					bool flag4 = false;
					if (((ICubeModel)overlapArg.wo).ContainsCube(intVector3))
					{
						if (sqrMagnitude < num4)
						{
							flag3 = true;
							overlapResult.cubes.Add(new OverlapCubeData(intVector3, OverlapState.Within));
						}
						else if (sqrMagnitude < num3)
						{
							flag3 = true;
							overlapResult.cubes.Add(new OverlapCubeData(intVector3, OverlapState.OnEdge));
						}
						flag4 = true;
					}
					if (!flag && flag3)
					{
						if (flag2)
						{
							OverlapCubeData overlapCubeData = overlapResult.cubes[overlapResult.cubes.Count - 1];
							OverlapState overlapState = HandleCubeOnRadiusLimit(i, overlapCubeData.cubePos, iterationBounds, flag3, flag, (ICubeModel)overlapArg.wo, val, num3);
							overlapResult.cubes[overlapResult.cubes.Count - 1] = new OverlapCubeData(overlapCubeData.cubePos, overlapState);
						}
					}
					else if (flag && !flag3 && flag4)
					{
						OverlapCubeData overlapCubeData2 = overlapResult.cubes[overlapResult.cubes.Count - 1];
						OverlapState overlapState2 = HandleCubeOnRadiusLimit(i, overlapCubeData2.cubePos, iterationBounds, flag3, flag, (ICubeModel)overlapArg.wo, val, num3);
						overlapResult.cubes[overlapResult.cubes.Count - 1] = new OverlapCubeData(overlapCubeData2.cubePos, overlapState2);
					}
					flag = flag3;
					flag2 = flag4;
				}
			}
		}
		if (overlapResult.cubes.Count > 0)
		{
			return true;
		}
		return false;
	}

	private static OverlapState HandleCubeOnRadiusLimit(int x, IntVector pos, IntVector iterationBounds, bool cubeIsWithinRadius, bool prevCubeIsWithinRadius, ICubeModel cubeModel, Vector3 localPosition, float localRadiusSquared)
	{
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
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
		if (!cubeModel.ContainsCube(pos))
		{
			return OverlapState.OnEdge;
		}
		Vector3 val = CubeMathFunctions.LocalIntVectorToLocalPos(pos) - localPosition;
		float sqrMagnitude = val.sqrMagnitude;
		if (sqrMagnitude < localRadiusSquared)
		{
			return OverlapState.OnEdge;
		}
		if (!prevCubeIsWithinRadius && cubeIsWithinRadius && intVector.x == 1)
		{
			return OverlapState.OnEdgeLeftUp;
		}
		if (prevCubeIsWithinRadius && !cubeIsWithinRadius && intVector.x == 1)
		{
			return OverlapState.OnEdgeRightUp;
		}
		if (prevCubeIsWithinRadius && !cubeIsWithinRadius && intVector.x == -1)
		{
			return OverlapState.OnEdgeRightDown;
		}
		if (!prevCubeIsWithinRadius && cubeIsWithinRadius && intVector.x == -1)
		{
			return OverlapState.OnEdgeLeftDown;
		}
		return OverlapState.OnEdge;
	}

	static SphereOverlapTest()
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
	}
}
