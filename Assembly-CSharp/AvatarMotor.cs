using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using MV.Common;
using UnityEngine;

public class AvatarMotor : MVRigidBody
{
	public delegate void OnWallJumpDelegate();

	public delegate void OnActiveBounceDelegate();

	protected MvCharacterController controller;

	protected ImpactState impactState = new ImpactState();

	private float walkSpeed = 3f;

	private float runSpeed = 8f;

	private float speedSmoothing = 10f;

	private float speed;

	private AnimationCurve slopeSpeedMultiplier = new AnimationCurve(new Keyframe[3]
	{
		new Keyframe(-90f, 1f),
		new Keyframe(0f, 1f),
		new Keyframe(90f, 1f)
	});

	private Vector3 velocityPrevFrame;

	private List<MVControllerColliderHit> moveHits = new List<MVControllerColliderHit>();

	private float inAirControlFactor = 3f;

	private JumpState jumpState;

	private BounceState bounceState = new BounceState();

	private MVGroundState groundState = new MVGroundState();

	public OnWallJumpDelegate OnWallJump;

	public OnActiveBounceDelegate OnActiveBounce;

	private MVMovableMotorState movableMotorState;

	protected StuckEvaluator stuckEvaluator;

	private float waterProximity;

	private MVInteractableBase interactableLocal;

