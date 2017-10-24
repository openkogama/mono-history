using UnityEngine;

public class AvatarInputControllerAndroid2DPlayMode : IAvatarInputController, IMotorAPI
{
	private Vector3 direction = Vector3.zero;

	private Quaternion rotation = Quaternion.identity;

	private bool jump;

	public Vector3 Direction => direction;

	public Quaternion Rotation
	{
		get
		{
			return rotation;
		}
		set
		{
			rotation = value;
		}
	}

	public bool Jump => jump;

	public void HandleDead()
	{
		jump = false;
		direction = Vector3.zero;
	}

	public void HandleInput(Vector3 moveDirection, bool jump, bool didShoot, Vector3 velocity, bool inGunMode, bool forceRotateToCamDirection)
	{
		if (moveDirection != Vector3.zero)
		{
			direction = moveDirection;
			rotation = GetRotationMoveDirection(moveDirection.normalized);
		}
		else
		{
			direction = moveDirection;
		}
		this.jump = jump;
	}

	private static Quaternion GetRotationMoveDirection(Vector3 moveDirection)
	{
		return Quaternion.LookRotation(moveDirection);
	}
}
