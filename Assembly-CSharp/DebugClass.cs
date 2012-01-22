using MV.WorldObject;
using UnityEngine;

internal static class DebugClass
{
	public static void DrawPoint(Vector3 worldPos, float duration)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		Debug.DrawLine(worldPos, worldPos + Vector3.right, Color.red, duration);
		Debug.DrawLine(worldPos, worldPos + Vector3.up, Color.green, duration);
		Debug.DrawLine(worldPos, worldPos + Vector3.forward, Color.blue, duration);
	}

	public static void DrawLineLocalToWorldSpace(Vector3 localLinePosFrom, Vector3 localLinePosTo, MVWorldObjectClient worldObject, Color color)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = worldObject.GameObject.transform.rotation * localLinePosFrom + worldObject.GameObject.transform.position;
		Vector3 val2 = worldObject.GameObject.transform.rotation * localLinePosTo + worldObject.GameObject.transform.position;
		Debug.DrawLine(val, val2, color);
	}

	public static void DebugDrawPickingInfo(GameObject gameObject, Cube cube, Face face, Edge edge, IntVector iVector)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		Vector3[] faceVerticesWorld = Cube.GetFaceVerticesWorld(gameObject, cube, face, iVector);
		Debug.DrawLine(faceVerticesWorld[0], faceVerticesWorld[1], Color.blue);
		Debug.DrawLine(faceVerticesWorld[1], faceVerticesWorld[2], Color.blue);
		Debug.DrawLine(faceVerticesWorld[2], faceVerticesWorld[3], Color.blue);
		Debug.DrawLine(faceVerticesWorld[3], faceVerticesWorld[0], Color.blue);
		if (edge != Edge.None)
		{
			Vector3[] edgeVerticesWorld = Cube.GetEdgeVerticesWorld(gameObject, cube, face, edge, iVector);
			Debug.DrawLine(edgeVerticesWorld[0], edgeVerticesWorld[1], Color.red);
		}
	}

	public static void DebugIntVector(IntVector intVector)
	{
		Debug.Log((object)("x " + intVector.x + " y " + intVector.y + " z " + intVector.z));
	}
}
