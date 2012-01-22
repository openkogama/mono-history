using System;
using UnityEngine;

namespace MV.WorldObject;

public static class CubeMathFunctions
{
	public static IntVector LocalPosToLocalIntVector(Vector3 localPos)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = localPos + Vector3.one * 0.5f;
		val.x = (float)Math.Floor(val.x);
		val.y = (float)Math.Floor(val.y);
		val.z = (float)Math.Floor(val.z);
		return new IntVector((short)val.x, (short)val.y, (short)val.z);
	}

	public static Vector3 LocalIntVectorToLocalPos(IntVector localIntVector)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		return new Vector3((float)localIntVector.x, (float)localIntVector.y, (float)localIntVector.z);
	}
}
