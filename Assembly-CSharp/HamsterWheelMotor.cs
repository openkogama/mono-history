using MV.Common;
using UnityEngine;

public class HamsterWheelMotor : SimpleVehicleMotorBase
{
	private const float accelerationSpeed = 48f;

	private const float verticalThrustTime = 1.2f;

	private const float stoppedJumpingTimeOut = 0.3f;

	private ImpactState impactState = new ImpactState();

	private MVGroundState groundState = new MVGroundState();

	private HamsterWheelBounceState bounceState = new HamsterWheelBounceState();

	private Vector3 curVelocity = Vector3.zero;

	private float speed;

	private float maxSpeed = 38.8f;

	private float minSpeed = -10f;

	private float angularSpeed = 1.4f;

	private float recalibrateCameraFactor = 1.05f;

	private float magnitudeDivider = 8f;

	private float frictionFactor = 1f;

	private float availableVerticalThrustTime = 1.2f;

	private bool wasJumping;

	private float stoppedJumpingTime = Time.time - 0.3f;

	private float regenerationFactor = 0.6f;

	private float jumpForce = 600f;

	private float waterProximityThresshold = 0.01f;

	private float waterDownVelocity = 50f;

	private float waterOffset = -0.6f;

	private float maxUnderWaterYMovement = 40f;

	public VehicleCamera VehicleCamera { private get; set; }

	public override bool Grounded => groundState.Grounded;

	public override Vector3 Velocity
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return curVelocity;
		}
	}

	public override bool IsMovementLocked { get; set; }

	public HamsterWheelMotor()
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
	}

	public override void Init(MvCharacterController characterController, MVInteractableBase interactableLocal)
	{
		base.Init(characterController, interactableLocal);
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
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = curVelocity;
		Vector3 velocity = curVelocity;
		bool movablesVelocityVector = movableMotorState.GetMovablesVelocityVector(controller, controller.Radius, groundState.GroundDepth, out var movableVelocityVector);
		if (movablesVelocityVector)
		{
			velocity = GetVechicleVelocity(velocity, movableVelocityVector);
			Move(velocity, Vector3.zero);
		}
		else
		{
			velocity = GetVechicleVelocity(velocity, Vector3.zero);
			Move(velocity, movableVelocityVector);
		}
		bounceState.UpdateBounceStateMaterial(moveHits, interactableLocal);
		bounceState.UpdateBouncyness(moveHits, velocity, groundState.Grounded);
		curVelocity = controller.Velocity / Time.fixedDeltaTime;
		if (!movablesVelocityVector)
		{
			curVelocity -= movableVelocityVector;
		}
		ApplyModifiersFromMaterials();
		DealImpactDamage(curVelocity * 0.5f, val * 0.5f);
		DealDamage();
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
		curVelocity = Vector3.zero;
		controller.Velocity = Vector3.zero;
	}

	protected override void SuspendImpactDamage()
	{
		impactState.SuspendImpactDamage();
	}

	private Vector3 GetVechicleVelocity(Vector3 velocity, Vector3 baseVelocity)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_021a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0225: Unknown result type (might be due to invalid IL or missing references)
		//IL_022f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0234: Unknown result type (might be due to invalid IL or missing references)
		//IL_0235: Unknown result type (might be due to invalid IL or missing references)
		//IL_0236: Unknown result type (might be due to invalid IL or missing references)
		//IL_0237: Unknown result type (might be due to invalid IL or missing references)
		//IL_023c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0264: Unknown result type (might be due to invalid IL or missing references)
		//IL_0266: Unknown result type (might be due to invalid IL or missing references)
		//IL_0271: Unknown result type (might be due to invalid IL or missing references)
		//IL_0276: Unknown result type (might be due to invalid IL or missing references)
		//IL_0254: Unknown result type (might be due to invalid IL or missing references)
		//IL_0257: Unknown result type (might be due to invalid IL or missing references)
		//IL_025c: Unknown result type (might be due to invalid IL or missing references)
		//IL_027e: Unknown result type (might be due to invalid IL or missing references)
		//IL_027f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0284: Unknown result type (might be due to invalid IL or missing references)
		//IL_028c: Unknown result type (might be due to invalid IL or missing references)
		//IL_028d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0292: Unknown result type (might be due to invalid IL or missing references)
		//IL_0295: Unknown result type (might be due to invalid IL or missing references)
		//IL_029c: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ac: Unknown result type (might be due to invalid IL or missing references)
		velocity *= 0.98f;
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
		if (DirectInputMoveMap.z > 0f)
		{
			if (speed < maxSpeed)
			{
				speed += 48f * Time.fixedDeltaTime;
			}
			if (speed > maxSpeed)
			{
				speed = maxSpeed;
			}
		}
		if (DirectInputMoveMap.z < 0f)
		{
			if (speed > minSpeed)
			{
				speed -= 48f * Time.fixedDeltaTime;
			}
			if (speed < minSpeed)
			{
				speed = minSpeed;
			}
		}
		if (DirectInputMoveMap.z == 0f)
		{
			speed = 0f;
		}
		Vector3 val3 = ((Component)this).transform.forward * speed * Time.fixedDeltaTime;
		velocity += val3;
		float num2 = WaterProximity();
		velocity = ((!(num2 > waterProximityThresshold)) ? ApplyGravity(velocity, curVelocity, interactableLocal) : ApplyWaterGravity(velocity, num2));
		velocity = bounceState.ApplyBouncyness(velocity);
		velocity = bounceState.ApplyBounceVelocityMaterials(velocity);
		velocity = GetImpulse(velocity, interactableLocal);
		velocity = HandleVerticalThrust(velocity);
		return velocity;
	}

	private Vector3 AddFrictionXZ(Vector3 velocity)
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

	private Vector3 HandleVerticalThrust(Vector3 velocity)
	{
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		if (!Jump)
		{
			if (wasJumping)
			{
				stoppedJumpingTime = Time.time;
				wasJumping = false;
			}
			if (availableVerticalThrustTime < 1.2f)
			{
				availableVerticalThrustTime = Mathf.Clamp(availableVerticalThrustTime + Time.deltaTime * regenerationFactor, 0f, 1.2f);
			}
			return velocity;
		}
		if (Time.time - stoppedJumpingTime < 0.3f)
		{
			return velocity;
		}
		availableVerticalThrustTime = Mathf.Clamp(availableVerticalThrustTime - Time.deltaTime, 0f, 1.2f);
		if (availableVerticalThrustTime == 0f)
		{
			stoppedJumpingTime = Time.time;
			wasJumping = false;
			return velocity;
		}
		float num = WaterProximity();
		if (!groundState.Grounded && num <= waterProximityThresshold)
		{
			return velocity;
		}
		if (!wasJumping)
		{
			velocity += Vector3.up * jumpForce * Time.deltaTime;
		}
		wasJumping = true;
		return velocity;
	}

	private void ApplyModifiersFromMaterials()
	{
		foreach (MVControllerColliderHit moveHit in moveHits)
		{
			interactableLocal.AddModifier(moveHit.material.modifierPackageType);
		}
	}

	private void DealImpactDamage(Vector3 curVelocity, Vector3 prevVelocity)
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
}
