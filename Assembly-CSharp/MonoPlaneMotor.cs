using MV.Common;
using UnityEngine;

public class MonoPlaneMotor : SimpleVehicleMotorBase
{
	private ImpactStateMonoPlane impactState = new ImpactStateMonoPlane();

	private Vector3 velocityPrevFrame;

	private BounceState bounceState = new BounceState();

	private MVGroundState groundState = new MVGroundState();

	private float thrustFactor = 10000f;

	private float mass = 330f;

	private float waterProximityThresshold = 0.01f;

	private float waterDownVelocity = 50f;

	private float waterOffset = -0.6f;

	private float maxUnderWaterYMovement = 40f;

	public int xMinLimit = -87;

	public int xMaxLimit = 87;

	private float mouseSensitivity = 5f;

	private float xAxisVelocity;

	private float yAxisVelocity;

	private float rotationSmoothTime = 0.5f;

	private float angularSpeed = 3.4f;

	private GroundChange groundChange = GroundChange.FromAirToGrounded;

	private float HARDCODEDJETPACKAIRFRICTION = 0.13f;

	private float yAxisTarget;

	private float xAxisTarget;

	private float yAxis;

	private float xAxis;

	private Quaternion planeRotation = default;

	private float throttleUpTime = 2f;

	private float throttleDownTime = 2f;

	private float throttle;

	private float velocityFactor = 50f;

	private float dragFactor = -200f;

	private float pitchUpDragFactor = -150f;

	private float pitchDownDragFactor = -100f;

	private float velocityMinForPitchSteady = 25f;

	private float pitchDownFactor = 2f;

	public AirCraftCamera VehicleCamera { private get; set; }

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

	public MonoPlaneMotor()
	{
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
	}

	protected override void SuspendImpactDamage()
	{
		impactState.SuspendImpactDamage();
	}

	public override void Init(MvCharacterController characterController, MVInteractableBase interactableLocal)
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		base.Init(characterController, interactableLocal);
		xAxisVelocity = (yAxisVelocity = 0f);
		xAxis = (xAxisTarget = ((Component)controller).transform.eulerAngles.x);
		yAxis = (yAxisTarget = ((Component)controller).transform.eulerAngles.y);
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

	public override void OnLocalVehicleLeave()
	{
		base.OnLocalVehicleLeave();
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
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		moveHits.Clear();
		Vector3 motion = (velocity + basevelocity) * Time.deltaTime;
		collisionFlags = controller.Move(motion);
		groundChange = groundState.UpdateIsGrounded(velocity, moveHits, controller);
	}

