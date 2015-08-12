using System;
using System.Collections.Generic;
using MV.Common;
using UnityEngine;

public class JetPackMotor : MVRigidBody
{
	private float velocityConstant = 40f;

	private float thrust = 1500f;

	private float thrustLeaveMode = 1000f;

	private float leaveModeForward = 4.5f;

	private KeyValuePair<float, float> leaveModeRotationRange = new KeyValuePair<float, float>(100f, 400f);

	private float runSpeed = 12f;

	private float speedSmoothing = 10f;

	private float speed;

	private AnimationCurve slopeSpeedMultiplier = new AnimationCurve(new Keyframe(-90f, 1f), new Keyframe(0f, 1f), new Keyframe(90f, 1f));

	private Vector3 velocityPrevFrame;

	private BounceState bounceState;

	private MVMovableMotorState movableMotorState;

	private ImpactState impactState = new ImpactState(RuntimeEventType.AvatarImpact25, RuntimeEventType.AvatarImpact50, RuntimeEventType.AvatarImpact75);

	private MVInteractableBase interactable;

	private MVInteractableBase vehicleInteractable;

	private float HARDCODEDJETPACKAIRFRICTION = 0.43f;

	private float waterProximity;

	private SmoothCharacterController smoothController;

	protected StuckEvaluator stuckEvaluator;

	private Vector3 lastKnownMovementDir = Vector3.zero;

	private float platformerRotSpeed = 5.8f;

	private bool leaveMode;

	private MvCharacterController Controller => smoothController.Controller;

	public bool IsUnderWater => WaterProximity > 0.5f;

	public override Vector3 Velocity => Controller.Velocity / Time.fixedDeltaTime;

