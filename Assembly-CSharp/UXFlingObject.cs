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
		UXMouseClickObject component = ((Component)this).gameObject.GetComponent<UXMouseClickObject>();
		component.OnMouseDown = (UXMouseClickObject.OnMouseDownDelegate)Delegate.Combine(component.OnMouseDown, new UXMouseClickObject.OnMouseDownDelegate(HandleDragStart));
		component.OnMouseDownMove = (UXMouseClickObject.OnMouseDownMoveDelegate)Delegate.Combine(component.OnMouseDownMove, new UXMouseClickObject.OnMouseDownMoveDelegate(HandleDrag));
		component.OnMouseUp = (UXMouseClickObject.OnMouseUpDelegate)Delegate.Combine(component.OnMouseUp, new UXMouseClickObject.OnMouseUpDelegate(HandleDragEnd));
	}

	private bool HandleDragStart(UXMouseClickObject clickObject, Vector3 startPos)
	{
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
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
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
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
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		int num = 0;
		float num2 = 0f;
		Vector3 val = Vector3.zero;
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
			Vector3 val2 = flingData.Position - flingData2.Position;
			val += val2.normalized;
			num2 += val2.magnitude / ((flingData.Time - flingData2.Time) * 50f);
			num++;
			num3--;
		}
		if (num > 0)
		{
			val /= (float)num;
			num2 /= (float)num;
			Vector3 fling = val * num2;
			if (OnFling != null)
			{
				OnFling(fling);
			}
		}
		mouseDown = false;
	}
}
