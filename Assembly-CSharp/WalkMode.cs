using System;
using UnityEngine;

public class WalkMode : AvatarMode
{
	private float walkSpeed = 3f;

	private float runSpeed = 8f;

	private float animationSpeedRatio = 0.38f;

	private float speedSmoothing = 10f;

	public float targetSpeed;

	public float speed;

	public Vector3 targetVelocity;

	private Vector3 moveDirection;

	private float maxFallBelow = 200f;

	private MvCharacterController characterController;

	private CharacterMotor characterMotor;

	private bool isJumping;

	public WalkMode(MvCharacterController characterController, AvatarController avatarController)
		: base(avatarController)
	{
		SetValuesToTweakSheet();
		this.characterController = characterController;
		this.characterMotor = new CharacterMotor(characterController);
		CharacterMotor characterMotor = this.characterMotor;
		characterMotor.OnDamage = (CharacterMotor.OnDamageDelegate)Delegate.Combine(characterMotor.OnDamage, (CharacterMotor.OnDamageDelegate)((float damage) =>
		{
			avatarController.MVAvatar.Health.Value -= damage;
		}));
	}

	private void SetValuesToTweakSheet()
	{
		walkSpeed = AvatarTweakSheet.WalkMode.walkSpeed;
		runSpeed = AvatarTweakSheet.WalkMode.runSpeed;
		animationSpeedRatio = AvatarTweakSheet.WalkMode.animationSpeedRatio;
		speedSmoothing = AvatarTweakSheet.WalkMode.speedSmoothing;
	}

	public override void Activate()
	{
		avatarController.MVAvatar.Animation.Value = "Idle";
		avatarAnimation.SetVisible(visible: true);
	}

	public override void Reset()
	{
		base.Reset();
		characterMotor.Reset();
	}

	public override void ApplyImpulse(Vector3 impulse, bool suspendImpactDamage = false)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		characterMotor.AddImpulse(impulse, suspendImpactDamage);
	}

	public override void FixedUpdate()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		//IL_0168: Unknown result type (might be due to invalid IL or missing references)
		Transform transform = ((Component)Camera.main).transform;
		Vector3 val = transform.rotation * movementMap.Direction;
		val.y = 0f;
		val.Normalize();
		moveDirection = val.normalized;
		if (!avatarController.InGunMode && moveDirection != Vector3.zero)
		{
			((Component)characterController).transform.rotation = Quaternion.LookRotation(moveDirection);
		}
		float num = ((!movementMap.Shift) ? runSpeed : walkSpeed);
		targetSpeed = val.magnitude * num;
		float num2 = speedSmoothing * Time.deltaTime;
		speed = Mathf.Lerp(speed, targetSpeed, num2);
		characterMotor.SetMaxSpeed(speed);
		characterMotor.inputMoveDirection = moveDirection;
		characterMotor.inputJump = movementMap.Jump;
		characterMotor.UpdateFunction();
		avatarAnimation.Move(moveDirection.magnitude * Time.deltaTime);
		float y = ((Component)characterController).transform.position.y;
		Bounds worldBounds = MVGameController.Instance.WOCM.WorldBounds;
		if (y < worldBounds.min.y - maxFallBelow)
		{
			avatarController.MVAvatar.Health.Value = 0f;
		}
		if (avatarController.AvatarState != AvatarState.Dead)
		{
			isJumping = characterMotor.IsJumping();
			if (isJumping)
			{
				avatarController.MVAvatar.Animation.Value = "Jump";
			}
			else if (moveDirection.sqrMagnitude > 0f)
			{
				avatarController.MVAvatar.Animation.Value = "Walk";
			}
			else
			{
				avatarController.MVAvatar.Animation.Value = "Idle";
			}
		}
	}

	public override void ProxyUpdate()
	{
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		if (MVGameController.Instance.Game.JoinState == MVJoinState.Playing)
		{
			MVNetworkListener mVNetworkListener = (MVNetworkListener)avatarController.MVAvatar.NetworkObject;
			if (mVNetworkListener != null)
			{
				Vector3 calculatedVelocity = mVNetworkListener.CalculatedVelocity;
				calculatedVelocity.y = 0f;
				speed = calculatedVelocity.magnitude * 75f;
			}
		}
	}

	public override void HandleInput(NetworkInputActionCodes actionCode, NetworkInputKeyCodes keyCode)
	{
		base.HandleInput(actionCode, keyCode);
	}
}
