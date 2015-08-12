using System;
using UnityEngine;

public class PositionChangedEventArgs : EventArgs
{
	public readonly Vector3 OldPos;

	public readonly Vector3 NewPos;

	public PositionChangedEventArgs(Vector3 oldPos, Vector3 newPos)
	{
		OldPos = oldPos;
		NewPos = newPos;
	}
}
