using UnityEngine;

public class JetPackMode : AvatarMode
{
	private float maxSpeed = 4f;

	private float speedModifier = 2f;

	private float heightAdjustSpeed = 5f;

	private float horizontalSpeed = 7f;

	private MvCharacterController characterController;

	public bool handleInput;

	private float targetSpeed;

	private float speed;

	private float speedSmoothingTime = 10f;

	private Vector3 jetPackTargetDeltaPos;

	private IgnoreInputTypes ignoreInputTypes;

	private float targetY;

	private bool targetYInitialized;

	private float keyVelocity;

	private float keyAcceleration = 50f;

	private float keyDamping = 20f;

	public JetPackMode(MvCharacterController characterController, AvatarController avatar)
		: base(avatar)
	{
		this.characterController = characterController;
	}

	public override void Activate()
	{
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		base.Activate();
		avatarController.MVAvatar.Animation.Value = "Jump";
		avatarAnimation.SetVisible(visible: false);
		targetY = ((Component)avatarController).gameObject.transform.localPosition.y;
	}

	public override void FixedUpdate()
	{
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		if (!targetYInitialized)
		{
			targetY = ((Component)avatarController).gameObject.transform.localPosition.y;
			targetYInitialized = true;
		}
		Move(GetMovementVelocity() + GetElevationVelocity());
	}

	private void Move(Vector3 velocity)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = velocity * Time.deltaTime;
		Vector3 position = ((Component)characterController).transform.position;
		if (true)
		{
			Transform transform = ((Component)characterController).transform;
			transform.position += val;
		}
		else
		{
			characterController.Move(val);
		}
		if (movementMap.Shift)
		{
			targetY = ((Component)characterController).transform.position.y;
		}
		Vector3 val2 = position - ((Component)characterController).transform.position;
		if ((double)val2.magnitude < 0.01)
		{
			targetY = ((Component)characterController).transform.position.y;
		}
		Vector3 val3 = val;
		val3.y = 0f;
		if ((double)val3.sqrMagnitude > 0.001)
		{
			((Component)characterController).transform.rotation = Quaternion.LookRotation(val3);
		}
	}

	private Vector3 GetDirection()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		Transform transform = ((Component)Camera.main).transform;
		Vector3 result = transform.rotation * movementMap.Direction;
		if (MVGameController.Instance.fixedToYPlane && !movementMap.Shift)
		{
			result.y = 0f;
		}
		result.Normalize();
		return result;
	}

	private Vector3 GetMovementVelocity()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		Vector3 direction = GetDirection();
		targetSpeed = direction.magnitude * maxSpeed * ((!movementMap.Shift) ? 1f : speedModifier);
		speed = Mathf.Lerp(speed, targetSpeed, speedSmoothingTime * Time.deltaTime);
		return direction * speed * speedModifier;
	}

	private Vector3 GetElevationVelocity()
	{
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		if ((ignoreInputTypes & IgnoreInputTypes.MouseScroll) == 0)
		{
			if (MVInputWrapper.GetKey((KeyCode)101) || MVInputWrapper.GetKey((KeyCode)99))
			{
				if (MVInputWrapper.GetKey((KeyCode)101))
				{
					keyVelocity += keyAcceleration * Time.deltaTime;
				}
				if (MVInputWrapper.GetKey((KeyCode)99))
				{
					keyVelocity -= keyAcceleration * Time.deltaTime;
				}
			}
			else
			{
				float num = Mathf.Min(1f, keyDamping * Time.deltaTime);
				keyVelocity = (1f - num) * keyVelocity;
			}
			float num2 = 5f * MVInputWrapper.GetAxisRaw("Mouse ScrollWheel");
			targetY += num2 + keyVelocity * Time.deltaTime;
			return (targetY - ((Component)characterController).transform.position.y) * heightAdjustSpeed * Vector3.up;
		}
		if ((ignoreInputTypes & IgnoreInputTypes.MouseScroll) != 0)
		{
			return Vector3.zero;
		}
		return Vector3.zero;
	}

	public override void Deactivate()
	{
		base.Deactivate();
	}
}
