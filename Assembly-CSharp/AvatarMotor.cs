using System;
using System.Collections.Generic;
using MV.Common;
using UnityEngine;

public class AvatarMotor : MVRigidBody
{
	public delegate void OnWallJumpDelegate();

	public delegate void OnActiveBounceDelegate();

	private float walkSpeed = 8f;

	private float speed;

	private AnimationCurve slopeSpeedMultiplier = new AnimationCurve(new Keyframe(-90f, 1f), new Keyframe(0f, 1f), new Keyframe(90f, 1f));

	private Vector3 velocityPrevFrame;

	private float inAirControlFactor = 3f;

	private JumpState jumpState;

	private BounceState bounceState;

	private SizeState sizeState;

	private Quaternion platformerRotation = Quaternion.AngleAxis(90f, Vector3.up);

	private float platformerRotationSpeed = 12.4f;

	private SmoothCharacterController smoothCharacterController;

	private ImpactState impactState = new ImpactState(RuntimeEventType.AvatarImpact75, RuntimeEventType.AvatarImpact50, RuntimeEventType.AvatarImpact25);

	private MVMovableMotorState movableMotorState;

	private float waterProximity;

	private AvatarInteractable interactableLocal;

	protected StuckEvaluator stuckEvaluator;

	public OnWallJumpDelegate OnWallJump;

	public OnActiveBounceDelegate OnActiveBounce;

	private float currentLerp;

	private readonly float lerpTime = 0.4f;

	private MvCharacterController Controller => smoothCharacterController.Controller;

	public SizeState GetSizeState => sizeState;

	public override Vector3 Velocity => Controller.Velocity / Time.fixedDeltaTime;

	public Vector3 InputMoveDirection { get; set; }

	public bool InputJump { get; set; }

	public bool InputRun { get; set; }

	public override bool Grounded => groundState.Grounded;

	public float WaterProximity => waterProximity;

	public override bool IsMovementLocked { get; set; }

	protected override void SuspendImpactDamage()
	{
		impactState.SuspendImpactDamage();
	}

	public bool IsStuck()
	{
		return stuckEvaluator.Update();
	}

