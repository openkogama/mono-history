using UnityEngine;

public class AvatarInputControllerAndroid : IMotorAPI, IAvatarInputController
{
	private Vector3 direction = Vector3.zero;

	private Quaternion rotation = Quaternion.identity;

	private bool jump;

	private AvatarInputControllerAndroidSettings settings;

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

	public AvatarInputControllerAndroid()
	{
		settings = Resources.Load("Prefabs/Avatar/AvatarInputControllerAndroidSettings", typeof(AvatarInputControllerAndroidSettings)) as AvatarInputControllerAndroidSettings;
	}

	public void HandleDead()
	{
		jump = false;
		direction = Vector3.zero;
	}

	public void HandleInput(Vector3 moveDirection, bool jump, bool didShoot, Vector3 velocity, bool inGunMode, bool forceRotateToCamDirection)
	{
		Vector3 normalized = moveDirection.normalized;
		normalized = DirectionBiasedForward(moveDirection);
		Vector3 vector = ToCameraDirection(normalized);
		if (inGunMode || forceRotateToCamDirection)
		{
			rotation = GetCameraYRotation();
		}
		else if (vector != Vector3.zero)
		{
			rotation = GetRotationMoveDirection(vector);
		}
		direction = vector * moveDirection.magnitude;
		if (direction.magnitude > 1f)
		{
			direction.Normalize();
		}
		this.jump = jump;
	}

	private Vector3 DirectionBiasedForward(Vector3 absolutDirection)
	{
		Vector3 normalized = absolutDirection.normalized;
		float num = Vector3.Dot(Vector3.forward, normalized);
		if (num > 0f)
		{
			normalized.x *= settings.directionBiasForward.Evaluate(num);
			return normalized.normalized;
		}
		return absolutDirection;
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
}
