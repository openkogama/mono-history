using System.Collections.Generic;
using UnityEngine;

namespace MV.WorldObject;

public class PathHelper
{
	public static Vector3 GetPositionByTime(Vector3 position, Vector3[] waypoints, Dictionary<object, object> data, ref float pathTime, ref int oldWayPoint, ref int newWayPoint, ref bool shouldStop)
	{
		return (MoverPattern)data["pattern"] switch
		{
			MoverPattern.Loop => GetPositionByTimeLoop(position, waypoints, data, ref pathTime, ref oldWayPoint, ref newWayPoint, ref shouldStop), 
			MoverPattern.PingPong => GetPositionByTimePingPong(position, waypoints, data, ref pathTime, ref oldWayPoint, ref newWayPoint, ref shouldStop), 
			_ => Vector3.zero, 
		};
	}

	public static float CalcRoundTripTime(Vector3[] waypoints, MoverPattern pattern, float moveSpeed)
	{
		float num = 0f;
		for (int i = 1; i < waypoints.Length; i++)
		{
			num += Vector3.Distance(waypoints[i], waypoints[i - 1]);
		}
		switch (pattern)
		{
		case MoverPattern.Loop:
			num += Vector3.Distance(waypoints[0], waypoints[waypoints.Length - 1]);
			break;
		case MoverPattern.PingPong:
		{
			for (int j = waypoints.Length - 1; j > 0; j--)
			{
				num += Vector3.Distance(waypoints[j], waypoints[j - 1]);
			}
			break;
		}
		default:
			return 0f;
		}
		return num / moveSpeed;
	}

	private static Vector3 GetPositionByTimeLoop(Vector3 position, Vector3[] waypoints, Dictionary<object, object> data, ref float pathTime, ref int oldWayPoint, ref int newWayPoint, ref bool shouldStop)
	{
		float num = (float)data["speed"];
		bool flag = (bool)data["once"];
		float num2 = pathTime * num;
		float num3 = 0f;
		int num4 = oldWayPoint;
		oldWayPoint = -1;
		newWayPoint = -1;
		bool flag2 = false;
		shouldStop = false;
		Vector3 vector = Vector3.zero;
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
		if ((flag & flag2) && oldWayPoint == 0 && num4 == waypoints.Length - 1)
		{
			shouldStop = true;
			vector = waypoints[0];
		}
		if (!flag2)
		{
			oldWayPoint = waypoints.Length - 1;
			newWayPoint = 0;
			num3 += Vector3.Distance(waypoints[newWayPoint], waypoints[oldWayPoint]);
		}
		float num5 = num3 - num2;
		float num6 = Vector3.Distance(waypoints[newWayPoint], waypoints[oldWayPoint]);
		float t = (num6 - num5) / num6;
		if (shouldStop)
		{
			float num7 = Vector3.Distance(position, Vector3.Lerp(waypoints[oldWayPoint], waypoints[newWayPoint], t));
			float num8 = Vector3.Distance(position, vector);
			pathTime -= (num7 - num8) / num;
			return vector;
		}
		return Vector3.Lerp(waypoints[oldWayPoint], waypoints[newWayPoint], t);
	}

	private static Vector3 GetPositionByTimePingPong(Vector3 position, Vector3[] waypoints, Dictionary<object, object> data, ref float pathTime, ref int oldWayPoint, ref int newWayPoint, ref bool shouldStop)
	{
		float num = (float)data["speed"];
		bool flag = (bool)data["once"];
		float num2 = pathTime * num;
		float num3 = 0f;
		int num4 = oldWayPoint;
		oldWayPoint = -1;
		newWayPoint = -1;
		bool flag2 = false;
		shouldStop = false;
		Vector3 vector = Vector3.zero;
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
		if ((flag & flag2) && oldWayPoint == 0 && num4 == 1)
		{
			shouldStop = true;
			vector = waypoints[0];
		}
		if (!flag2)
		{
			for (int j = waypoints.Length - 1; j > 0; j--)
			{
				num3 += Vector3.Distance(waypoints[j - 1], waypoints[j]);
				if (num3 > num2)
				{
					oldWayPoint = j;
					newWayPoint = j - 1;
					break;
				}
			}
		}
		if (flag && oldWayPoint == waypoints.Length - 1 && num4 == waypoints.Length - 2)
		{
			shouldStop = true;
			vector = waypoints[waypoints.Length - 1];
		}
		float num5 = num3 - num2;
		float num6 = Vector3.Distance(waypoints[newWayPoint], waypoints[oldWayPoint]);
		float t = (num6 - num5) / num6;
		if (shouldStop)
		{
			float num7 = Vector3.Distance(position, Vector3.Lerp(waypoints[oldWayPoint], waypoints[newWayPoint], t));
			float num8 = Vector3.Distance(position, vector);
			pathTime -= (num7 - num8) / num;
			return vector;
		}
		return Vector3.Lerp(waypoints[oldWayPoint], waypoints[newWayPoint], t);
	}
}
