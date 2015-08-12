using System;
using UnityEngine;

public class MovementMap
{
	[Flags]
	private enum MovementMapFlags
	{
		None = 0,
		Left = 1,
		Forward = 2,
		Right = 4,
		Back = 8,
		Jump = 0x10,
		Run = 0x20
	}

	private MovementMapFlags frameUpdateMovementMapState;

	private MovementMapFlags movementMapState;

	public Vector3 Direction
	{
		get
		{
			Vector3 zero = Vector3.zero;
			if ((movementMapState & MovementMapFlags.Forward) != 0)
			{
				zero += Vector3.forward;
			}
			if ((movementMapState & MovementMapFlags.Back) != 0)
			{
				zero -= Vector3.forward;
			}
			if ((movementMapState & MovementMapFlags.Left) != 0)
			{
				zero -= Vector3.right;
			}
			if ((movementMapState & MovementMapFlags.Right) != 0)
			{
				zero += Vector3.right;
			}
			return zero;
		}
	}

	public bool Run => (movementMapState & MovementMapFlags.Run) != 0;

	public bool Jump => (movementMapState & MovementMapFlags.Jump) != 0;

	public void HandleInputState(bool fromFrameUpdate)
	{
		MovementMapFlags movementMapFlags = MovementMapFlags.None;
		movementMapState = MovementMapFlags.None;
		if (!MVInputWrapper.ignoreInGameInput)
		{
			if (MVInputWrapper.GetBooleanControl(KogamaControls.MoveForward))
			{
				movementMapFlags |= MovementMapFlags.Forward;
			}
			if (MVInputWrapper.GetBooleanControl(KogamaControls.MoveBackwards))
			{
				movementMapFlags |= MovementMapFlags.Back;
			}
			if (MVInputWrapper.GetBooleanControl(KogamaControls.MoveLeft))
			{
				movementMapFlags |= MovementMapFlags.Left;
			}
			if (MVInputWrapper.GetBooleanControl(KogamaControls.MoveRight))
			{
				movementMapFlags |= MovementMapFlags.Right;
			}
			if (MVInputWrapper.GetBooleanControl(KogamaControls.Run))
			{
				movementMapFlags |= MovementMapFlags.Run;
			}
			if (MVInputWrapper.GetBooleanControl(KogamaControls.Jump))
			{
				movementMapFlags |= MovementMapFlags.Jump;
			}
			if (fromFrameUpdate)
			{
				frameUpdateMovementMapState |= movementMapFlags;
				return;
			}
			movementMapState |= movementMapFlags;
			movementMapState |= frameUpdateMovementMapState;
			frameUpdateMovementMapState = MovementMapFlags.None;
		}
	}
}
