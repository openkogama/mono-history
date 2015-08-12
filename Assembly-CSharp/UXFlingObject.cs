using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(UXMouseClickObject))]
public class UXFlingObject : MonoBehaviour
{
	private struct FlingData
	{
		public float Time;

		public Vector3 Position;
	}

	public delegate void OnDragMoveDelegate(Vector3 moved);

	public delegate void OnFlingDelegate(Vector3 fling);

	private const float TIME_SCALE = 50f;

	public OnDragMoveDelegate OnDragMove;

	public OnFlingDelegate OnFling;

	public int flingDataSize = 5;

	private Vector3 lastDragPos;

	private List<FlingData> flingDataSet;

	private bool mouseDown;

	private void Start()
	{
		UXMouseClickObject component = gameObject.GetComponent<UXMouseClickObject>();
		component.OnMouseDown = (UXMouseClickObject.OnMouseDownDelegate)Delegate.Combine(component.OnMouseDown, new UXMouseClickObject.OnMouseDownDelegate(HandleDragStart));
		component.OnMouseDownMove = (UXMouseClickObject.OnMouseDownMoveDelegate)Delegate.Combine(component.OnMouseDownMove, new UXMouseClickObject.OnMouseDownMoveDelegate(HandleDrag));
		component.OnMouseUp = (UXMouseClickObject.OnMouseUpDelegate)Delegate.Combine(component.OnMouseUp, new UXMouseClickObject.OnMouseUpDelegate(HandleDragEnd));
	}

	private bool HandleDragStart(UXMouseClickObject clickObject, Vector3 startPos)
	{
		if (OnDragMove != null)
		{
			OnDragMove(Vector3.zero);
		}
		flingDataSet = new List<FlingData>();
		lastDragPos = startPos;
		mouseDown = true;
		return true;
	}

	private void HandleDrag(UXMouseClickObject clickObject, Vector3 dragPos)
	{
		if (mouseDown)
		{
			if (OnDragMove != null)
			{
				OnDragMove(dragPos - lastDragPos);
			}
			lastDragPos = dragPos;
			flingDataSet.Add(new FlingData
			{
				Time = Time.time,
				Position = dragPos
			});
		}
	}

	private void HandleDragEnd(UXMouseClickObject clickObject, Vector3 stopPos)
	{
		int num = 0;
		float num2 = 0f;
		Vector3 zero = Vector3.zero;
		flingDataSet.Add(new FlingData
		{
			Time = Time.time,
			Position = stopPos
		});
		int num3 = flingDataSet.Count - 1;
		while (num3 > 1 && num3 > flingDataSet.Count - flingDataSize)
		{
			FlingData flingData = flingDataSet[num3];
			FlingData flingData2 = flingDataSet[num3 - 1];
			Vector3 vector = flingData.Position - flingData2.Position;
			zero += vector.normalized;
			num2 += vector.magnitude / ((flingData.Time - flingData2.Time) * 50f);
			num++;
			num3--;
		}
		if (num > 0)
		{
			zero /= (float)num;
			num2 /= (float)num;
			Vector3 fling = zero * num2;
			if (OnFling != null)
			{
				OnFling(fling);
			}
		}
		mouseDown = false;
	}
}
