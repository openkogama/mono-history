using System;
using UnityEngine;

public class RotationChangedEventArgs : EventArgs
{
	public readonly Quaternion OldRotation;

	public readonly Quaternion NewRotation;

	public RotationChangedEventArgs(Quaternion newRotation)
	{
		NewRotation = newRotation;
	}
}
