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
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
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
		IntVector intVector3 = intVector2 - intVector + IntVector.One;
		overlapResult = new CommonOverlapResult(wo.Id, new List<OverlapCubeData>());
		for (int i = 0; i < intVector3.x; i++)
		{
			for (int j = 0; j < intVector3.y; j++)
			{
				for (int k = 0; k < intVector3.z; k++)
				{
					IntVector intVector4 = new IntVector((short)(intVector.x + i), (short)(intVector.y + j), (short)(intVector.z + k));
					if (((ICubeModel)overlapArg.wo).ContainsCube(intVector4))
					{
						Vector3 val3 = CubeMathFunctions.LocalIntVectorToLocalPos(intVector4) - val;
						float sqrMagnitude = val3.sqrMagnitude;
						if (sqrMagnitude < num4)
						{
							overlapResult.cubes.Add(new OverlapCubeData(intVector4, OverlapState.Within));
						}
						else if (sqrMagnitude < num3)
						{
							overlapResult.cubes.Add(new OverlapCubeData(intVector4, OverlapState.OnEdge));
						}
					}
				}
			}
		}
		if (overlapResult.cubes.Count > 0)
		{
			return true;
		}
		return false;
	}

	static SphereOverlapTest()
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
	}
}
