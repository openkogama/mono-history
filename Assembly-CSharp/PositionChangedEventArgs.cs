using System;
using UnityEngine;

public class PositionChangedEventArgs : EventArgs
{
	public readonly Vector3 OldPos;

	public readonly Vector3 NewPos;

	public PositionChangedEventArgs(Vector3 oldPos, Vector3 newPos)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		OldPos = oldPos;
		NewPos = newPos;
	}
}
