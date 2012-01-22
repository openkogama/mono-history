using System.Collections;
using UnityEngine;

namespace MV.WorldObject;

public class PathHelper
{
	public static Vector3 GetPositionByTime(Vector3 position, Vector3[] waypoints, Hashtable data, ref float pathTime, ref int oldWayPoint, ref int newWayPoint, ref bool shouldStop)
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		return (MoverPattern)data["pattern"] switch
		{
			MoverPattern.Loop => GetPositionByTimeLoop(position, waypoints, data, ref pathTime, ref oldWayPoint, ref newWayPoint, ref shouldStop), 
			MoverPattern.PingPong => GetPositionByTimePingPong(position, waypoints, data, ref pathTime, ref oldWayPoint, ref newWayPoint, ref shouldStop), 
			_ => Vector3.zero, 
		};
	}

	public static float CalcRoundTripTime(Vector3[] waypoints, MoverPattern pattern, float moveSpeed)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		float num = 0f;
		for (int i = 1; i < waypoints.Length; i++)
		{
			num += Vector3.Distance(waypoints[i], waypoints[i - 1]);
		}
		switch (pattern)
		{
		case MoverPattern.Loop:
			num += Vector3.Distance(waypoints[0], waypoints[^1]);
			break;
		case MoverPattern.PingPong:
		{
			for (int num2 = waypoints.Length - 1; num2 > 0; num2--)
			{
				num += Vector3.Distance(waypoints[num2], waypoints[num2 - 1]);
			}
			break;
		}
		default:
			return 0f;
		}
		return num / moveSpeed;
	}

	private static Vector3 GetPositionByTimeLoop(Vector3 position, Vector3[] waypoints, Hashtable data, ref float pathTime, ref int oldWayPoint, ref int newWayPoint, ref bool shouldStop)
	{
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0172: Unknown result type (might be due to invalid IL or missing references)
		//IL_0180: Unknown result type (might be due to invalid IL or missing references)
		//IL_0187: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		float num = (float)data["speed"];
		bool flag = (bool)data["once"];
		float num2 = pathTime * num;
		float num3 = 0f;
		int num4 = oldWayPoint;
		oldWayPoint = -1;
		newWayPoint = -1;
		bool flag2 = false;
		shouldStop = false;
		Vector3 val = Vector3.zero;
		for (int i = 1; i < waypoints.Length; i++)
		{
			num3 += Vector3.Distance(waypoints[i], waypoints[i - 1]);
			if (num3 > num2)
			{
				oldWayPoint = i - 1;
				newWayPoint = i;
				flag2 = true;
				break;
			}
		}
		if (flag && flag2 && oldWayPoint == 0 && num4 == waypoints.Length - 1)
		{
			shouldStop = true;
			val = waypoints[0];
		}
		if (!flag2)
		{
			oldWayPoint = waypoints.Length - 1;
			newWayPoint = 0;
			num3 += Vector3.Distance(waypoints[newWayPoint], waypoints[oldWayPoint]);
		}
		float num5 = num3 - num2;
		float num6 = Vector3.Distance(waypoints[newWayPoint], waypoints[oldWayPoint]);
		float num7 = (num6 - num5) / num6;
		if (shouldStop)
		{
			float num8 = Vector3.Distance(position, Vector3.Lerp(waypoints[oldWayPoint], waypoints[newWayPoint], num7));
			float num9 = Vector3.Distance(position, val);
			pathTime -= (num8 - num9) / num;
			return val;
		}
		return Vector3.Lerp(waypoints[oldWayPoint], waypoints[newWayPoint], num7);
	}

	private static Vector3 GetPositionByTimePingPong(Vector3 position, Vector3[] waypoints, Hashtable data, ref float pathTime, ref int oldWayPoint, ref int newWayPoint, ref bool shouldStop)
	{
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Unknown result type (might be due to invalid IL or missing references)
		//IL_017f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0186: Unknown result type (might be due to invalid IL or missing references)
		//IL_0192: Unknown result type (might be due to invalid IL or missing references)
		//IL_0193: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		float num = (float)data["speed"];
		bool flag = (bool)data["once"];
		float num2 = pathTime * num;
		float num3 = 0f;
		int num4 = oldWayPoint;
		oldWayPoint = -1;
		newWayPoint = -1;
		bool flag2 = false;
		shouldStop = false;
		Vector3 val = Vector3.zero;
		for (int i = 1; i < waypoints.Length; i++)
		{
			num3 += Vector3.Distance(waypoints[i], waypoints[i - 1]);
			if (num3 > num2)
			{
				oldWayPoint = i - 1;
				newWayPoint = i;
				flag2 = true;
				break;
			}
		}
		if (flag && flag2 && oldWayPoint == 0 && num4 == 1)
		{
			shouldStop = true;
			val = waypoints[0];
		}
		if (!flag2)
		{
			for (int num5 = waypoints.Length - 1; num5 > 0; num5--)
			{
				num3 += Vector3.Distance(waypoints[num5 - 1], waypoints[num5]);
				if (num3 > num2)
				{
					oldWayPoint = num5;
					newWayPoint = num5 - 1;
					break;
				}
			}
		}
		if (flag && oldWayPoint == waypoints.Length - 1 && num4 == waypoints.Length - 2)
		{
			shouldStop = true;
			val = waypoints[^1];
		}
		float num6 = num3 - num2;
		float num7 = Vector3.Distance(waypoints[newWayPoint], waypoints[oldWayPoint]);
		float num8 = (num7 - num6) / num7;
		if (shouldStop)
		{
			float num9 = Vector3.Distance(position, Vector3.Lerp(waypoints[oldWayPoint], waypoints[newWayPoint], num8));
			float num10 = Vector3.Distance(position, val);
			pathTime -= (num9 - num10) / num;
			return val;
		}
		return Vector3.Lerp(waypoints[oldWayPoint], waypoints[newWayPoint], num8);
	}
}
