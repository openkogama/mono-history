using UnityEngine;

public class JetPackMode : AvatarMode
{
	private const float moveSlowDownPoint = 0.75f;

	private readonly float maxSpeed = 1.75f;

	private readonly float speedModifier = 5f;

	private readonly float heightAdjustSpeed = 5f;

	public bool handleInput;

	private Vector3 jetPackTargetDeltaPos;

	private IgnoreInputTypes ignoreInputTypes;

	private float targetSpeed;

	private float speed;

	private float speedSmoothingTime = 10f;

	private float keyVelocity;

	private float keyAcceleration = 20f;

	private float keyDamping = 10f;

	private bool moveConstraintSet;

	private Vector3 moveConstraintCenter;

	private float moveConstraintRadius;

	private MVAvatarLocal mvAvatar;

	public float YMovementSpeedScale { get; set; }

	public float XZMovementSpeedScale { get; set; }

	public JetPackMode(MVAvatarLocal mvAvatar)
	{
		this.mvAvatar = mvAvatar;
		YMovementSpeedScale = 1f;
		XZMovementSpeedScale = 1f;
	}

	public override void Activate()
	{
		mvAvatar.SetAnimation("Idle");
		if (mvAvatar.Body == null)
		{
			Debug.LogWarning("mvAvatar.Body not present");
		}
		else
		{
			mvAvatar.Body.Visible = false;
		}
	}

	public override void FrameUpdate()
	{
		MovementMap movementMap = new MovementMap();
		movementMap.HandleInputState(fromFrameUpdate: false);
		Move(GetMovementVelocity(movementMap));
		MoveCharacter(GetElevationVelocity());
	}

	public void SetMoveConstraint(Vector3 center, float radius)
	{
		moveConstraintSet = true;
		moveConstraintCenter = center;
		moveConstraintRadius = radius;
	}

	public void RemoveMoveConstraint()
	{
		moveConstraintSet = false;
	}

	public override void FixedUpdate(MovementMap movementMap)
	{
	}

	private void MoveCharacter(Vector3 moveDelta)
	{
		if (moveConstraintSet)
		{
			Vector3 position = mvAvatar.GameObject.transform.position;
			Vector3 b = position + moveDelta;
			Vector3 normalized = (moveConstraintCenter - position).normalized;
			Vector3 normalized2 = moveDelta.normalized;
			if (!(0.5f < Vector3.Dot(normalized2, normalized)))
			{
				float num = Vector3.Distance(moveConstraintCenter, b);
				float num2 = 0.75f * moveConstraintRadius;
				if (num2 < num)
				{
					float num3 = num - num2;
					float num4 = 0.25f * moveConstraintRadius;
					float num5 = 0f;
					if (num3 < num4)
					{
						num5 = 1f - num3 / num4;
					}
					moveDelta *= num5 * num5;
				}
			}
		}
		mvAvatar.GameObject.transform.position += moveDelta;
	}

	private void Move(Vector3 velocity)
	{
		Vector3 vector = velocity * Time.deltaTime;
		MoveCharacter(vector);
		Vector3 forward = vector;
		forward.y = 0f;
		if ((double)forward.sqrMagnitude > 0.001)
		{
			mvAvatar.GameObject.transform.rotation = Quaternion.LookRotation(forward);
		}
	}

	private Vector3 GetDirection(MovementMap movementMap)
	{
		Transform transform = Camera.main.transform;
		Vector3 result = transform.rotation * movementMap.Direction;
		if (!movementMap.Run)
		{
			result.y = 0f;
		}
		result.Normalize();
		return result;
	}

	private Vector3 GetMovementVelocity(MovementMap movementMap)
	{
		Vector3 direction = GetDirection(movementMap);
		targetSpeed = direction.magnitude * maxSpeed * ((!movementMap.Run) ? 1f : speedModifier);
		speed = Mathf.Lerp(speed, targetSpeed, speedSmoothingTime * Time.deltaTime);
		return direction * speed * speedModifier * XZMovementSpeedScale;
	}

	private Vector3 GetElevationVelocity()
	{
		if ((ignoreInputTypes & IgnoreInputTypes.MouseScroll) == 0)
		{
			if (MVInputWrapper.GetBooleanControl(KogamaControls.Use))
			{
				keyVelocity += keyAcceleration * Time.deltaTime;
			}
			else if (MVInputWrapper.GetBooleanControl(KogamaControls.MoveDown))
			{
				keyVelocity -= keyAcceleration * Time.deltaTime;
			}
			else if (MVInputWrapper.GetAxis("Mouse ScrollWheel") != 0f)
			{
				if (MVInputWrapper.GetAxis("Mouse ScrollWheel") > 0f)
				{
					keyVelocity = 12f;
				}
				else
				{
					keyVelocity = -12f;
				}
			}
			else
			{
				float num = Mathf.Min(1f, keyDamping * Time.deltaTime);
				keyVelocity = (1f - num) * keyVelocity;
			}
			float num2 = keyVelocity * Time.deltaTime;
			return num2 * Vector3.up * heightAdjustSpeed * YMovementSpeedScale;
		}
		if ((ignoreInputTypes & IgnoreInputTypes.MouseScroll) != 0)
		{
			return Vector3.zero;
		}
		return Vector3.zero;
	}
}
