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

	public bool InGunMode { private get; set; }

	public bool ForceRotateAvatarToFiringDirection { private get; set; }

	private bool IsSwimming => swimStartProximity <= prevWaterProximity;

	public WalkMode(MVAvatarLocal mvAvatar, AvatarMotor avatarMotor)
	{
		this.mvAvatar = mvAvatar;
		characterMotor = avatarMotor;
		avatarSound = mvAvatar.GameObject.GetComponent<AvatarSound>();
		AvatarMotor avatarMotor2 = characterMotor;
		avatarMotor2.OnWallJump = (AvatarMotor.OnWallJumpDelegate)Delegate.Combine(avatarMotor2.OnWallJump, new AvatarMotor.OnWallJumpDelegate(avatarSound.HandleWallJump));
		AvatarMotor avatarMotor3 = characterMotor;
		avatarMotor3.OnActiveBounce = (AvatarMotor.OnActiveBounceDelegate)Delegate.Combine(avatarMotor3.OnActiveBounce, new AvatarMotor.OnActiveBounceDelegate(avatarSound.HandleActiveBounce));
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
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		WaterPlaneManager waterPlaneManager = MVGameController.Instance.WOCM.WaterPlaneManager;
		float num = waterPlaneManager.ComputeAvatarWaterProximity(mvAvatar.GameObject.transform.position);
		if (prevWaterProximity < swimStartProximity && swimStartProximity <= num)
		{
			mvAvatar.SetAnimation("Swim");
		}
		else if (swimStartProximity < prevWaterProximity && num < swimStartProximity)
		{
			mvAvatar.SetAnimation("Walk");
		}
		prevWaterProximity = num;
	}

	public override void FixedUpdate(MovementMap movementMap)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		HandleWaterplane();
		Transform transform = ((Component)Camera.main).transform;
		Vector3 val = movementMap.Direction;
		if (mvAvatar.IsDead)
		{
			val = Vector3.zero;
			characterMotor.InputMoveDirection = Vector3.zero;
		}
		if (val.magnitude > 0f)
		{
			Vector3 val2 = transform.rotation * val;
			val2.y = 0f;
			val = val2.normalized;
		}
		Quaternion setQuaternion = Quaternion.identity;
		bool shouldSetRotation = false;
		if (!InGunMode && !ForceRotateAvatarToFiringDirection && val != Vector3.zero)
		{
			setQuaternion = Quaternion.LookRotation(val);
			shouldSetRotation = true;
		}
		if (!mvAvatar.IsDead)
		{
			characterMotor.InputMoveDirection = val;
			characterMotor.InputJump = movementMap.Jump;
			characterMotor.InputRun = movementMap.Run;
		}
		characterMotor.UpdateFunction(setQuaternion, shouldSetRotation);
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
			else if (val.sqrMagnitude > 0f)
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
