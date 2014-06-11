using System;
using System.Collections;
using System.Collections.Generic;
using MV.WorldObject;
using UnityEngine;

public class MVMonoPlane : MVSimpleOneSeatVehicle
{
	private const string _vehiclePrefab = "Prefabs/Blueprints/Vehicles/MonoPlane";

	private float deathExplosionDamageValue = 40f;

	private float deathExplosionRadius = 10f;

	private float deathExplosionImpulse = 2000f;

	public MVMonoPlane(Hashtable data, Dictionary<int, MVWorldObjectClient> worldObjects)
		: base(data, "Prefabs/Blueprints/Vehicles/MonoPlane", worldObjects)
	{
		interactionFlags |= InteractionFlags.CanEdit;
	}

	public override void Initialize()
	{
		base.Initialize();
		if (!IsInSpawner)
		{
			gameObject.AddComponent<InteractionDataHandler>();
		}
		MVRuntimeDataVariable isVehicleDead = IsVehicleDead;
		isVehicleDead.OnChange = (MVRuntimeDataVariable.OnChangeDelegate)Delegate.Combine(isVehicleDead.OnChange, new MVRuntimeDataVariable.OnChangeDelegate(OnIsDeadChange));
		MVCubeModelBase mVCubeModelBase = (MVCubeModelBase)GetChild("MonoPlaneHull");
		editableCubeModelWrapper = new EditableCubeModelWrapper(mVCubeModelBase, new IntVector(-16, -2, -5), new IntVector(2, 5, 6), 50);
		MonoPlaneVisualization component = gameObject.GetComponent<MonoPlaneVisualization>();
		component.Init(mVCubeModelBase.GameObject.transform, seatManager, (float)RuntimeVariablesRepository.GetRuntimeVariables(WorldObjectType)["health"], Health, IsInSpawner);
		visualization = component;
	}

	private void OnIsDeadChange(object isDead)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		if ((bool)isDead)
		{
			HashSet<int> worldIDsRecursive = WorldIDsRecursive;
			Vector3 val = Vector3.down + gameObject.transform.rotation * Vector3.forward;
			MVGameController.Instance.WOCM.SharedWorldObjectGameplayFunctions.ExplosionCreator.Explode(gameObject.transform.position + val, deathExplosionDamageValue, deathExplosionRadius, deathExplosionImpulse, worldIDsRecursive);
		}
	}

	public override Bounds GetLocalBounds(BoundsContext boundsContext)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		if (boundsContext == BoundsContext.Insert || boundsContext == BoundsContext.BoxVisualization)
		{
			return new Bounds(Vector3.zero, Vector3.one * 2f);
		}
		return base.GetLocalBounds(boundsContext);
	}

	protected override LocalObjectsBase CreateLocalObjects(int seatID, MVAvatarLocal vehicleUser)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		MvCharacterController mvCharacterController = gameObject.AddComponent<MvCharacterController>();
		mvCharacterController.Init(1.3f, 2f, Vector3.up * 0.4f);
		mvCharacterController.IgnoreWoIds = WorldIDsRecursive;
		MonoPlaneMotor monoPlaneMotor = gameObject.AddComponent<MonoPlaneMotor>();
		MVCameraBase camera = seatManager.seats[seatID].Camera;
		if (!(camera is AirCraftCamera))
		{
			Debug.LogError((object)"Expected camera type is VehicleCamera.");
			return null;
		}
		monoPlaneMotor.VehicleCamera = (AirCraftCamera)camera;
		return new LocalObjectsSimpleVehicle(this, mvCharacterController, monoPlaneMotor);
	}
}
