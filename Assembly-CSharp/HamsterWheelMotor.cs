using System;
using MV.Common;
using UnityEngine;

public class HamsterWheelMotor : SimpleVehicleMotorBase
{
	private ImpactState impactState = new ImpactState(RuntimeEventType.VehicleImpact25, RuntimeEventType.VehicleImpact50, RuntimeEventType.VehicleImpact75);

	private HamsterWheelBounceState bounceState;

	private JumpState jumpState;

	private Vector3 curVelocity = Vector3.zero;

	private float speed;

	private float maxSpeed = 38.8f;

	private float minSpeed = -10f;

	private const float accelerationSpeed = 48f;

	private float angularSpeed = 1.4f;

	private float recalibrateCameraFactor = 1.05f;

	private float waterProximityThresshold = 0.01f;

	private float waterDownVelocity = 50f;

	private float waterOffset = -0.6f;

	private float maxUnderWaterYMovement = 40f;

	private float platformerRotSpeed = 4.4f;

	private Vector3 lastKnownMovementDir = Vector3.zero;

	private Vector3 platformerSpeed = Vector3.zero;

	private float platformerSpeedFactor = 0.75f;

	private MVInteractableBase interactable;

	public IVehicleCamera VehicleCamera { private get; set; }

	public override bool Grounded => groundState.Grounded;

	public override Vector3 Velocity => curVelocity;

	public override bool IsMovementLocked { get; set; }

	public override void Init(SmoothCharacterController smoothCharacterController, VehicleInteractable interactableLocal)
	{
		density = 1.5f;
		base.Init(smoothCharacterController, interactableLocal);
		bounceState = new HamsterWheelBounceState(interactableLocal);
		interactable = interactableLocal;
		MvCharacterController controller = smoothCharacterController.Controller;
		controller.OnControllerColliderHit = (Action<MVControllerColliderHit>)Delegate.Combine(controller.OnControllerColliderHit, new Action<MVControllerColliderHit>(bounceState.HandleMoveHit));
		MvCharacterController controller2 = smoothCharacterController.Controller;
		controller2.OnControllerColliderHit = (Action<MVControllerColliderHit>)Delegate.Combine(controller2.OnControllerColliderHit, new Action<MVControllerColliderHit>(impactState.HandleMoveHit));
		jumpState = new JumpState(3f);
		MVGroundState mVGroundState = groundState;
		mVGroundState.OnGroundChange = (Action<GroundChange>)Delegate.Combine(mVGroundState.OnGroundChange, new Action<GroundChange>(jumpState.UpdateJumpState));
		MvCharacterController controller3 = smoothCharacterController.Controller;
		controller3.OnControllerColliderHit = (Action<MVControllerColliderHit>)Delegate.Combine(controller3.OnControllerColliderHit, new Action<MVControllerColliderHit>(jumpState.HandleMoveHit));
	}

	public override void VehicleUpdateFunction()
	{
		Vector3 vector = curVelocity;
		Vector3 velocity = curVelocity;
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
		curVelocity = Controller.Velocity / Time.fixedDeltaTime;
		if (!flag)
		{
			curVelocity -= movableVelocityVector;
		}
		DealImpactDamage(curVelocity * 0.5f, vector * 0.5f);
	}

	public override void Reset()
	{
		base.Reset();
		impactState.prevVelocityChangeVector = Vector3.zero;
		curVelocity = Vector3.zero;
		Controller.Velocity = Vector3.zero;
	}

	protected override void SuspendImpactDamage()
	{
		impactState.SuspendImpactDamage();
	}

	private Vector3 GetVehicleVelocity(Vector3 velocity, Vector3 baseVelocity)
	{
		return MVGameControllerBase.Game.GameType switch
		{
			MVGameType.Classic => GetVehicleVelocityClassicCam(velocity, baseVelocity), 
			MVGameType.Platformer => GetVehicleVelocityPlatformerCam(velocity, baseVelocity), 
			_ => GetVehicleVelocityClassicCam(velocity, baseVelocity), 
		};
	}

