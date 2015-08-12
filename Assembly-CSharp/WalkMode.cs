using System;
using UnityEngine;

public class WalkMode : AvatarMode
{
	private const float USE_RADIUS = 2.5f;

	private const float USE_HEIGHT = 1.5f;

	private AvatarMotor characterMotor;

	private bool isJumping;

	private AvatarSound avatarSound;

	private MVAvatarLocal mvAvatar;

	public bool vehicleState;

	private readonly float swimStartProximity = 0.6f;

	private float prevWaterProximity;

	private bool IsSwimming => swimStartProximity <= prevWaterProximity;

	public bool InGunMode { private get; set; }

	public bool ForceRotateAvatarToFiringDirection { private get; set; }

	public WalkMode(MVAvatarLocal mvAvatar, AvatarMotor avatarMotor)
	{
		this.mvAvatar = mvAvatar;
		characterMotor = avatarMotor;
		avatarSound = mvAvatar.GameObject.GetComponent<AvatarSound>();
		AvatarMotor avatarMotor2 = characterMotor;
		avatarMotor2.OnWallJump = (AvatarMotor.OnWallJumpDelegate)Delegate.Combine(avatarMotor2.OnWallJump, new AvatarMotor.OnWallJumpDelegate(avatarSound.HandleWallJump));
		AvatarMotor avatarMotor3 = characterMotor;
		avatarMotor3.OnWallJump = (AvatarMotor.OnWallJumpDelegate)Delegate.Combine(avatarMotor3.OnWallJump, new AvatarMotor.OnWallJumpDelegate(HandleWallJump));
		AvatarMotor avatarMotor4 = characterMotor;
		avatarMotor4.OnActiveBounce = (AvatarMotor.OnActiveBounceDelegate)Delegate.Combine(avatarMotor4.OnActiveBounce, new AvatarMotor.OnActiveBounceDelegate(avatarSound.HandleActiveBounce));
	}

	private void HandleWallJump()
	{
		GameSessionCounters.Increment(GameSessionCounterType.WallJump);
	}

	public override void Activate()
	{
		mvAvatar.SetAnimation("Idle");
		if (mvAvatar.Body != null)
		{
			mvAvatar.Body.Visible = true;
		}
	}

	private void HandleWaterplane()
	{
		WaterPlaneManager waterPlaneManager = MVGameController.WOCM.WaterPlaneManager;
		float num = waterPlaneManager.ComputeAvatarWaterProximity(mvAvatar.GameObject.transform.position);
		if (!mvAvatar.IsDead)
		{
			if (prevWaterProximity < swimStartProximity && swimStartProximity <= num)
			{
				mvAvatar.SetAnimation("Swim");
			}
			else if (swimStartProximity < prevWaterProximity && num < swimStartProximity)
			{
				mvAvatar.SetAnimation("Walk");
			}
		}
		prevWaterProximity = num;
	}

	public override void FrameUpdate()
	{
		characterMotor.UpdateFunction();
	}

	public override void FixedUpdate(MovementMap movementMap)
	{
		HandleWaterplane();
		Transform transform = Camera.main.transform;
		Vector3 vector = movementMap.Direction;
		bool inputJump = movementMap.Jump;
		if (mvAvatar.IsDead)
		{
			vector = Vector3.zero;
			inputJump = false;
			characterMotor.InputJump = false;
			characterMotor.InputMoveDirection = Vector3.zero;
		}
		if (vector.magnitude > 0f)
		{
			Vector3 vector2 = transform.rotation * vector;
			vector2.y = 0f;
			vector = vector2.normalized;
		}
		Quaternion setQuaternion = Quaternion.identity;
		bool shouldSetRotation = false;
		if (!InGunMode && !ForceRotateAvatarToFiringDirection && vector != Vector3.zero)
		{
			setQuaternion = Quaternion.LookRotation(vector);
			shouldSetRotation = true;
		}
		if (!mvAvatar.IsDead)
		{
			characterMotor.InputMoveDirection = vector;
			characterMotor.InputJump = inputJump;
		}
		characterMotor.FixedUpdateFunction(setQuaternion, shouldSetRotation);
		if (!mvAvatar.IsDead)
		{
			isJumping = characterMotor.IsJumping();
			if (IsSwimming)
			{
				mvAvatar.SetAnimation("Swim");
			}
			else if (isJumping)
			{
				mvAvatar.SetAnimation("Jump");
			}
			else if (vehicleState)
			{
				mvAvatar.SetAnimation("Idle");
			}
			else if (vector.sqrMagnitude > 0f)
			{
				mvAvatar.SetAnimation("Walk");
			}
			else
			{
				mvAvatar.SetAnimation("Idle");
			}
		}
	}
}
