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
		settings = Object.Instantiate(PrefabPool.Instance.AvatarInputControllerAndroidSettings);
	}

	public void HandleDead()
	{
		jump = false;
		direction = Vector3.zero;
	}

	public void HandleInput(Vector3 moveDirection, bool jump, bool didShoot, Vector3 velocity, bool inGunMode, bool forceRotateToCamDirection)
	{
		Vector3 normalized = moveDirection.normalized;
		normalized = GetDirectionBias(moveDirection);
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

	private Vector3 GetDirectionBias(Vector3 absolutDirection)
	{
		Vector3 biasedDirection = GetBiasedDirection(absolutDirection, Vector3.forward);
		biasedDirection = GetBiasedDirection(biasedDirection, Vector3.left);
		biasedDirection = GetBiasedDirection(biasedDirection, Vector3.right);
		return GetBiasedDirection(biasedDirection, Vector3.back);
	}

	private Vector3 GetBiasedDirection(Vector3 absoluteDirection, Vector3 testDirection)
	{
		absoluteDirection.Normalize();
		float num = Vector3.Dot(testDirection, absoluteDirection);
		if (num <= 0f)
		{
			return absoluteDirection;
		}
		float t = settings.dotEvaluator.Evaluate(num);
		return Vector3.Lerp(absoluteDirection, testDirection, t).normalized;
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
