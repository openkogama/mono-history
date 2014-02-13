using System;
using UnityEngine;

public class ScaleChangedEventArgs : EventArgs
{
	public readonly Vector3 OldScale;

	public readonly Vector3 NewScale;

	public ScaleChangedEventArgs(Vector3 oldScale, Vector3 newScale)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		OldScale = oldScale;
		NewScale = newScale;
	}
}