	public void Init(AvatarInteractable interactableLocal, Vector3 centerOffset)
	{
		Init();
		smoothCharacterController = gameObject.AddComponent<SmoothCharacterController>();
		smoothCharacterController.Init(gameObject);
		Controller.Init(0.45f, 1.9f, centerOffset);
		HashSet<int> worldIDsRecursive = worldObjectParent.WorldIDsRecursive;
		Controller.IgnoreWoIds = worldIDsRecursive;
		stuckEvaluator = new StuckEvaluator(Controller.GetOverlappingObjects);
		movableMotorState = new MVMovableMotorState();
		this.interactableLocal = interactableLocal;
		bounceState = new BounceState(interactableLocal);
		sizeState = new SizeState(interactableLocal, Controller);
		this.jumpState = new JumpState(0.2f);
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
		controller.OnControllerColliderHit = (Action<MVControllerColliderHit>)Delegate.Combine(controller.OnControllerColliderHit, new Action<MVControllerColliderHit>(sizeState.OnScalingWhileColliding));
		MvCharacterController controller2 = Controller;
		controller2.OnControllerColliderHit = (Action<MVControllerColliderHit>)Delegate.Combine(controller2.OnControllerColliderHit, new Action<MVControllerColliderHit>(interactableLocal.HandleMoveHit));
		MvCharacterController controller3 = Controller;
		controller3.OnControllerColliderHit = (Action<MVControllerColliderHit>)Delegate.Combine(controller3.OnControllerColliderHit, new Action<MVControllerColliderHit>(this.jumpState.HandleMoveHit));
		MvCharacterController controller4 = Controller;
		controller4.OnControllerColliderHit = (Action<MVControllerColliderHit>)Delegate.Combine(controller4.OnControllerColliderHit, new Action<MVControllerColliderHit>(bounceState.HandleMoveHit));
		MvCharacterController controller5 = Controller;
		controller5.OnControllerColliderHit = (Action<MVControllerColliderHit>)Delegate.Combine(controller5.OnControllerColliderHit, new Action<MVControllerColliderHit>(impactState.HandleMoveHit));
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

	public void OverrideDirection(Vector3 dir)
	{
		Controller.transform.forward = dir;
	}

	public void UpdateFunction()
	{
		smoothCharacterController.SmoothMove();
	}

	public void FixedUpdateFunction(Quaternion setQuaternion, bool shouldSetRotation)
	{
		waterProximity = MVGameController.WOCM.WaterPlaneManager.ComputeAvatarWaterProximity(Controller.gameObject.transform.position);
		if (IsMovementLocked)
		{
			return;
		}
		if (shouldSetRotation)
		{
			if (GameDB.GameType == MVGameType.Classic)
			{
				Controller.transform.rotation = setQuaternion;
			}
			else if (GameDB.GameType == MVGameType.Platformer)
			{
				platformerRotation = setQuaternion;
			}
		}
		if (GameDB.GameType == MVGameType.Platformer)
		{
			Controller.transform.rotation = Quaternion.Lerp(Controller.transform.rotation, platformerRotation, platformerRotationSpeed * Time.fixedDeltaTime);
		}
		Vector3 prevVelocity = velocityPrevFrame;
		Vector3 velocity = velocityPrevFrame;
		bool flag = movableMotorState.Move(velocity, Controller, Controller.Radius, groundState, out var movableVelocityVector);
		velocity = GetVelocity(velocity, movableVelocityVector);
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
		HandleSoundEffects();
		sizeState.UpdateScale();
	}

	public void UpdateVelocity()
	{
		velocityPrevFrame = Controller.Velocity / Time.fixedDeltaTime;
	}

	private void HandleSoundEffects()
	{
		if (bounceState.Bounced && InputJump && OnActiveBounce != null)
		{
			OnActiveBounce();
		}
	}

	private Vector3 GetVelocity(Vector3 velocity, Vector3 movableVelocity)
	{
		if (groundState.Grounded)
		{
			velocity = groundState.ApplySlidingVelocity(velocity, density, interactableLocal);
			velocity -= velocity * MathFunctions.Pow2(interactableLocal.HandleModifierEffect(AvatarModifierEffect.Friction, groundState.GroundMaterial.physicalProperties.friction)) * Time.fixedDeltaTime;
			velocity = ApplyInputVelocityChangeGrounded(velocity);
		}
		else
		{
			velocity = ApplyInputVelocityChange(velocity);
			velocity = ApplyGravity(velocity, velocityPrevFrame, interactableLocal);
		}
		velocity = bounceState.ApplyBounceVelocity(velocity);
		velocity = jumpState.ApplyJumping(interactableLocal, groundState, density, WaterProximity, InputJump, velocity, movableVelocity);
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
		collisionFlags = Controller.Move(motion);
		groundState.Update(Controller, velocity);
	}

	private Vector3 ApplyInputVelocityChangeGrounded(Vector3 velocity)
	{
		speed = GetSpeedGrounded(speed);
		Vector3 hVelocity = InputMoveDirection * speed;
		hVelocity = MVRigidBody.AdjustGroundVelocityToNormal(hVelocity, groundState.GroundNormal);
		Vector3 vector = hVelocity - velocity;
		vector *= MathFunctions.Pow2(interactableLocal.HandleModifierEffect(AvatarModifierEffect.Friction, groundState.GroundMaterial.physicalProperties.friction)) * Time.fixedDeltaTime / 0.02f;
		velocity += vector;
		if (MathFunctions.Pow2(interactableLocal.HandleModifierEffect(AvatarModifierEffect.Friction, groundState.GroundMaterial.physicalProperties.friction)) < 0.1f && hVelocity.magnitude != 0f)
		{
			velocity += hVelocity * 0.5f * Time.fixedDeltaTime;
		}
		return velocity;
	}

	private Vector3 ApplyInputVelocityChange(Vector3 velocity)
	{
		speed = GetSpeed(speed);
		Vector3 vector = InputMoveDirection * speed;
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

	private float GetSpeedGrounded(float currentSpeed)
	{
		float num = GetSpeed(currentSpeed);
		float time = Mathf.Asin(velocityPrevFrame.normalized.y) * 57.29578f;
		return num * slopeSpeedMultiplier.Evaluate(time);
	}

	private float GetSpeed(float currentSpeed)
	{
		float baseValue = walkSpeed;
		baseValue = interactableLocal.HandleModifierEffect(AvatarModifierEffect.Speed, baseValue);
		currentLerp += Time.fixedDeltaTime;
		if (currentLerp > lerpTime)
		{
			currentLerp = lerpTime;
		}
		if (InputMoveDirection.magnitude == 0f)
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
