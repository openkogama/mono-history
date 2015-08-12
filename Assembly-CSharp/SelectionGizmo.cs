using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SelectionGizmo : MonoBehaviour
{
	public HashSet<MVWorldObjectClient> wos;

	private Camera mainCamera;

	private Vector3 sp;

	private Rect? rect;

	private ClipBox clipBox = new ClipBox();

	public void OnDrawGizmos()
	{
		if (wos.Count > 0)
		{
			foreach (MVWorldObjectClient wo in wos)
			{
				Vector3[] boundsCornersWorld = wo.GetBoundsCornersWorld(BoundsContext.Default);
				Gizmos.color = Color.red;
				Vector3[] array = boundsCornersWorld;
				foreach (Vector3 center in array)
				{
					Gizmos.DrawWireSphere(center, 0.1f);
				}
				Vector2[] array2 = boundsCornersWorld.Select((Vector3 c) => Camera.main.WorldToScreenPoint(c).xy()).ToArray();
				ClipLine[] clipLines = new ClipLine[12]
				{
					new ClipLine(array2[0], array2[1]),
					new ClipLine(array2[1], array2[2]),
					new ClipLine(array2[2], array2[3]),
					new ClipLine(array2[3], array2[0]),
					new ClipLine(array2[0], array2[7]),
					new ClipLine(array2[1], array2[6]),
					new ClipLine(array2[2], array2[5]),
					new ClipLine(array2[3], array2[4]),
					new ClipLine(array2[4], array2[5]),
					new ClipLine(array2[5], array2[6]),
					new ClipLine(array2[6], array2[7]),
					new ClipLine(array2[7], array2[4])
				};
				clipBox.ClipMin = Vector2.zero;
				clipBox.ClipMax = new Vector2(Screen.width, Screen.height);
				ClipLine[] source = clipBox.Clip(clipLines).ToArray();
				rect = ClippedBoundingRect(source.SelectMany((ClipLine line) => new Vector2[2] { line.Start, line.End }).ToArray());
			}
			return;
		}
		rect = null;
	}

	public void OnGUI()
	{
		if (!rect.HasValue)
		{
		}
	}

	public Rect ClippedBoundingRect(Vector2[] points)
	{
		Vector2 vector = new Vector2(float.PositiveInfinity, float.PositiveInfinity);
		Vector2 vector2 = new Vector2(float.NegativeInfinity, float.NegativeInfinity);
		if (points.Length == 0)
		{
			throw new ArgumentException("points");
		}
		for (int i = 0; i < points.Length; i++)
		{
			Vector2 vector3 = points[i];
			Vector2 vector4 = new Vector2(Mathf.Clamp(vector3.x, 0f, Screen.width), (float)Screen.height - Mathf.Clamp(vector3.y, 0f, Screen.height));
			vector = new Vector2(Mathf.Min(vector.x, vector4.x), Mathf.Min(vector.y, vector4.y));
			vector2 = new Vector2(Mathf.Max(vector2.x, vector4.x), Mathf.Max(vector2.y, vector4.y));
		}
		return new Rect(vector.x, vector.y, vector2.x - vector.x, vector2.y - vector.y);
	}
}