	private Vector3 GetVehicleVelocityClassicCam(Vector3 velocity, Vector3 movableVelocity)
	{
		float num = interactable.HandleModifierEffect(AvatarModifierEffect.Speed, maxSpeed);
		velocity -= (velocity - 0.98f * velocity) * (Time.fixedDeltaTime / 0.02f);
		if (HandleInput)
		{
			Controller.transform.Rotate(Vector3.up, Time.fixedDeltaTime * angularSpeed * MVInputWrapper.GetAxisWithoutSensitivity("Horizontal") * 57.29578f, Space.World);
		}
		if (Mathf.Abs(DirectInputMoveMap.z) > 0f || (double)Mathf.Abs(DirectInputMoveMap.x) > 0.0)
		{
			Quaternion quaternion = Quaternion.Euler(0f, VehicleCamera.RotationAroundY, 0f);
			Quaternion quaternion2 = Quaternion.Slerp(Controller.transform.rotation, Controller.transform.rotation * quaternion, Time.fixedDeltaTime * recalibrateCameraFactor);
			float num2 = Quaternion.Angle(quaternion2, Controller.transform.rotation);
			Controller.transform.rotation = quaternion2;
			if (VehicleCamera.RotationAroundY < 0f)
			{
				VehicleCamera.RotationAroundY += num2;
			}
			else
			{
				VehicleCamera.RotationAroundY -= num2;
			}
		}
		if (DirectInputMoveMap.z > 0f)
		{
			if (speed < num)
			{
				speed += 48f * Time.fixedDeltaTime;
			}
			if (speed > num)
			{
				speed = num;
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
		Vector3 vector = transform.forward * speed * Time.fixedDeltaTime;
		velocity += vector;
		float num3 = WaterProximity();
		velocity = ((!(num3 > waterProximityThresshold)) ? ApplyGravity(velocity, curVelocity, interactableLocal) : ApplyWaterGravity(velocity, num3));
		velocity = bounceState.ApplyBounceVelocityMaterials(velocity);
		velocity = jumpState.ApplyJumping(interactableLocal, groundState, density, 0f, Jump, velocity, movableVelocity);
		velocity = GetImpulse(velocity, interactableLocal);
		return velocity;
	}

	private Vector3 GetVehicleVelocityPlatformerCam(Vector3 velocity, Vector3 movableVelocity)
	{
		velocity -= (velocity - 0.98f * velocity) * (Time.fixedDeltaTime / 0.02f);
		float num = interactable.HandleModifierEffect(AvatarModifierEffect.Speed, maxSpeed);
		if (DirectInputMoveMap.sqrMagnitude > 0f)
		{
			lastKnownMovementDir = DirectInputMoveMap;
		}
		if (lastKnownMovementDir.sqrMagnitude > 0f)
		{
			Controller.transform.rotation = Quaternion.Lerp(Controller.transform.rotation, Quaternion.LookRotation(lastKnownMovementDir), platformerRotSpeed * Time.fixedDeltaTime);
		}
		platformerSpeed.y = 0f;
		if (DirectInputMoveMap.x > 0f)
		{
			if (platformerSpeed.x < num)
			{
				platformerSpeed.x += 48f * Time.fixedDeltaTime;
			}
		}
		else if (DirectInputMoveMap.x < 0f)
		{
			if (platformerSpeed.x > 0f - num)
			{
				platformerSpeed.x -= 48f * Time.fixedDeltaTime;
			}
		}
		else
		{
			platformerSpeed.x = 0f;
		}
		if (DirectInputMoveMap.z > 0f)
		{
			if (platformerSpeed.z < num)
			{
				platformerSpeed.z += 48f * Time.fixedDeltaTime;
			}
		}
		else if (DirectInputMoveMap.z < 0f)
		{
			if (platformerSpeed.z > 0f - num)
			{
				platformerSpeed.z -= 48f * Time.fixedDeltaTime;
			}
		}
		else
		{
			platformerSpeed.z = 0f;
		}
		velocity += platformerSpeed * platformerSpeedFactor * Time.fixedDeltaTime;
		float num2 = WaterProximity();
		velocity = ((!(num2 > waterProximityThresshold)) ? ApplyGravity(velocity, curVelocity, interactableLocal) : ApplyWaterGravity(velocity, num2));
		velocity = bounceState.ApplyBounceVelocityMaterials(velocity);
		velocity = jumpState.ApplyJumping(interactableLocal, groundState, density, 0f, Jump, velocity, movableVelocity);
		velocity = GetImpulse(velocity, interactableLocal);
		return velocity;
	}

	private void DealImpactDamage(Vector3 curVelocity, Vector3 prevVelocity)
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
}
