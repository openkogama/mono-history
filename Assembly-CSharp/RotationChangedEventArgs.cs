using System;
using UnityEngine;

public class RotationChangedEventArgs : EventArgs
{
	public readonly Quaternion OldRotation;

	public readonly Quaternion NewRotation;

	public RotationChangedEventArgs(Quaternion oldRotation, Quaternion newRotation)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		OldRotation = oldRotation;
		NewRotation = newRotation;
	}
}
