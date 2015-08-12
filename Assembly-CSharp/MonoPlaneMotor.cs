using System;
using MV.Common;
using UnityEngine;

public class MonoPlaneMotor : SimpleVehicleMotorBase
{
	private ImpactStateMonoPlane impactState = new ImpactStateMonoPlane();

	private Vector3 velocityPrevFrame;

	private BounceState bounceState;

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

	public override Vector3 Velocity => velocityPrevFrame;

	public override bool Grounded => groundState.Grounded;

	public override bool IsMovementLocked { get; set; }

	protected override void SuspendImpactDamage()
	{
		impactState.SuspendImpactDamage();
	}

	public override void Init(SmoothCharacterController smoothController, VehicleInteractable interactableLocal)
	{
		base.Init(smoothController, interactableLocal);
		bounceState = new BounceState(interactableLocal);
		xAxisVelocity = (yAxisVelocity = 0f);
		xAxis = (xAxisTarget = Controller.transform.eulerAngles.x);
		yAxis = (yAxisTarget = Controller.transform.eulerAngles.y);
		MvCharacterController controller = smoothController.Controller;
		controller.OnControllerColliderHit = (Action<MVControllerColliderHit>)Delegate.Combine(controller.OnControllerColliderHit, new Action<MVControllerColliderHit>(bounceState.HandleMoveHit));
		MvCharacterController controller2 = smoothController.Controller;
		controller2.OnControllerColliderHit = (Action<MVControllerColliderHit>)Delegate.Combine(controller2.OnControllerColliderHit, new Action<MVControllerColliderHit>(impactState.HandleMoveHit));
	}

	public override void Reset()
	{
		base.Reset();
		impactState.prevVelocityChangeVector = Vector3.zero;
		velocityPrevFrame = Vector3.zero;
		Controller.Velocity = Vector3.zero;
	}

	public override void VehicleUpdateFunction()
	{
		Vector3 prevVelocity = velocityPrevFrame;
		Vector3 velocity = velocityPrevFrame;
		bool flag = movableMotorState.Move(velocity, Controller, Controller.Radius, groundState, out var movableVelocityVector);
		velocity = GetVehicleVelocity(velocity, movableVelocityVector);
		if (flag)
		{
			Move(velocity, Vector3.zero);
		}
		else
		{
			Move(velocity, movableVelocityVector);
		}
		velocityPrevFrame = Controller.Velocity / Time.fixedDeltaTime;
		if (!flag)
		{
			velocityPrevFrame -= movableVelocityVector;
		}
		DealImpactDamage(velocityPrevFrame, prevVelocity);
	}

	protected void DealImpactDamage(Vector3 curVelocity, Vector3 prevVelocity)
	{
		float num = impactState.UpdateImpactState(curVelocity, prevVelocity, interactableLocal);
		if (num != 0f)
		{
			interactableLocal.TakeDamage(num, null, PlayerKilledByType.Impact);
		}
	}

	private void Move(Vector3 velocity, Vector3 basevelocity)
	{
		Vector3 motion = (velocity + basevelocity) * Time.deltaTime;
		collisionFlags = Controller.Move(motion);
		groundState.Update(Controller, velocity);
	}

	private Vector3 GetVehicleVelocity(Vector3 velocity, Vector3 baseVelocity)
	{
		if (groundChange == GroundChange.UnChanged)
		{
			Debug.Log("get rid of me");
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
		if (groundState.Grounded)
		{
			velocity -= velocity * MathFunctions.Pow2(interactableLocal.HandleModifierEffect(AvatarModifierEffect.Friction, groundState.GroundMaterial.physicalProperties.friction)) * Time.deltaTime;
		}
		else
		{
			velocity -= velocity * MathFunctions.Pow2(interactableLocal.HandleModifierEffect(AvatarModifierEffect.Friction, HARDCODEDJETPACKAIRFRICTION)) * Time.deltaTime;
		}
		velocity = GetVehicleInputVelocity(velocity);
		velocity = bounceState.ApplyBounceVelocity(velocity);
		velocity = GetImpulse(velocity, interactableLocal);
		velocity = MVRigidBody.VelocityDamping(velocity, 1f, interactableLocal);
		return velocity;
	}

	private float WaterProximity()
	{
		WaterPlaneManager waterPlaneManager = MVGameController.WOCM.WaterPlaneManager;
		return waterPlaneManager.ComputeAvatarWaterProximity(gameObject.transform.position + Vector3.up * waterOffset);
	}

	protected Vector3 ApplyWaterGravity(Vector3 velocity, float waterProximity)
	{
		velocity.y += waterDownVelocity * Time.deltaTime * waterProximity;
		if (velocity.y > 0f)
		{
			velocity.y = Mathf.Clamp(velocity.y, 0f, maxUnderWaterYMovement);
		}
		return velocity;
	}

	private Vector3 GetVehicleInputVelocity(Vector3 velocity)
	{
		planeRotation = GeneratePlaneRotation(velocity);
		transform.rotation = Quaternion.Euler(0f, yAxis, 0f);
		VehicleCamera.lookAt.localRotation = Quaternion.Euler(xAxis, 0f, 0f);
		throttle = UpdateThrottle(DirectInputMoveMap.z);
		Vector3 vector = planeRotation * Vector3.forward;
		Vector3 vector2 = planeRotation * Vector3.up;
		Vector3 vector3 = planeRotation * Vector3.right;
		Vector3 position = Controller.transform.position;
		Debug.DrawLine(position, position + vector, Color.blue, 0f, depthTest: false);
		Debug.DrawLine(position, position + vector2, Color.green, 0f, depthTest: false);
		Debug.DrawLine(position, position + vector3, Color.red, 0f, depthTest: false);
		Vector3 vector4 = CalcDrag(velocity);
		Vector3 vector5 = vector * thrustFactor * throttle;
		Vector3 vector6 = (vector5 + vector4) / mass;
		velocity += vector6 * Time.fixedDeltaTime;
		return velocity;
	}

	protected Vector3 HandleLift(Vector3 velocity, MVInteractableBase interactableLocal)
	{
		float num = Mathf.Clamp(velocity.magnitude / velocityFactor, 0f, 1f);
		velocity.y += num * (float)MVPhysics.Gravity * interactableLocal.HandleModifierEffect(AvatarModifierEffect.Density, density) * Time.deltaTime;
		return velocity;
	}

	private Vector3 CalcDrag(Vector3 velocity)
	{
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
		xAxisTarget = Mathf.Clamp(NormalizeAngle(xAxisTarget), xMinLimit, xMaxLimit);
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