	public override Vector3 Velocity
	{
		get
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			return controller.Velocity / Time.fixedDeltaTime;
		}
	}

	public MvCharacterController CharacterController
	{
		get
		{
			return controller;
		}
		set
		{
			controller = value;
		}
	}

	public Vector3 InputMoveDirection
	{
		[CompilerGenerated]
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return field;
		}
		[CompilerGenerated]
		set
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			field = value;
		}
	}

	public bool InputJump { get; set; }

	public bool InputRun { get; set; }

	private MvCharacterController MvCharacterController => controller;

	public override bool Grounded => groundState.Grounded;

	public float WaterProximity => waterProximity;

	public override bool IsMovementLocked { get; set; }

	public AvatarMotor()
	{
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Expected Obj, but got Unknown
	}

	protected override void SuspendImpactDamage()
	{
		impactState.SuspendImpactDamage();
	}

	public List<MVOverlapResult> GetOverlappingObjects()
	{
		return controller.GetOverlappingObjects();
	}

	public bool IsStuck()
	{
		return stuckEvaluator.Update();
	}

	public void Init(MVInteractableBase interactableLocal, Vector3 centerOffset)
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		controller = ((Component)this).gameObject.AddComponent<MvCharacterController>();
		controller.Init(0.45f, 1.9f, centerOffset);
		HashSet<int> worldIDsRecursive = worldObjectParent.WorldIDsRecursive;
		controller.IgnoreWoIds = worldIDsRecursive;
		MvCharacterController mvCharacterController = controller;
		mvCharacterController.OnControllerColliderHit = (MvCharacterController.OnControllerColliderHitDelegate)Delegate.Combine(mvCharacterController.OnControllerColliderHit, new MvCharacterController.OnControllerColliderHitDelegate(OnControllerColliderHit));
		stuckEvaluator = new StuckEvaluator(controller.GetOverlappingObjects);
		movableMotorState = new MVMovableMotorState();
		this.interactableLocal = interactableLocal;
		SetValuesToTweakSheet();
		this.jumpState = new JumpState();
		JumpState jumpState = this.jumpState;
		jumpState.OnWallJump = (JumpState.OnWallJumpDelegate)Delegate.Combine(jumpState.OnWallJump, (JumpState.OnWallJumpDelegate)(() =>
		{
			if (OnWallJump != null)
			{
				OnWallJump();
			}
		}));
	}

	private void SetValuesToTweakSheet()
	{
		inAirControlFactor = AvatarTweakSheet.CharacterMotor.inAirControlFactor;
		walkSpeed = AvatarTweakSheet.WalkMode.walkSpeed;
		runSpeed = AvatarTweakSheet.WalkMode.runSpeed;
		speedSmoothing = AvatarTweakSheet.WalkMode.speedSmoothing;
	}

	private void OnControllerColliderHit(MVControllerColliderHit hit)
	{
		moveHits.Add(hit);
	}

	public override void Reset()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		base.Reset();
		impactState.prevVelocityChangeVector = Vector3.zero;
		velocityPrevFrame = Vector3.zero;
		controller.Velocity = Vector3.zero;
	}

	public void UpdateFunction(Quaternion setQuaternion, bool shouldSetRotation)
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		waterProximity = MVGameController.Instance.WOCM.WaterPlaneManager.ComputeAvatarWaterProximity(((Component)MvCharacterController).gameObject.transform.position);
		if (!IsMovementLocked)
		{
			if (shouldSetRotation)
			{
				((Component)controller).transform.rotation = setQuaternion;
			}
			Vector3 prevVelocity = velocityPrevFrame;
			Vector3 velocity = velocityPrevFrame;
			bool movablesVelocityVector = movableMotorState.GetMovablesVelocityVector(controller, controller.Radius, groundState.GroundDepth, out var movableVelocityVector);
			GroundChange groundChange;
			if (movablesVelocityVector)
			{
				velocity = GetVelocity(velocity, movableVelocityVector);
				groundChange = Move(velocity, Vector3.zero);
			}
			else
			{
				velocity = GetVelocity(velocity, Vector3.zero);
				groundChange = Move(velocity, movableVelocityVector);
			}
			jumpState.UpdateJumpState(groundChange);
			bounceState.UpdateBounceState(moveHits, interactableLocal);
			UpdateVelocity();
			if (!movablesVelocityVector)
			{
				velocityPrevFrame -= movableVelocityVector;
			}
			ApplyModifiersFromMaterials();
			DealImpactDamage(velocityPrevFrame, prevVelocity);
			HandleSoundEffects();
		}
	}

	public void UpdateVelocity()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		velocityPrevFrame = controller.Velocity / Time.fixedDeltaTime;
	}

	private void HandleSoundEffects()
	{
		if (bounceState.Bounced && InputJump)
		{
			Debug.Log((object)"Bounced and Jumping");
			if (OnActiveBounce != null)
			{
				OnActiveBounce();
			}
		}
	}

	private Vector3 GetVelocity(Vector3 velocity, Vector3 baseVelocity)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		if (groundState.Grounded)
		{
			velocity = groundState.ApplySlidingVelocity(velocity, density, interactableLocal);
		}
		if (groundState.Grounded)
		{
			velocity -= velocity * MathFunctions.Pow2(interactableLocal.HandleModifierEffect(AvatarModifierEffect.Friction, groundState.GroundMaterial.physicalProperties.friction)) * Time.deltaTime;
		}
		velocity = ApplyInputVelocityChange(velocity, baseVelocity);
		if (!groundState.Grounded)
		{
			velocity = ApplyGravity(velocity, velocityPrevFrame, interactableLocal);
		}
		velocity = bounceState.ApplyBounceVelocity(velocity);
		velocity = jumpState.ApplyJumping(interactableLocal, groundState, density, WaterProximity, InputJump, velocity, moveHits);
		velocity = GetImpulse(velocity, interactableLocal);
		velocity *= interactableLocal.HandleModifierEffect(AvatarModifierEffect.VelocityDamping, 1f);
		return velocity;
	}

	protected void DealImpactDamage(Vector3 curVelocity, Vector3 prevVelocity)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		float num = impactState.UpdateImpactState(curVelocity, prevVelocity, moveHits, interactableLocal);
		if (num != 0f)
		{
			interactableLocal.TakeDamage(num, null, PlayerKilledByType.Impact);
		}
	}

	private void ApplyModifiersFromMaterials()
	{
		foreach (MVControllerColliderHit moveHit in moveHits)
		{
			interactableLocal.AddModifier(moveHit.material.modifierPackageType);
		}
	}

	private GroundChange Move(Vector3 velocity, Vector3 basevelocity)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		moveHits.Clear();
		Vector3 motion = (velocity + basevelocity) * Time.deltaTime;
		collisionFlags = controller.Move(motion);
		return groundState.UpdateIsGrounded(velocity, moveHits, controller);
	}

	private Vector3 ApplyInputVelocityChange(Vector3 velocity, Vector3 baseVelocity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_016d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = GetDesiredHorizontalVelocity();
		if (groundState.Grounded)
		{
			val = MVRigidBody.AdjustGroundVelocityToNormal(val, groundState.GroundNormal) + baseVelocity;
		}
		Vector3 val2 = val - velocity;
		if (groundState.Grounded)
		{
			val2 *= MathFunctions.Pow2(interactableLocal.HandleModifierEffect(AvatarModifierEffect.Friction, groundState.GroundMaterial.physicalProperties.friction));
			velocity += val2;
			if (MathFunctions.Pow2(interactableLocal.HandleModifierEffect(AvatarModifierEffect.Friction, groundState.GroundMaterial.physicalProperties.friction)) < 0.1f && val.magnitude != 0f)
			{
				velocity += val * 0.5f * Time.deltaTime;
			}
		}
		else if (val.magnitude != 0f)
		{
			Vector3 val3 = velocity;
			val3.y = 0f;
			float magnitude = val3.magnitude;
			val3 += val * inAirControlFactor * Time.deltaTime;
			float magnitude2 = val3.magnitude;
			if (magnitude2 > magnitude && magnitude2 > speed)
			{
				val3.Normalize();
				val3 *= magnitude;
			}
			velocity.x = val3.x;
			velocity.z = val3.z;
		}
		return velocity;
	}

	private Vector3 GetDesiredHorizontalVelocity()
	{
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		float baseValue = ((!InputRun) ? runSpeed : walkSpeed);
		baseValue = interactableLocal.HandleModifierEffect(AvatarModifierEffect.Speed, baseValue);
		Vector3 inputMoveDirection = InputMoveDirection;
		float num = inputMoveDirection.magnitude * baseValue;
		float num2 = speedSmoothing * Time.deltaTime;
		speed = Mathf.Lerp(speed, num, num2);
		Vector3 inputMoveDirection2 = InputMoveDirection;
		if (inputMoveDirection2.magnitude == 0f)
		{
			speed = 0f;
		}
		if (groundState.Grounded)
		{
			float num3 = Mathf.Asin(velocityPrevFrame.normalized.y) * 57.29578f;
			speed *= slopeSpeedMultiplier.Evaluate(num3);
		}
		return InputMoveDirection * speed;
	}

	public bool IsJumping()
	{
		return jumpState.Jumping;
	}
}
