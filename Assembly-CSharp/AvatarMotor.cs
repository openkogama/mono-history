using System;
using System.Collections.Generic;
using MV.Common;
using UnityEngine;

public class AvatarMotor : MVRigidBody
{
	public delegate void OnWallJumpDelegate();

	public delegate void OnActiveBounceDelegate();

	private float walkSpeedDefault = 8f;

	private float walkSpeed = 8f;

	private float speed;

	private AnimationCurve slopeSpeedMultiplier = new AnimationCurve(new Keyframe(-90f, 1f), new Keyframe(0f, 1f), new Keyframe(90f, 1f));

	private Vector3 velocityPrevFrame;

	private float inAirControlFactor = 3f;

	private JumpState jumpState;

	private BounceState bounceState;

	private WaterState waterState;

	private SmoothCharacterController smoothCharacterController;

	private ImpactState impactState = new ImpactState(RuntimeEventType.AvatarImpact75, RuntimeEventType.AvatarImpact50, RuntimeEventType.AvatarImpact25);

	private MVMovableMotorState movableMotorState;

	private AvatarInteractable interactableLocal;

	private float speedBoostSetting = 1f;

	private float slowFallVelocityMultiplier = 1f;

	private float frictionMultiplier = 1f;

	protected StuckEvaluator stuckEvaluator;

	public OnWallJumpDelegate OnWallJump;

	public OnActiveBounceDelegate OnActiveBounce;

	private float currentLerp;

	private readonly float lerpTime = 0.4f;

	private MvCharacterController Controller => smoothCharacterController.Controller;

	public SizeState GetSizeState { get; private set; }

	private float FrictionCoefficient => MathFunctions.Pow2(interactableLocal.HandleModifierEffect(AvatarModifierEffect.Friction, groundState.GroundMaterial.PhysicalProperties.friction * frictionMultiplier));

	public override Vector3 Velocity => Controller.Velocity / Time.fixedDeltaTime;

	public override bool Grounded => groundState.Grounded;

	public override bool IsMovementLocked { get; set; }

	protected override void SuspendImpactDamage()
	{
		impactState.SuspendImpactDamage();
	}

	public bool IsStuck()
	{
		return stuckEvaluator.Update();
	}

	public void Init(AvatarInteractable interactableLocal, Vector3 centerOffset, MVWorldObjectClient worldObjectOwner, WorldObjectSkillDataManager skillDataManager)
	{
		Init();
		InitSkills(skillDataManager);
		smoothCharacterController = gameObject.AddComponent<SmoothCharacterController>();
		smoothCharacterController.Init(gameObject, null, worldObjectOwner);
		Controller.Init(0.45f, 1.9f, centerOffset);
		MVGameControllerBase.Game.LocalPlayer.BoostController.SubscribeToBoostChanged(BoostType.MovementSpeedFloatMultiplier, HandleMovementBoost);
		HandleMovementBoost();
		HashSet<int> worldIDsRecursive = worldObjectParent.WorldIDsRecursive;
		Controller.IgnoreWoIds = worldIDsRecursive;
		stuckEvaluator = new StuckEvaluator(Controller.GetOverlappingObjects);
		movableMotorState = new MVMovableMotorState();
		this.interactableLocal = interactableLocal;
		bounceState = new BounceState(interactableLocal);
		waterState = new WaterState(skillDataManager);
		GetSizeState = new SizeState(interactableLocal, Controller);
		this.jumpState = new JumpState(0.2f, skillDataManager);
		JumpState jumpState = this.jumpState;
		jumpState.OnWallJump = (JumpState.OnWallJumpDelegate)Delegate.Combine(jumpState.OnWallJump, (JumpState.OnWallJumpDelegate)(() =>
		{
			if (OnWallJump != null)
			{
				OnWallJump();
			}
		}));
		MVGroundState mVGroundState = groundState;
		mVGroundState.OnGroundChange = (Action<GroundChange>)Delegate.Combine(mVGroundState.OnGroundChange, new Action<GroundChange>(this.jumpState.UpdateJumpState));
		MvCharacterController controller = Controller;
		controller.OnControllerColliderHit = (Action<MVControllerColliderHit>)Delegate.Combine(controller.OnControllerColliderHit, new Action<MVControllerColliderHit>(GetSizeState.OnScalingWhileColliding));
		MvCharacterController controller2 = Controller;
		controller2.OnControllerColliderHit = (Action<MVControllerColliderHit>)Delegate.Combine(controller2.OnControllerColliderHit, new Action<MVControllerColliderHit>(interactableLocal.HandleMoveHit));
		MvCharacterController controller3 = Controller;
		controller3.OnControllerColliderHit = (Action<MVControllerColliderHit>)Delegate.Combine(controller3.OnControllerColliderHit, new Action<MVControllerColliderHit>(this.jumpState.HandleMoveHit));
		MvCharacterController controller4 = Controller;
		controller4.OnControllerColliderHit = (Action<MVControllerColliderHit>)Delegate.Combine(controller4.OnControllerColliderHit, new Action<MVControllerColliderHit>(bounceState.HandleMoveHit));
		MvCharacterController controller5 = Controller;
		controller5.OnControllerColliderHit = (Action<MVControllerColliderHit>)Delegate.Combine(controller5.OnControllerColliderHit, new Action<MVControllerColliderHit>(impactState.HandleMoveHit));
	}