	private Vector3 GetVehicleVelocity(Vector3 velocity, Vector3 baseVelocity)
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		if (groundChange == GroundChange.UnChanged)
		{
			Debug.Log((object)"get rid of me");
		}
		float num = WaterProximity();
		if (num > waterProximityThresshold)
		{
			velocity = ApplyWaterGravity(velocity, num);
		}
		else if (!groundState.Grounded)
		{
			velocity = ApplyGravity(velocity, velocityPrevFrame, interactableLocal);
			velocity = HandleLift(velocity, interactableLocal);
		}
		if (groundState.Grounded)
		{
			velocity = groundState.ApplySlidingVelocity(velocity, density, interactableLocal);
		}
		velocity = ((!groundState.Grounded) ? (velocity - velocity * MathFunctions.Pow2(interactableLocal.HandleModifierEffect(AvatarModifierEffect.Friction, HARDCODEDJETPACKAIRFRICTION)) * Time.deltaTime) : (velocity - velocity * MathFunctions.Pow2(interactableLocal.HandleModifierEffect(AvatarModifierEffect.Friction, groundState.GroundMaterial.physicalProperties.friction)) * Time.deltaTime));
		velocity = GetVehicleInputVelocity(velocity);
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
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		planeRotation = GeneratePlaneRotation(velocity);
		((Component)this).transform.rotation = Quaternion.Euler(0f, yAxis, 0f);
		VehicleCamera.lookAt.localRotation = Quaternion.Euler(xAxis, 0f, 0f);
		throttle = UpdateThrottle(DirectInputMoveMap.z);
		Vector3 val = planeRotation * Vector3.forward;
		Vector3 val2 = planeRotation * Vector3.up;
		Vector3 val3 = planeRotation * Vector3.right;
		Vector3 position = ((Component)controller).transform.position;
		Debug.DrawLine(position, position + val, Color.blue, 0f, false);
		Debug.DrawLine(position, position + val2, Color.green, 0f, false);
		Debug.DrawLine(position, position + val3, Color.red, 0f, false);
		Vector3 val4 = CalcDrag(velocity);
		Vector3 val5 = val * thrustFactor * throttle;
		Vector3 val6 = (val5 + val4) / mass;
		velocity += val6 * Time.fixedDeltaTime;
		return velocity;
	}

	protected Vector3 HandleLift(Vector3 velocity, MVInteractableBase interactableLocal)
	{
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		float num = Mathf.Clamp(velocity.magnitude / velocityFactor, 0f, 1f);
		velocity.y += num * 30f * interactableLocal.HandleModifierEffect(AvatarModifierEffect.Density, density) * Time.deltaTime;
		return velocity;
	}

	private Vector3 CalcDrag(Vector3 velocity)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		float num = 0f;
		float num2 = Vector3.Dot(planeRotation * Vector3.forward, Vector3.up);
		num = ((!(num2 > 0f)) ? (pitchDownDragFactor * num2) : (pitchUpDragFactor * num2));
		return velocity * (dragFactor + num);
	}

	private float UpdateThrottle(float forwardInput)
	{
		if (forwardInput > 0f)
		{
			throttle = Mathf.Clamp(throttle + Time.deltaTime / throttleUpTime, 0f, 1f);
		}
		if (forwardInput < 0f || !HandleInput)
		{
			throttle = Mathf.Clamp(throttle - Time.deltaTime / throttleDownTime, 0f, 1f);
		}
		return throttle;
	}

	private Quaternion GeneratePlaneRotation(Vector3 velocity)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		if (!Grounded)
		{
			float num = CalculatePitchDown(velocity);
			xAxisTarget += num;
		}
		if (HandleInput)
		{
			yAxisTarget += MVInputWrapper.GetAxisRaw("Mouse X") * mouseSensitivity;
			yAxisTarget += angularSpeed * DirectInputMoveMap.x;
			xAxisTarget += (0f - MVInputWrapper.GetAxisRaw("Mouse Y")) * mouseSensitivity;
		}
		yAxisTarget = Mathf.Clamp(yAxisTarget, yAxis - 45f, yAxis + 45f);
		xAxisTarget = Mathf.Clamp(xAxisTarget, -45f, 60f);
		xAxisTarget = Mathf.Clamp(NormalizeAngle(xAxisTarget), (float)xMinLimit, (float)xMaxLimit);
		xAxis = Mathf.SmoothDampAngle(xAxis, xAxisTarget, ref xAxisVelocity, rotationSmoothTime);
		yAxis = Mathf.SmoothDampAngle(yAxis, yAxisTarget, ref yAxisVelocity, rotationSmoothTime);
		return Quaternion.Euler(xAxis, yAxis, 0f);
	}

	private float CalculatePitchDown(Vector3 velocity)
	{
		float magnitude = velocity.magnitude;
		return pitchDownFactor * Mathf.Clamp((velocityMinForPitchSteady - magnitude) / velocityMinForPitchSteady, 0f, 1f);
	}

	private static float NormalizeAngle(float angle)
	{
		while (angle < -180f)
		{
			angle += 360f;
		}
		while (angle > 180f)
		{
			angle -= 360f;
		}
		return angle;
	}
}
