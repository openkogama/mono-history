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
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_0179: Unknown result type (might be due to invalid IL or missing references)
		//IL_0186: Unknown result type (might be due to invalid IL or missing references)
		//IL_019b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_0203: Unknown result type (might be due to invalid IL or missing references)
		//IL_0210: Unknown result type (might be due to invalid IL or missing references)
		//IL_0226: Unknown result type (might be due to invalid IL or missing references)
		//IL_0233: Unknown result type (might be due to invalid IL or missing references)
		//IL_0246: Unknown result type (might be due to invalid IL or missing references)
		//IL_0262: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ab: Unknown result type (might be due to invalid IL or missing references)
		if (wos.Count > 0)
		{
			foreach (MVWorldObjectClient wo in wos)
			{
				Vector3[] boundsCornersWorld = wo.GetBoundsCornersWorld(BoundsContext.Default);
				Gizmos.color = Color.red;
				Vector3[] array = boundsCornersWorld;
				foreach (Vector3 val in array)
				{
					Gizmos.DrawWireSphere(val, 0.1f);
				}
				Vector2[] array2 = boundsCornersWorld.Select((Vector3 c) =>
				{
					//IL_0005: Unknown result type (might be due to invalid IL or missing references)
					//IL_0006: Unknown result type (might be due to invalid IL or missing references)
					//IL_000b: Unknown result type (might be due to invalid IL or missing references)
					return Camera.main.WorldToScreenPoint(c).xy();
				}).ToArray();
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
				clipBox.ClipMax = new Vector2((float)Screen.width, (float)Screen.height);
				ClipLine[] source = clipBox.Clip(clipLines).ToArray();
				rect = ClippedBoundingRect(((IEnumerable<ClipLine>)source).SelectMany((Func<ClipLine, IEnumerable<Vector2>>)((ClipLine line) =>
				{
					//IL_000e: Unknown result type (might be due to invalid IL or missing references)
					//IL_0013: Unknown result type (might be due to invalid IL or missing references)
					//IL_0020: Unknown result type (might be due to invalid IL or missing references)
					//IL_0025: Unknown result type (might be due to invalid IL or missing references)
					return new Vector2[2] { line.Start, line.End };
				})).ToArray());
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
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		Vector2 val = new Vector2(float.PositiveInfinity, float.PositiveInfinity);
		Vector2 val2 = new Vector2(float.NegativeInfinity, float.NegativeInfinity);
		if (points.Length == 0)
		{
			throw new ArgumentException("points");
		}
		for (int i = 0; i < points.Length; i++)
		{
			Vector2 val3 = points[i];
			Vector2 val4 = new Vector2(Mathf.Clamp(val3.x, 0f, (float)Screen.width), (float)Screen.height - Mathf.Clamp(val3.y, 0f, (float)Screen.height));
			val = new Vector2(Mathf.Min(val.x, val4.x), Mathf.Min(val.y, val4.y));
			val2 = new Vector2(Mathf.Max(val2.x, val4.x), Mathf.Max(val2.y, val4.y));
		}
		return new Rect(val.x, val.y, val2.x - val.x, val2.y - val.y);
	}
}