	private void InitSkills(WorldObjectSkillDataManager skillDataManager)
	{
		if (skillDataManager.HasSkill("SuperSpeed"))
		{
			speedBoostSetting = (float)skillDataManager.GetSkillIntValue("SuperSpeed") / 100f;
		}
		bool flag = skillDataManager.HasSkill("SlowFall");
		slowFallVelocityMultiplier = ((!flag) ? 1f : ((float)(100 - skillDataManager.GetSkillIntValue("SlowFall")) / 100f));
		bool flag2 = skillDataManager.HasSkill("FrictionMultiplier");
		frictionMultiplier = ((!flag2) ? 1f : ((float)(100 - skillDataManager.GetSkillIntValue("FrictionMultiplier")) / 100f));
	}

	public void OverrideCharacterController(SmoothCharacterController controller)
	{
		smoothCharacterController = controller;
	}

	public override void Reset()
	{
		base.Reset();
		impactState.prevVelocityChangeVector = Vector3.zero;
		velocityPrevFrame = Vector3.zero;
		smoothCharacterController.Reset();
	}

	public void UpdateFunction()
	{
		smoothCharacterController.SmoothMove();
	}

	public void FixedUpdateFunction(IMotorAPI motorApi)
	{
		if (!IsMovementLocked)
		{
			Controller.transform.rotation = motorApi.Rotation;
			Vector3 prevVelocity = velocityPrevFrame;
			Vector3 velocity = velocityPrevFrame;
			bool flag = movableMotorState.Move(velocity, Controller, Controller.Radius, groundState, out var movableVelocityVector);
			velocity = GetVelocity(velocity, movableVelocityVector, motorApi.Jump, motorApi.Direction);
			if (flag)
			{
				Move(velocity, Vector3.zero);
			}
			else
			{
				Move(velocity, movableVelocityVector);
			}
			UpdateVelocity();
			if (!flag)
			{
				velocityPrevFrame -= movableVelocityVector;
			}
			DealImpactDamage(velocityPrevFrame, prevVelocity);
			HandleSoundEffects(motorApi.Jump);
			GetSizeState.UpdateScale();
			waterState.Update(Controller.transform.position, interactableLocal);
		}
	}

	public void UpdateVelocity()
	{
		velocityPrevFrame = Controller.Velocity / Time.fixedDeltaTime;
	}

	private void HandleSoundEffects(bool inputJump)
	{
		if (bounceState.Bounced && inputJump && OnActiveBounce != null)
		{
			OnActiveBounce();
		}
	}

	private void HandleMovementBoost()
	{
		walkSpeed = walkSpeedDefault;
		if (MVGameControllerBase.Game.LocalPlayer.BoostController.TryGetActiveBoost(BoostType.MovementSpeedFloatMultiplier, out var boost))
		{
			walkSpeed = walkSpeedDefault * (1f + (float)(int)boost.Value / 100f);
		}
	}

	private void OnDestroy()
	{
		if (MVGameControllerBase.IsAlive)
		{
			MVGameControllerBase.Game.LocalPlayer.BoostController.UnSubscribeToBoostChanged(BoostType.MovementSpeedFloatMultiplier, HandleMovementBoost);
		}
	}

