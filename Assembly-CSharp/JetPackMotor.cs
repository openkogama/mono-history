using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using MV.Common;
using UnityEngine;

public class JetPackMotor : MVRigidBody
{
	private float velocityConstant = 40f;

	private float thrust = 30f;

	private float thrustLeaveMode = 20f;

	private float velocityXZFactor = 0.2f;

	private float leaveModeForward = 4.5f;

	private KeyValuePair<float, float> leaveModeRotationRange = new KeyValuePair<float, float>(100f, 400f);

	private float runSpeed = 12f;

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

	private BounceState bounceState = new BounceState();

	private MVGroundState groundState = new MVGroundState();

	private MVMovableMotorState movableMotorState;

	private ImpactState impactState = new ImpactState();

	private MVInteractableBase interactable;

	private MVInteractableBase vehicleInteractable;

	private float HARDCODEDJETPACKAIRFRICTION = 0.43f;

	private float waterProximity;

	protected MvCharacterController controller;

	protected StuckEvaluator stuckEvaluator;

	private bool leaveMode;

	private MvCharacterController MvCharacterController => controller;

	public bool IsUnderWater => WaterProximity > 0.5f;

	public override Vector3 Velocity
	{
		get
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			return controller.Velocity / Time.fixedDeltaTime;
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

	public bool Thrust { get; set; }

	public bool InputRun { get; set; }

	public bool LeaveMode
	{
		get
		{
			return leaveMode;
		}
		set
		{
			leaveMode = value;
			if (leaveMode)
			{
				interactable = vehicleInteractable;
			}
		}
	}

	public override bool Grounded => groundState.Grounded;

	public float WaterProximity => waterProximity;

	public override bool IsMovementLocked { get; set; }

	public JetPackMotor()
	{
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Expected Obj, but got Unknown
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

	public void Init(MVInteractableBase interactableLocal, MVInteractableBase vehicleInteractable, MvCharacterController avatarController, float thrustStrength, float density)
	{
		thrust = thrustStrength;
		base.density = density;
		controller = avatarController;
		HashSet<int> worldIDsRecursive = worldObjectParent.WorldIDsRecursive;
		controller.IgnoreWoIds = worldIDsRecursive;
		MvCharacterController mvCharacterController = controller;
		mvCharacterController.OnControllerColliderHit = (MvCharacterController.OnControllerColliderHitDelegate)Delegate.Combine(mvCharacterController.OnControllerColliderHit, new MvCharacterController.OnControllerColliderHitDelegate(OnControllerColliderHit));
		stuckEvaluator = new StuckEvaluator(controller.GetOverlappingObjects);
		movableMotorState = new MVMovableMotorState();
		interactable = interactableLocal;
		this.vehicleInteractable = vehicleInteractable;
	}

	private void OnControllerColliderHit(MVControllerColliderHit hit)
	{
		moveHits.Add(hit);
	}

	public void UpdateFunction(Quaternion setQuaternion, bool shouldSetRotation)
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
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
			if (movablesVelocityVector)
			{
				velocity = GetVelocity(velocity, movableVelocityVector);
				Move(velocity, Vector3.zero);
			}
			else
			{
				velocity = GetVelocity(velocity, Vector3.zero);
				Move(velocity, movableVelocityVector);
			}
			bounceState.UpdateBounceState(moveHits, interactable);
			velocityPrevFrame = controller.Velocity / Time.fixedDeltaTime;
			if (!movablesVelocityVector)
			{
				velocityPrevFrame -= movableVelocityVector;
			}
			ApplyModifiersFromMaterials();
			DealImpactDamage(velocityPrevFrame, prevVelocity);
		}
	}

	private Vector3 GetVelocity(Vector3 velocity, Vector3 baseVelocity)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		velocity -= velocity * MathFunctions.Pow2(HARDCODEDJETPACKAIRFRICTION) * Time.deltaTime;
		velocity = ApplyInputVelocityChange(velocity, baseVelocity);
		if (!groundState.Grounded)
		{
			velocity = ApplyGravity(velocity, velocityPrevFrame, interactable);
		}
		velocity = bounceState.ApplyBounceVelocity(velocity);
		if (Thrust)
		{
			ApplyJetImpulse(velocity, thrust);
		}
		if (Thrust && LeaveMode)
		{
			ApplyJetImpulse(velocity, thrustLeaveMode);
			((Component)this).transform.RotateAround(controller.Center + ((Component)this).transform.position, Vector3.up, Random.Range(leaveModeRotationRange.Key, leaveModeRotationRange.Value) * Time.deltaTime);
			velocity += ((Component)this).transform.rotation * Vector3.forward * Random.Range(0f, leaveModeForward);
		}
		velocity = GetImpulse(velocity, interactable);
		velocity *= interactable.HandleModifierEffect(AvatarModifierEffect.VelocityDamping, 1f);
		return velocity;
	}

	private void ApplyJetImpulse(Vector3 velocity, float jetpackThrust)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		float num = Mathf.Clamp(Velocity.y, 0f, velocityConstant);
		float num2 = (velocityConstant - num) / velocityConstant;
		Vector3 val = velocity;
		val.y = 0f;
		val *= velocityXZFactor;
		Vector3 val2 = Vector3.up + val;
		val2.Normalize();
		AddImpulse(Vector3.up * jetpackThrust * num2);
	}

	protected void DealImpactDamage(Vector3 curVelocity, Vector3 prevVelocity)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		float num = impactState.UpdateImpactState(curVelocity, prevVelocity, moveHits, interactable);
		if (num != 0f)
		{
			interactable.TakeDamage(num, null, PlayerKilledByType.Impact);
		}
	}

	private void ApplyModifiersFromMaterials()
	{
		foreach (MVControllerColliderHit moveHit in moveHits)
		{
			interactable.AddModifier(moveHit.material.modifierPackageType);
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
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = GetDesiredHorizontalVelocity();
		if (groundState.Grounded)
		{
			val = MVRigidBody.AdjustGroundVelocityToNormal(val, groundState.GroundNormal) + baseVelocity;
		}
		Vector3 val2 = val - velocity;
		val2 *= MathFunctions.Pow2(HARDCODEDJETPACKAIRFRICTION);
		velocity += val2;
		return velocity;
	}

	private Vector3 GetDesiredHorizontalVelocity()
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		float baseValue = runSpeed;
		baseValue = interactable.HandleModifierEffect(AvatarModifierEffect.Speed, baseValue);
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
}
