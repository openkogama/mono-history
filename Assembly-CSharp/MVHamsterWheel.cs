using System;
using System.Collections.Generic;
using MV.Common;
using MV.WorldObject;
using MV.WorldObject.RuntimeEvents;
using UnityEngine;

public class MVHamsterWheel(Dictionary<object, object> data, Dictionary<int, MVWorldObjectClient> worldObjects) : MVSimpleOneSeatVehicle(data, PrefabPool.Instance.MVHamsterWheelPrefab, worldObjects)
{
	protected class LocalObjectsHamsterWheel : LocalObjectsSimpleVehicle
	{
		public LocalObjectsHamsterWheel(MVHamsterWheel vehicle, SmoothCharacterController smoothController, SimpleVehicleMotorBase hamsterWheelMotor)
			: base(vehicle, smoothController, hamsterWheelMotor)
		{
		}

		public override void Leave()
		{
			base.Leave();
			(Owner as MVHamsterWheel).IsMovingForward.Value = false;
			(Owner as MVHamsterWheel).IsMovingBackwards.Value = false;
		}

		public override IInputToPlayerMovement FixedUpdate(IInputToPlayerMovement movementMap)
		{
			HamsterWheelMotor hamsterWheelMotor = vehicleMotor as HamsterWheelMotor;
			MVHamsterWheel mVHamsterWheel = Owner as MVHamsterWheel;
			if (hamsterWheelMotor.DirectInputMoveMap.z > 0f && !mVHamsterWheel.wasMovingForward)
			{
				mVHamsterWheel.IsMovingForward.Value = true;
				mVHamsterWheel.wasMovingForward = true;
			}
			if (hamsterWheelMotor.DirectInputMoveMap.z <= 0f && mVHamsterWheel.wasMovingForward)
			{
				mVHamsterWheel.IsMovingForward.Value = false;
				mVHamsterWheel.wasMovingForward = false;
			}
			if (hamsterWheelMotor.DirectInputMoveMap.z < 0f && !mVHamsterWheel.wasMovingBackwards)
			{
				mVHamsterWheel.IsMovingBackwards.Value = true;
				mVHamsterWheel.wasMovingBackwards = true;
			}
			if (hamsterWheelMotor.DirectInputMoveMap.z >= 0f && mVHamsterWheel.wasMovingBackwards)
			{
				mVHamsterWheel.IsMovingBackwards.Value = false;
				mVHamsterWheel.wasMovingBackwards = false;
			}
			if ((vehicleMotor as HamsterWheelMotor).Grounded && !mVHamsterWheel.wasGrounded)
			{
				mVHamsterWheel.IsGrounded.Value = true;
				mVHamsterWheel.wasGrounded = true;
			}
			if (!(vehicleMotor as HamsterWheelMotor).Grounded && mVHamsterWheel.wasGrounded)
			{
				mVHamsterWheel.IsGrounded.Value = false;
				mVHamsterWheel.wasGrounded = false;
			}
			return base.FixedUpdate(movementMap);
		}
	}

	private const string _vehiclePrefab = "Prefabs/Blueprints/Vehicles/HamsterWheel";

	private float deathExplosionDamageValue = 40f;

	private float deathExplosionRadius = 10f;

	private float deathExplosionImpulse = 2000f;

	public MVRuntimeDataVariable IsMovingForward;

	private bool wasMovingForward;

	public MVRuntimeDataVariable IsMovingBackwards;

	private bool wasMovingBackwards;

	public MVRuntimeDataVariable IsGrounded;

	private bool wasGrounded;

	public override bool IsDead => (bool)IsVehicleDead.Value;

	public override void Initialize()
	{
		IsMovingForward = RuntimeDataVariables.New("isMovingForward", 2f, writeThrough: false);
		IsMovingBackwards = RuntimeDataVariables.New("isMovingBackwards", 2f, writeThrough: false);
		IsGrounded = RuntimeDataVariables.New("isGrounded", 2f, writeThrough: false);
		base.Initialize();
		if (!IsInSpawner)
		{
			gameObject.AddComponent<InteractionDataHandler>();
		}
		MVRuntimeDataVariable isVehicleDead = IsVehicleDead;
		isVehicleDead.OnChange = (MVRuntimeDataVariable.OnChangeDelegate)Delegate.Combine(isVehicleDead.OnChange, new MVRuntimeDataVariable.OnChangeDelegate(OnIsDeadChange));
		HamsterWheelVisualization component = gameObject.GetComponent<HamsterWheelVisualization>();
		component.Init(seatManager, (float)RuntimeVariablesRepository.GetRuntimeVariables(WorldObjectType)["health"], Health, IsMovingForward, IsMovingBackwards, IsGrounded, IsInSpawner);
		visualization = component;
	}

	public override void InitializeInventory()
	{
		base.InitializeInventory();
		visualization = gameObject.GetComponent<HamsterWheelVisualization>();
		visualization.enabled = false;
	}

	private void OnIsDeadChange(object isDead)
	{
		if ((bool)isDead)
		{
			Debug.Log("Hamster wheel has died");
			HashSet<int> worldIDsRecursive = WorldIDsRecursive;
			Vector3 vector = Vector3.down + gameObject.transform.rotation * Vector3.forward;
			if (localObjects == null)
			{
				SharedWorldObjectGameplayFunctions.Explosion.Explode(PrefabPool.Instance.ParticleExplosion, gameObject.transform.position + vector, deathExplosionDamageValue, deathExplosionRadius, deathExplosionImpulse, local: true, null, worldIDsRecursive);
				return;
			}
			ExplosionEvent explosionEvent = new ExplosionEvent(RuntimeEventType.Bazooka, gameObject.transform.position + vector);
			SharedWorldObjectGameplayFunctions.Explosion.Explode(PrefabPool.Instance.ParticleExplosion, gameObject.transform.position + vector, deathExplosionDamageValue, deathExplosionRadius, deathExplosionImpulse, local: false, explosionEvent, worldIDsRecursive);
		}
	}

	public override Bounds GetLocalBounds(BoundsContext boundsContext)
	{
		return new Bounds(Vector3.zero, Vector3.one * 2f);
	}

	protected override LocalObjectsBase CreateLocalObjects(int seatID, MVAvatarLocal vehicleUser)
	{
		SmoothCharacterController smoothCharacterController = gameObject.AddComponent<SmoothCharacterController>();
		smoothCharacterController.Init(gameObject);
		smoothCharacterController.Controller.Init(1.5f, 3f, Vector3.up * 0.5f);
		smoothCharacterController.Controller.IgnoreWoIds = WorldIDsRecursive;
		HamsterWheelMotor hamsterWheelMotor = gameObject.AddComponent<HamsterWheelMotor>();
		MVCameraBase camera = seatManager.seats[seatID].Camera;
		if (!(camera is IVehicleCamera))
		{
			Debug.LogError("Expected camera type is VehicleCamera.");
			return null;
		}
		hamsterWheelMotor.VehicleCamera = (IVehicleCamera)camera;
		return new LocalObjectsHamsterWheel(this, smoothCharacterController, hamsterWheelMotor);
	}
}