	private Vector3 GetVelocity(Vector3 velocity, Vector3 movableVelocity, bool inputJump, Vector3 inputDirection)
	{
		if (groundState.Grounded)
		{
			velocity = groundState.ApplySlidingVelocity(velocity, density, interactableLocal);
			velocity -= velocity * FrictionCoefficient * Time.fixedDeltaTime;
			velocity = ApplyInputVelocityChangeGrounded(velocity, inputDirection);
		}
		else
		{
			velocity = ApplyInputVelocityChange(velocity, inputDirection);
			velocity = ApplyGravity(velocity, velocityPrevFrame, interactableLocal);
			if (inputJump && velocity.y < 0f)
			{
				velocity.y *= slowFallVelocityMultiplier;
			}
		}
		velocity = bounceState.ApplyBounceVelocity(velocity);
		velocity = jumpState.ApplyJumping(interactableLocal, groundState, density, MVGameControllerBase.WaterPlaneManager.ComputeAvatarWaterProximity(Controller.gameObject.transform.position), inputJump, velocity, movableVelocity);
		velocity = GetImpulse(velocity, interactableLocal);
		velocity = MVRigidBody.VelocityDamping(velocity, 1f, interactableLocal);
		return velocity;
	}

	protected void DealImpactDamage(Vector3 curVelocity, Vector3 prevVelocity)
	{
		float num = impactState.UpdateImpactState(curVelocity, prevVelocity, interactableLocal);
		if (num != 0f)
		{
			interactableLocal.TakeDamage(num, null, PlayerKilledByType.Impact);
		}
	}

	private void Move(Vector3 velocity, Vector3 movableVelocity)
	{
		Vector3 motion = (velocity + movableVelocity) * Time.fixedDeltaTime;
		Controller.Move(motion);
		groundState.Update(Controller, velocity);
	}

	private Vector3 ApplyInputVelocityChangeGrounded(Vector3 velocity, Vector3 inputDirection)
	{
		speed = GetSpeedGrounded(speed, inputDirection);
		Vector3 hVelocity = inputDirection * speed;
		hVelocity = MVRigidBody.AdjustGroundVelocityToNormal(hVelocity, groundState.GroundNormal);
		Vector3 vector = hVelocity - velocity;
		vector *= FrictionCoefficient * Time.fixedDeltaTime / 0.02f;
		velocity += vector;
		if (FrictionCoefficient < 0.1f && hVelocity.magnitude != 0f)
		{
			velocity += hVelocity * 0.5f * Time.fixedDeltaTime;
		}
		return velocity;
	}

	private Vector3 ApplyInputVelocityChange(Vector3 velocity, Vector3 inputDirection)
	{
		speed = GetSpeed(speed, inputDirection);
		Vector3 vector = inputDirection * speed;
		if (vector.magnitude == 0f)
		{
			return velocity;
		}
		Vector3 vector2 = velocity;
		vector2.y = 0f;
		float magnitude = vector2.magnitude;
		vector2 += vector * inAirControlFactor * Time.fixedDeltaTime;
		float magnitude2 = vector2.magnitude;
		if (magnitude2 > magnitude && magnitude2 > speed)
		{
			vector2.Normalize();
			vector2 *= magnitude;
		}
		velocity.x = vector2.x;
		velocity.z = vector2.z;
		return velocity;
	}

	private float GetSpeedGrounded(float currentSpeed, Vector3 inputDirection)
	{
		float num = GetSpeed(currentSpeed, inputDirection);
		float time = Mathf.Asin(velocityPrevFrame.normalized.y) * 57.29578f;
		return num * slopeSpeedMultiplier.Evaluate(time);
	}

	private float GetSpeed(float currentSpeed, Vector3 inputDirection)
	{
		float baseValue = walkSpeed;
		baseValue = interactableLocal.HandleModifierEffect(AvatarModifierEffect.Speed, baseValue) * speedBoostSetting;
		currentLerp += Time.fixedDeltaTime;
		if (currentLerp > lerpTime)
		{
			currentLerp = lerpTime;
		}
		if (inputDirection.magnitude == 0f)
		{
			currentLerp = 0f;
		}
		return Mathf.Sin(currentLerp / lerpTime * 0.5f * (float)Math.PI) * baseValue;
	}

	public bool IsJumping()
	{
		return jumpState.Jumping;
	}
}
