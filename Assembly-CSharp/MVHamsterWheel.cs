using System;
using System.Collections;
using System.Collections.Generic;
using MV.WorldObject;
using UnityEngine;

public class MVHamsterWheel(Hashtable data, Dictionary<int, MVWorldObjectClient> worldObjects) : MVSimpleOneSeatVehicle(data, "Prefabs/Blueprints/Vehicles/HamsterWheel", worldObjects)
{
	protected class LocalObjectsHamsterWheel : LocalObjectsSimpleVehicle
	{
		public LocalObjectsHamsterWheel(MVHamsterWheel vehicle, MvCharacterController controller, SimpleVehicleMotorBase hamsterWheelMotor)
			: base(vehicle, controller, hamsterWheelMotor)
		{
		}

		public override void Leave()
		{
			base.Leave();
			(Owner as MVHamsterWheel).IsMovingForward.Value = false;
			(Owner as MVHamsterWheel).IsMovingBackwards.Value = false;
		}

		public override MovementMap Update(MovementMap movementMap)
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
			return base.Update(movementMap);
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
		((Behaviour)visualization).enabled = false;
	}

	private void OnIsDeadChange(object isDead)
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		if ((bool)isDead)
		{
			Debug.Log((object)"Hamster wheel has died");
			HashSet<int> worldIDsRecursive = WorldIDsRecursive;
			Vector3 val = Vector3.down + gameObject.transform.rotation * Vector3.forward;
			MVGameController.Instance.WOCM.SharedWorldObjectGameplayFunctions.ExplosionCreator.Explode(gameObject.transform.position + val, deathExplosionDamageValue, deathExplosionRadius, deathExplosionImpulse, worldIDsRecursive);
		}
	}

	public override Bounds GetLocalBounds(BoundsContext boundsContext)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		return new Bounds(Vector3.zero, Vector3.one * 2f);
	}

	protected override LocalObjectsBase CreateLocalObjects(int seatID, MVAvatarLocal vehicleUser)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		MvCharacterController mvCharacterController = gameObject.AddComponent<MvCharacterController>();
		mvCharacterController.Init(1.5f, 3f, Vector3.up * 0.5f);
		mvCharacterController.IgnoreWoIds = WorldIDsRecursive;
		HamsterWheelMotor hamsterWheelMotor = gameObject.AddComponent<HamsterWheelMotor>();
		MVCameraBase camera = seatManager.seats[seatID].Camera;
		if (!(camera is VehicleCamera))
		{
			Debug.LogError((object)"Expected camera type is VehicleCamera.");
			return null;
		}
		hamsterWheelMotor.VehicleCamera = (VehicleCamera)camera;
		return new LocalObjectsHamsterWheel(this, mvCharacterController, hamsterWheelMotor);
	}
}
