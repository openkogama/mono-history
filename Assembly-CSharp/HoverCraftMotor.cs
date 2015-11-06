using System;
using MV.Common;
using UnityEngine;

public class HoverCraftMotor : SimpleVehicleMotorBase
{
	private const float verticalThrustTime = 0.6f;

	private const float stoppedJumpingTimeOut = 0.3f;

	private ImpactState impactState = new ImpactState(RuntimeEventType.VehicleImpact25, RuntimeEventType.VehicleImpact50, RuntimeEventType.VehicleImpact75);

	private Vector3 velocityPrevFrame;

	private BounceState bounceState;

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

	private float hullRotationFactorClassic = 20f;

	private float hullRotationFactorPlatformer = 50f;

	private float extraThrustFactor = 0.8f;

	private float maxUnderWaterYMovement = 40f;

	private float recalibrateCameraFactor = 1.25f;

	private float platformerRotSpeed = 180f;

	private Vector3 lastKnownMovementDir = Vector3.zero;

	private bool started;

	private float startTime;

	private float interval = 5f;

	private float f;

	private float driftCorrectionRotation = 10f;

	private float availableVerticalThrustTime = 0.6f;

	private bool wasJumping;

	private float stoppedJumpingTime = Time.time - 0.3f;

	private float regenerationFactor = 0.6f;

	private bool isVerticalThrusting;

	private float jumpForce = 4f;

	public IVehicleCamera VehicleCamera { private get; set; }

	public override Vector3 Velocity => velocityPrevFrame;

	public override bool Grounded => groundState.Grounded;

	public override bool IsMovementLocked { get; set; }

	protected override void SuspendImpactDamage()
	{
		impactState.SuspendImpactDamage();
	}

	public override void Init(SmoothCharacterController characterController, VehicleInteractable interactableLocal)
	{
		base.Init(characterController, interactableLocal);
		bounceState = new BounceState(interactableLocal);
		MvCharacterController controller = characterController.Controller;
		controller.OnControllerColliderHit = (Action<MVControllerColliderHit>)Delegate.Combine(controller.OnControllerColliderHit, new Action<MVControllerColliderHit>(bounceState.HandleMoveHit));
		MvCharacterController controller2 = characterController.Controller;
		controller2.OnControllerColliderHit = (Action<MVControllerColliderHit>)Delegate.Combine(controller2.OnControllerColliderHit, new Action<MVControllerColliderHit>(impactState.HandleMoveHit));
		density = 1.3f;
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
		Controller.Move(motion);
		groundState.Update(Controller, velocity);
	}

	private Vector3 GetVehicleVelocity(Vector3 velocity, Vector3 baseVelocity)
	{
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
		velocity = MVRigidBody.VelocityDamping(velocity, 1f, interactableLocal);
		return velocity;
	}

	private float WaterProximity()
	{
		WaterPlaneManager waterPlaneManager = MVGameControllerBase.WaterPlaneManager;
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
		return MVGameControllerBase.Game.GameType switch
		{
			MVGameType.Classic => GetVehicleInputVelocityClassicCam(velocity), 
			MVGameType.Platformer => GetVehicleInputVelocityPlatformerCam(velocity), 
			_ => GetVehicleInputVelocityClassicCam(velocity), 
		};
	}

	private bool CanRotate()
	{
		float b = 10f;
		f = Mathf.Lerp(f, b, Time.deltaTime);
		if (!started && Input.GetKey(KeyCode.D))
		{
			startTime = Time.fixedTime;
			started = true;
		}
		if (Time.fixedTime - startTime > interval)
		{
			return false;
		}
		return true;
	}

