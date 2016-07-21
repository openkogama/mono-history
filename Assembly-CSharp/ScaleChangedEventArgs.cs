using System;
using UnityEngine;

public class ScaleChangedEventArgs : EventArgs
{
	public readonly Vector3 NewScale;

	public ScaleChangedEventArgs(Vector3 newScale)
	{
		NewScale = newScale;
	}
}
