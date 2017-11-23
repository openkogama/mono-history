using UnityEngine;

public class AvatarInputController : IMotorAPI, IAvatarInputController
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

	public void HandleInput(Vector3 moveDirection, bool jump, bool didShoot, Vector3 velocity, bool inGunMode, bool forceRotateToCamDirection)
	{
		direction = ToCameraDirection(moveDirection);
		if (RotateToCameraDirection(didShoot, inGunMode || forceRotateToCamDirection, velocity))
		{
			rotation = GetCameraYRotation();
		}
		else if (direction != Vector3.zero)
		{
			rotation = GetRotationMoveDirection(direction);
		}
		this.jump = jump;
	}

	private static Vector3 ToCameraDirection(Vector3 moveDirection)
	{
		if (moveDirection.magnitude > 0f)
		{
			Transform transform = Camera.main.transform;
			Vector3 vector = transform.rotation * moveDirection;
			vector.y = 0f;
			moveDirection = vector.normalized;
		}
		return moveDirection;
	}

	private static Quaternion GetCameraYRotation()
	{
		Transform transform = Camera.main.transform;
		return Quaternion.Euler(0f, transform.rotation.eulerAngles.y, 0f);
	}

	private static Quaternion GetRotationMoveDirection(Vector3 moveDirection)
	{
		return Quaternion.LookRotation(moveDirection);
	}

	private static bool RotateToCameraDirection(bool didShoot, bool inGunOrForce, Vector3 velocity)
	{
		if (didShoot && RotateToCameraDirectionBecauseOfShooting(velocity))
		{
			return true;
		}
		if (inGunOrForce && RotateToCameraDirection(velocity))
		{
			return true;
		}
		return false;
	}

	private static bool RotateToCameraDirectionBecauseOfShooting(Vector3 velocity)
	{
		if (velocity.sqrMagnitude < 0.01f)
		{
			return true;
		}
		return false;
	}

	private static bool RotateToCameraDirection(Vector3 velocity)
	{
		if (velocity.sqrMagnitude > 0.001f)
		{
			return true;
		}
		return false;
	}
}