	private Vector3 GetVehicleInputVelocityClassicCam(Vector3 velocity)
	{
		velocity = HoverCraftFrictionXZ(velocity);
		if (HandleInput)
		{
			Controller.transform.Rotate(Vector3.up, 57.29578f * Time.fixedDeltaTime * angularSpeed * DirectInputMoveMap.x * Mathf.Abs(MVInputWrapper.GetAxis("Horizontal")), Space.World);
		}
		if (Mathf.Abs(DirectInputMoveMap.z) > 0f || (double)Mathf.Abs(DirectInputMoveMap.x) > 0.0)
		{
			Quaternion quaternion = Quaternion.Euler(0f, VehicleCamera.RotationAroundY, 0f);
			Quaternion quaternion2 = Quaternion.Slerp(Controller.transform.rotation, Controller.transform.rotation * quaternion, Time.fixedDeltaTime * recalibrateCameraFactor);
			float num = Quaternion.Angle(quaternion2, Controller.transform.rotation);
			Controller.transform.rotation = quaternion2;
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
		directInputMoveMap = Controller.transform.rotation * directInputMoveMap;
		if (groundState.Grounded)
		{
			directInputMoveMap = MVRigidBody.AdjustGroundVelocityToNormal(directInputMoveMap, groundState.GroundNormal);
		}
		Vector3 normalized = velocity.normalized;
		float sqrMagnitude = velocity.sqrMagnitude;
		Vector3 vector = VerticalDrag(normalized, sqrMagnitude);
		Vector3 vector2 = XZDrag(normalized, sqrMagnitude);
		Vector3 vector3 = HullRotationDrag(normalized, sqrMagnitude, hullRotationFactorClassic);
		Vector3 vector4 = vector + vector2 + vector3;
		Vector3 vector5 = directInputMoveMap * interactableLocal.HandleModifierEffect(AvatarModifierEffect.Speed, thrustFactor);
		Vector3 vector6 = Controller.transform.rotation * Vector3.forward * vector3.magnitude * extraThrustFactor;
		Vector3 vector7 = (vector5 + vector4 + vector6) / mass;
		velocity += vector7 * Time.fixedDeltaTime;
		velocity = HandleVerticalThrust(velocity);
		return velocity;
	}

	private Vector3 GetVehicleInputVelocityPlatformerCam(Vector3 velocity)
	{
		velocity = HoverCraftFrictionXZ(velocity);
		Vector3 vector = DirectInputMoveMap;
		vector.Normalize();
		if (groundState.Grounded)
		{
			vector = MVRigidBody.AdjustGroundVelocityToNormal(vector, groundState.GroundNormal);
		}
		Vector3 normalized = velocity.normalized;
		float sqrMagnitude = velocity.sqrMagnitude;
		Vector3 vector2 = VerticalDrag(normalized, sqrMagnitude);
		Vector3 vector3 = XZDrag(normalized, sqrMagnitude);
		Vector3 vector4 = HullRotationDrag(normalized, sqrMagnitude, hullRotationFactorPlatformer);
		Vector3 vector5 = vector2 + vector3 + vector4;
		Vector3 vector6 = vector * interactableLocal.HandleModifierEffect(AvatarModifierEffect.Speed, thrustFactor);
		Vector3 vector7 = Controller.transform.rotation * vector * extraThrustFactor;
		Vector3 vector8 = (vector6 + vector5 + vector7) / mass;
		velocity += vector8 * Time.fixedDeltaTime;
		velocity = HandleVerticalThrust(velocity);
		if (vector.magnitude > 0.999f)
		{
			lastKnownMovementDir = vector;
		}
		if (lastKnownMovementDir.magnitude > 0.999f)
		{
			FixedRotationSpeedPlatformer(lastKnownMovementDir);
		}
		velocity = PlatformerDriftCorrection(velocity, lastKnownMovementDir);
		return velocity;
	}

	private Vector3 PlatformerDriftCorrection(Vector3 velocity, Vector3 targetDir)
	{
		Vector3 vec = velocity;
		vec.y = 0f;
		float magnitude = vec.magnitude;
		vec.Normalize();
		vec = RotateTowardsAroundY(vec, targetDir, driftCorrectionRotation);
		vec *= magnitude;
		velocity.x = vec.x;
		velocity.z = vec.z;
		return velocity;
	}

	public static Vector3 RotateTowardsAroundY(Vector3 vec, Vector3 target, float speedInDegrees)
	{
		float num = 57.29578f * MathFunctions.SignedAngle(vec, target, Vector3.up);
		float num2 = Mathf.Abs(num);
		float num3 = speedInDegrees * Time.deltaTime;
		if (num3 > num2)
		{
			num3 = num2;
		}
		float num4 = 1f;
		if (num < 0f)
		{
			num4 = -1f;
		}
		Quaternion quaternion = Quaternion.Euler(0f, num4 * num3, 0f);
		return quaternion * vec;
	}

	private float GetDriftCorrectValue(float val)
	{
		float num = Mathf.Abs(val);
		float num2 = Mathf.Max(0f, num - driftCorrectionRotation * Time.deltaTime);
		if (val < 0f)
		{
			return 0f - num2;
		}
		return num2;
	}

	private void FixedRotationSpeedPlatformer(Vector3 targetDir)
	{
		float num = 57.29578f * MathFunctions.SignedAngle(Controller.transform.rotation * Vector3.forward, targetDir, Vector3.up);
		float num2 = Mathf.Abs(num);
		float num3 = 1f;
		if (num < 0f)
		{
			num3 = -1f;
		}
		float num4 = platformerRotSpeed * Time.fixedDeltaTime;
		if (num4 > num2)
		{
			num4 = num2;
		}
		Controller.transform.Rotate(Vector3.up, num4 * num3, Space.World);
	}

	private Vector3 HoverCraftFrictionXZ(Vector3 velocity)
	{
		Vector3 vector = velocity;
		vector.y = 0f;
		float magnitude = vector.magnitude;
		if (magnitude < 0.001f)
		{
			velocity.x = 0f;
			velocity.z = 0f;
			return velocity;
		}
		float num = 1f / (magnitude / magnitudeDivider + 1f);
		Vector3 vector2 = vector.normalized * num * frictionFactor * Time.deltaTime;
		if (vector2.sqrMagnitude > vector.sqrMagnitude)
		{
			vector2 = vector;
		}
		velocity -= vector2;
		return velocity;
	}

	private Vector3 XZDrag(Vector3 velocityNormal, float velocitySquareMagnitude)
	{
		velocityNormal.y = 0f;
		Vector3 vector = velocityNormal;
		vector *= dragCoefficientXZ;
		return vector * velocitySquareMagnitude;
	}

	private Vector3 HullRotationDrag(Vector3 velocityNormal, float velocitySquareMagnitude, float hullRotationFactor)
	{
		velocityNormal.y = 0f;
		float num = DragCoefficientXZHullRotationFactor(velocityNormal, hullRotationFactor);
		Vector3 vector = velocityNormal;
		vector *= 0f - num;
		return vector * velocitySquareMagnitude;
	}

	private Vector3 VerticalDrag(Vector3 velocityNormal, float velocitySquareMagnitude)
	{
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
			velocity += Vector3.up * jumpForce;
		}
		wasJumping = true;
		return velocity;
	}

	private float DragCoefficientXZHullRotationFactor(Vector3 velocity, float hullRotationFactor)
	{
		velocity.Normalize();
		return Mathf.Abs(Vector3.Dot(Controller.transform.rotation * Vector3.right, velocity)) * hullRotationFactor;
	}
}
