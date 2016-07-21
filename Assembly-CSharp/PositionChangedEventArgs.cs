using System;
using UnityEngine;

public class PositionChangedEventArgs : EventArgs
{
	public readonly Vector3 NewPos;

	public PositionChangedEventArgs(Vector3 newPos)
	{
		NewPos = newPos;
	}
}
