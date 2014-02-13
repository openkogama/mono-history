using System;
using MV.WorldObject;
using UnityEngine;

public class ConstraintBoxChangedEventArgs : EventArgs
{
	public readonly Vector3 Center;

	public readonly IntVector MinCorner;

	public readonly IntVector MaxCorner;

	public ConstraintBoxChangedEventArgs(Vector3 center, IntVector minCorner, IntVector maxCorner)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		Center = center;
		MinCorner = minCorner;
		MaxCorner = maxCorner;
	}
}
