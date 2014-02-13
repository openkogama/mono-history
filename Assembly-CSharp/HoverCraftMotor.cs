using MV.Common;
using UnityEngine;

public class HoverCraftMotor : SimpleVehicleMotorBase
{
	private const float verticalThrustTime = 0.6f;

	private const float stoppedJumpingTimeOut = 0.3f;

	private ImpactState impactState = new ImpactState();

	private Vector3 velocityPrevFrame;

	private BounceState bounceState = new BounceState();

	private MVGroundState groundState = new MVGroundState();

	private float thrustFactor = 10000f;

	private float dragCoefficientXZ = -2f;

	private float dragCoefficentUp = 10f;

	private float dragCoefficentDown = -50f;

	private float mass = 330f;

	private float angularSpeed = 3.4f;

	private float waterProximityThresshold = 0.01f;

	private float waterDownVelocity = 50f;

	private float waterOffset = -0.6f;

	private float magnitudeDivider = 8f;

	private float frictionFactor = 10f;

	private float hullRotationFactor = 20f;

	private float extraThrustFactor = 0.8f;

	private float maxUnderWaterYMovement = 40f;

	private float recalibrateCameraFactor = 1.25f;

	private float availableVerticalThrustTime = 0.6f;

	private bool wasJumping;

	private float stoppedJumpingTime = Time.time - 0.3f;

	private float regenerationFactor = 0.6f;

	private bool isVerticalThrusting;

	private float jumpForce = 200f;

	public VehicleCamera VehicleCamera { private get; set; }

