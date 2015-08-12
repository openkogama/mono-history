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
		Center = center;
		MinCorner = minCorner;
		MaxCorner = maxCorner;
	}
}
