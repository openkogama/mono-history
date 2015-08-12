using System;
using UnityEngine;

public class ScaleChangedEventArgs : EventArgs
{
	public readonly Vector3 OldScale;

	public readonly Vector3 NewScale;

	public ScaleChangedEventArgs(Vector3 oldScale, Vector3 newScale)
	{
		OldScale = oldScale;
		NewScale = newScale;
	}
}
