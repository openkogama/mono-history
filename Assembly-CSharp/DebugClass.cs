using MV.WorldObject;
using UnityEngine;

internal static class DebugClass
{
	public static void DrawPoint(Vector3 worldPos, float duration)
	{
		Debug.DrawLine(worldPos, worldPos + Vector3.right, Color.red, duration);
		Debug.DrawLine(worldPos, worldPos + Vector3.up, Color.green, duration);
		Debug.DrawLine(worldPos, worldPos + Vector3.forward, Color.blue, duration);
	}

	public static void DrawLineLocalToWorldSpace(Vector3 localLinePosFrom, Vector3 localLinePosTo, MVWorldObjectClient worldObject, Color color)
	{
		Vector3 start = worldObject.WorldRotation * localLinePosFrom + worldObject.WorldPosition;
		Vector3 end = worldObject.WorldRotation * localLinePosTo + worldObject.WorldPosition;
		Debug.DrawLine(start, end, color);
	}

	public static void DebugDrawPickingInfo(GameObject gameObject, Cube cube, Face face, Edge edge, IntVector iVector)
	{
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
		Debug.Log("x " + intVector.x + " y " + intVector.y + " z " + intVector.z);
	}
}