	public override Vector3 Velocity
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return velocityPrevFrame;
		}
	}

	public override bool Grounded => groundState.Grounded;

	public override bool IsMovementLocked { get; set; }

	protected override void SuspendImpactDamage()
	{
		impactState.SuspendImpactDamage();
	}

	public override void Init(MvCharacterController characterController, MVInteractableBase interactableLocal)
	{
		base.Init(characterController, interactableLocal);
		density = 1.3f;
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

	public override void VehicleUpdateFunction()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		Vector3 prevVelocity = velocityPrevFrame;
		Vector3 velocity = velocityPrevFrame;
		bool movablesVelocityVector = movableMotorState.GetMovablesVelocityVector(controller, controller.Radius, groundState.GroundDepth, out var movableVelocityVector);
		if (movablesVelocityVector)
		{
			velocity = GetVehicleVelocity(velocity, movableVelocityVector);
			Move(velocity, Vector3.zero);
		}
		else
		{
			velocity = GetVehicleVelocity(velocity, Vector3.zero);
			Move(velocity, movableVelocityVector);
		}
		bounceState.UpdateBounceState(moveHits, interactableLocal);
		velocityPrevFrame = controller.Velocity / Time.fixedDeltaTime;
		if (!movablesVelocityVector)
		{
			velocityPrevFrame -= movableVelocityVector;
		}
		ApplyModifiersFromMaterials();
		DealImpactDamage(velocityPrevFrame, prevVelocity);
		DealDamage();
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

	private void DealDamage()
	{
		float num = interactableLocal.HandleModifierEffect(AvatarModifierEffect.EnvironmentDamagePrSec, 0f) * Time.deltaTime;
		if (num != 0f)
		{
			interactableLocal.TakeDamage(num, null, PlayerKilledByType.Environmental);
		}
	}

	private void ApplyModifiersFromMaterials()
	{
		foreach (MVControllerColliderHit moveHit in moveHits)
		{
			interactableLocal.AddModifier(moveHit.material.modifierPackageType);
		}
	}

	private void Move(Vector3 velocity, Vector3 basevelocity)
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
		groundState.UpdateIsGrounded(velocity, moveHits, controller);
	}

	private Vector3 GetVehicleVelocity(Vector3 velocity, Vector3 baseVelocity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		velocity = GetVehicleInputVelocity(velocity);
		float num = WaterProximity();
		if (num > waterProximityThresshold)
		{
			velocity = ApplyWaterGravity(velocity, num);
		}
		else if (!groundState.Grounded)
		{
			if (isVerticalThrusting)
			{
				density = 0.2f;
			}
			velocity = ApplyGravity(velocity, velocityPrevFrame, interactableLocal);
			density = 1.3f;
		}
		velocity = bounceState.ApplyBounceVelocity(velocity);
		velocity = GetImpulse(velocity, interactableLocal);
		velocity *= interactableLocal.HandleModifierEffect(AvatarModifierEffect.VelocityDamping, 1f);
		return velocity;
	}

	private float WaterProximity()
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		WaterPlaneManager waterPlaneManager = MVGameController.Instance.WOCM.WaterPlaneManager;
		return waterPlaneManager.ComputeAvatarWaterProximity(((Component)this).gameObject.transform.position + Vector3.up * waterOffset);
	}

	protected Vector3 ApplyWaterGravity(Vector3 velocity, float waterProximity)
	{
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		velocity.y += waterDownVelocity * Time.deltaTime * waterProximity;
		if (velocity.y > 0f)
		{
			velocity.y = Mathf.Clamp(velocity.y, 0f, maxUnderWaterYMovement);
		}
		return velocity;
	}

	private Vector3 GetVehicleInputVelocity(Vector3 velocity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		//IL_0188: Unknown result type (might be due to invalid IL or missing references)
		//IL_018d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0199: Unknown result type (might be due to invalid IL or missing references)
		//IL_019d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01be: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01da: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0208: Unknown result type (might be due to invalid IL or missing references)
		//IL_020d: Unknown result type (might be due to invalid IL or missing references)
		//IL_020f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0211: Unknown result type (might be due to invalid IL or missing references)
		//IL_0213: Unknown result type (might be due to invalid IL or missing references)
		//IL_0218: Unknown result type (might be due to invalid IL or missing references)
		//IL_021a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0225: Unknown result type (might be due to invalid IL or missing references)
		//IL_022a: Unknown result type (might be due to invalid IL or missing references)
		//IL_022c: Unknown result type (might be due to invalid IL or missing references)
		//IL_022d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0234: Unknown result type (might be due to invalid IL or missing references)
		//IL_0239: Unknown result type (might be due to invalid IL or missing references)
		//IL_023e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0241: Unknown result type (might be due to invalid IL or missing references)
		//IL_0242: Unknown result type (might be due to invalid IL or missing references)
		//IL_0247: Unknown result type (might be due to invalid IL or missing references)
		//IL_0249: Unknown result type (might be due to invalid IL or missing references)
		//IL_0174: Unknown result type (might be due to invalid IL or missing references)
		//IL_017b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0180: Unknown result type (might be due to invalid IL or missing references)
		//IL_0185: Unknown result type (might be due to invalid IL or missing references)
		velocity = HoverCraftFrictionXZ(velocity);
		((Component)controller).transform.RotateAround(Vector3.up, Time.fixedDeltaTime * angularSpeed * DirectInputMoveMap.x * Mathf.Abs(MVInputWrapper.GetAxis("Horizontal")));
		if (Mathf.Abs(DirectInputMoveMap.z) > 0f || (double)Mathf.Abs(DirectInputMoveMap.x) > 0.0)
		{
			Quaternion val = Quaternion.Euler(0f, VehicleCamera.RotationAroundY, 0f);
			Quaternion val2 = Quaternion.Slerp(((Component)controller).transform.rotation, ((Component)controller).transform.rotation * val, Time.fixedDeltaTime * recalibrateCameraFactor);
			float num = Quaternion.Angle(val2, ((Component)controller).transform.rotation);
			((Component)controller).transform.rotation = val2;
			if (VehicleCamera.RotationAroundY < 0f)
			{
				VehicleCamera.RotationAroundY += num;
			}
			else
			{
				VehicleCamera.RotationAroundY -= num;
			}
		}
		Vector3 directInputMoveMap = DirectInputMoveMap;
		directInputMoveMap.x = 0f;
		directInputMoveMap = ((Component)controller).transform.rotation * directInputMoveMap;
		if (groundState.Grounded)
		{
			directInputMoveMap = MVRigidBody.AdjustGroundVelocityToNormal(directInputMoveMap, groundState.GroundNormal);
		}
		Vector3 normalized = velocity.normalized;
		float sqrMagnitude = velocity.sqrMagnitude;
		Vector3 val3 = VerticalDrag(normalized, sqrMagnitude);
		Vector3 val4 = XZDrag(normalized, sqrMagnitude);
		Vector3 val5 = HullRotationDrag(normalized, sqrMagnitude);
		Vector3 val6 = val3 + val4 + val5;
		Vector3 val7 = directInputMoveMap * thrustFactor;
		Vector3 val8 = ((Component)controller).transform.rotation * Vector3.forward * val5.magnitude * extraThrustFactor;
		Vector3 val9 = (val7 + val6 + val8) / mass;
		velocity += val9 * Time.fixedDeltaTime;
		velocity = HandleVerticalThrust(velocity);
		return velocity;
	}

	private Vector3 HoverCraftFrictionXZ(Vector3 velocity)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = velocity;
		val.y = 0f;
		float magnitude = val.magnitude;
		if (magnitude < 0.001f)
		{
			velocity.x = 0f;
			velocity.z = 0f;
			return velocity;
		}
		float num = 1f / (magnitude / magnitudeDivider + 1f);
		Vector3 val2 = val.normalized * num * frictionFactor * Time.deltaTime;
		if (val2.sqrMagnitude > val.sqrMagnitude)
		{
			val2 = val;
		}
		velocity -= val2;
		return velocity;
	}

	private Vector3 XZDrag(Vector3 velocityNormal, float velocitySquareMagnitude)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		velocityNormal.y = 0f;
		Vector3 val = velocityNormal;
		val *= dragCoefficientXZ;
		return val * velocitySquareMagnitude;
	}

	private Vector3 HullRotationDrag(Vector3 velocityNormal, float velocitySquareMagnitude)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		float num = DragCoefficientXZHullRotationFactor(velocityNormal);
		velocityNormal.y = 0f;
		Vector3 val = velocityNormal;
		val *= 0f - num;
		return val * velocitySquareMagnitude;
	}

	private Vector3 VerticalDrag(Vector3 velocityNormal, float velocitySquareMagnitude)
	{
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		velocityNormal.x = 0f;
		velocityNormal.z = 0f;
		if (velocityNormal.y > 0f && !isVerticalThrusting)
		{
			velocityNormal.y *= dragCoefficentUp;
		}
		if (velocityNormal.y <= 0f && !isVerticalThrusting)
		{
			velocityNormal.y *= dragCoefficentDown;
		}
		return velocityNormal * velocitySquareMagnitude;
	}

	private Vector3 HandleVerticalThrust(Vector3 velocity)
	{
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		isVerticalThrusting = false;
		if (!Jump)
		{
			if (wasJumping)
			{
				stoppedJumpingTime = Time.time;
				wasJumping = false;
			}
			if (availableVerticalThrustTime < 0.6f)
			{
				availableVerticalThrustTime = Mathf.Clamp(availableVerticalThrustTime + Time.deltaTime * regenerationFactor, 0f, 0.6f);
			}
			return velocity;
		}
		if (Time.time - stoppedJumpingTime < 0.3f)
		{
			return velocity;
		}
		availableVerticalThrustTime = Mathf.Clamp(availableVerticalThrustTime - Time.deltaTime, 0f, 0.6f);
		if (availableVerticalThrustTime == 0f)
		{
			stoppedJumpingTime = Time.time;
			wasJumping = false;
			return velocity;
		}
		isVerticalThrusting = true;
		if (!wasJumping)
		{
			velocity += Vector3.up * jumpForce * Time.deltaTime;
		}
		wasJumping = true;
		return velocity;
	}

	private float DragCoefficientXZHullRotationFactor(Vector3 velocity)
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		velocity.y = 0f;
		velocity.Normalize();
		return Mathf.Abs(Vector3.Dot(((Component)controller).transform.rotation * Vector3.right, velocity)) * hullRotationFactor;
	}
}
