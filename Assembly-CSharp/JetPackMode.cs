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
		base.Activate();
		mvAvatar.SetAnimation("Idle");
		if (mvAvatar.Body == null)
		{
			Debug.LogWarning((object)"mvAvatar.Body not present");
		}
		else
		{
			mvAvatar.Body.Visible = false;
		}
	}

	public override void FixedUpdate(MovementMap movementMap)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		Move(GetMovementVelocity(movementMap));
	}

	public override void FrameUpdate(MovementMap movementMap)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		MoveCharacter(GetElevationVelocity());
	}

	public void SetMoveConstraint(Vector3 center, float radius)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		moveConstraintSet = true;
		moveConstraintCenter = center;
		moveConstraintRadius = radius;
	}

	public void RemoveMoveConstraint()
	{
		moveConstraintSet = false;
	}

	private void MoveCharacter(Vector3 moveDelta)
	{
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		if (moveConstraintSet)
		{
			Vector3 position = mvAvatar.GameObject.transform.position;
			Vector3 val = position + moveDelta;
			Vector3 val2 = moveConstraintCenter - position;
			Vector3 normalized = val2.normalized;
			Vector3 normalized2 = moveDelta.normalized;
			if (!(0.5f < Vector3.Dot(normalized2, normalized)))
			{
				float num = Vector3.Distance(moveConstraintCenter, val);
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
		Transform transform = mvAvatar.GameObject.transform;
		transform.position += moveDelta;
	}

	private void Move(Vector3 velocity)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = velocity * Time.deltaTime;
		MoveCharacter(val);
		Vector3 val2 = val;
		val2.y = 0f;
		if ((double)val2.sqrMagnitude > 0.001)
		{
			mvAvatar.GameObject.transform.rotation = Quaternion.LookRotation(val2);
		}
	}

	private Vector3 GetDirection(MovementMap movementMap)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		Transform transform = ((Component)Camera.main).transform;
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
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		Vector3 direction = GetDirection(movementMap);
		targetSpeed = direction.magnitude * maxSpeed * ((!movementMap.Run) ? 1f : speedModifier);
		speed = Mathf.Lerp(speed, targetSpeed, speedSmoothingTime * Time.deltaTime);
		return direction * speed * speedModifier * XZMovementSpeedScale;
	}

	private Vector3 GetElevationVelocity()
	{
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		if ((ignoreInputTypes & IgnoreInputTypes.MouseScroll) == 0)
		{
			if (MVInputWrapper.GetKey((KeyCode)101))
			{
				keyVelocity += keyAcceleration * Time.deltaTime;
			}
			else if (MVInputWrapper.GetKey((KeyCode)99))
			{
				keyVelocity -= keyAcceleration * Time.deltaTime;
			}
			else if (MVInputWrapper.GetAxis("Mouse ScrollWheel") != 0f)
			{
				keyVelocity = 60f * MVInputWrapper.GetAxis("Mouse ScrollWheel");
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