	public Vector3 InputMoveDirection { get; set; }

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
				if (interactable is IMoveHitHandler)
				{
					IMoveHitHandler moveHitHandler = (IMoveHitHandler)interactable;
					MvCharacterController controller = Controller;
					controller.OnControllerColliderHit = (Action<MVControllerColliderHit>)Delegate.Remove(controller.OnControllerColliderHit, new Action<MVControllerColliderHit>(moveHitHandler.HandleMoveHit));
				}
				interactable = vehicleInteractable;
			}
		}
	}

	public override bool Grounded => groundState.Grounded;

	public float WaterProximity => waterProximity;

	public override bool IsMovementLocked { get; set; }

	protected override void SuspendImpactDamage()
	{
		impactState.SuspendImpactDamage();
	}

	public List<MVOverlapResult> GetOverlappingObjects()
	{
		return Controller.GetOverlappingObjects();
	}

	public bool IsStuck()
	{
		return stuckEvaluator.Update();
	}

	public void Init(AvatarInteractable interactableLocal, VehicleInteractable vehicleInteractable, SmoothCharacterController avatarController, float thrustStrength, float density)
	{
		Init();
		bounceState = new BounceState(interactableLocal);
		thrust = thrustStrength;
		base.density = density;
		smoothController = avatarController;
		HashSet<int> worldIDsRecursive = worldObjectParent.WorldIDsRecursive;
		Controller.IgnoreWoIds = worldIDsRecursive;
		stuckEvaluator = new StuckEvaluator(Controller.GetOverlappingObjects);
		movableMotorState = new MVMovableMotorState();
		interactable = interactableLocal;
		MvCharacterController controller = avatarController.Controller;
		controller.OnControllerColliderHit = (Action<MVControllerColliderHit>)Delegate.Combine(controller.OnControllerColliderHit, new Action<MVControllerColliderHit>(interactableLocal.HandleMoveHit));
		MvCharacterController controller2 = avatarController.Controller;
		controller2.OnControllerColliderHit = (Action<MVControllerColliderHit>)Delegate.Combine(controller2.OnControllerColliderHit, new Action<MVControllerColliderHit>(bounceState.HandleMoveHit));
		MvCharacterController controller3 = avatarController.Controller;
		controller3.OnControllerColliderHit = (Action<MVControllerColliderHit>)Delegate.Combine(controller3.OnControllerColliderHit, new Action<MVControllerColliderHit>(impactState.HandleMoveHit));
		this.vehicleInteractable = vehicleInteractable;
	}

	public void FrameUpdate()
	{
		smoothController.SmoothMove();
	}

	public void FixedUpdateFunction(Quaternion setQuaternion, bool shouldSetRotation)
	{
		waterProximity = MVGameController.WOCM.WaterPlaneManager.ComputeAvatarWaterProximity(Controller.gameObject.transform.position);
		if (IsMovementLocked)
		{
			return;
		}
		if (shouldSetRotation && GameDB.GameType == MVGameType.Classic)
		{
			Controller.transform.rotation = setQuaternion;
		}
		else if (GameDB.GameType == MVGameType.Platformer)
		{
			if (InputMoveDirection.sqrMagnitude > 0f)
			{
				lastKnownMovementDir = InputMoveDirection;
			}
			if (lastKnownMovementDir.sqrMagnitude > 0f)
			{
				Controller.transform.rotation = Quaternion.Lerp(Controller.transform.rotation, Quaternion.LookRotation(lastKnownMovementDir), platformerRotSpeed * Time.fixedDeltaTime);
			}
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
		velocityPrevFrame = Controller.Velocity / Time.fixedDeltaTime;
		if (!flag)
		{
			velocityPrevFrame -= movableVelocityVector;
		}
		DealImpactDamage(velocityPrevFrame, prevVelocity);
	}

	private Vector3 GetVelocity(Vector3 velocity, Vector3 baseVelocity)
	{
		velocity -= velocity * MathFunctions.Pow2(HARDCODEDJETPACKAIRFRICTION) * Time.deltaTime;
		velocity = ApplyInputVelocityChange(velocity, baseVelocity);
		if (!groundState.Grounded)
		{
			velocity = ApplyGravity(velocity, velocityPrevFrame, interactable);
		}
		velocity = bounceState.ApplyBounceVelocity(velocity);
		if (Thrust)
		{
			ApplyJetImpulse(thrust);
		}
		if (Thrust && LeaveMode)
		{
			ApplyJetImpulse(thrustLeaveMode);
			transform.RotateAround(Controller.Center + transform.position, Vector3.up, UnityEngine.Random.Range(leaveModeRotationRange.Key, leaveModeRotationRange.Value) * Time.deltaTime);
			velocity += transform.rotation * Vector3.forward * UnityEngine.Random.Range(0f, leaveModeForward);
		}
		velocity = GetImpulse(velocity, interactable);
		velocity = MVRigidBody.VelocityDamping(velocity, 1f, interactable);
		return velocity;
	}

	private void ApplyJetImpulse(float jetpackThrust)
	{
		float num = Mathf.Clamp(Velocity.y, 0f, velocityConstant);
		float num2 = (velocityConstant - num) / velocityConstant;
		AddImpulse(Vector3.up * jetpackThrust * num2 * Time.fixedDeltaTime);
	}

	protected void DealImpactDamage(Vector3 curVelocity, Vector3 prevVelocity)
	{
		float num = impactState.UpdateImpactState(curVelocity, prevVelocity, interactable);
		if (num != 0f)
		{
			interactable.TakeDamage(num, null, PlayerKilledByType.Impact);
		}
	}

	private GroundChange Move(Vector3 velocity, Vector3 basevelocity)
	{
		Vector3 motion = (velocity + basevelocity) * Time.deltaTime;
		collisionFlags = Controller.Move(motion);
		groundState.Update(Controller, velocity);
		return GroundChange.UnChanged;
	}

	private Vector3 ApplyInputVelocityChange(Vector3 velocity, Vector3 baseVelocity)
	{
		Vector3 vector = GetDesiredHorizontalVelocity();
		if (groundState.Grounded && !Thrust)
		{
			vector = MVRigidBody.AdjustGroundVelocityToNormal(vector, groundState.GroundNormal) + baseVelocity;
		}
		Vector3 vector2 = (vector - velocity) * (Time.fixedDeltaTime / 0.02f);
		vector2 *= MathFunctions.Pow2(HARDCODEDJETPACKAIRFRICTION);
		velocity += vector2;
		return velocity;
	}

	private Vector3 GetDesiredHorizontalVelocity()
	{
		float baseValue = runSpeed;
		baseValue = interactable.HandleModifierEffect(AvatarModifierEffect.Speed, baseValue);
		float to = InputMoveDirection.magnitude * baseValue;
		float t = speedSmoothing * Time.deltaTime;
		speed = Mathf.Lerp(speed, to, t);
		if (InputMoveDirection.magnitude == 0f)
		{
			speed = 0f;
		}
		if (groundState.Grounded)
		{
			float time = Mathf.Asin(velocityPrevFrame.normalized.y) * 57.29578f;
			speed *= slopeSpeedMultiplier.Evaluate(time);
		}
		return InputMoveDirection * speed;
	}
}
