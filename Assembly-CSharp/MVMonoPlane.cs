using System;
using System.Collections.Generic;
using MV.Common;
using MV.WorldObject;
using MV.WorldObject.RuntimeEvents;
using UnityEngine;

public class MVMonoPlane : MVSimpleOneSeatVehicle
{
	private const string _vehiclePrefab = "Prefabs/Blueprints/Vehicles/MonoPlane";

	private float deathExplosionDamageValue = 40f;

	private float deathExplosionRadius = 10f;

	private float deathExplosionImpulse = 2000f;

	public MVMonoPlane(Dictionary<object, object> data, Dictionary<int, MVWorldObjectClient> worldObjects)
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
		if ((bool)isDead)
		{
			HashSet<int> worldIDsRecursive = WorldIDsRecursive;
			Vector3 vector = Vector3.down + gameObject.transform.rotation * Vector3.forward;
			if (localObjects == null)
			{
				SharedWorldObjectGameplayFunctions.Explosion.Explode("ParticleFX/Explosion", gameObject.transform.position + vector, deathExplosionDamageValue, deathExplosionRadius, deathExplosionImpulse, local: true, null, worldIDsRecursive);
				return;
			}
			ExplosionEvent explosionEvent = new ExplosionEvent(RuntimeEventType.Bazooka, gameObject.transform.position + vector);
			SharedWorldObjectGameplayFunctions.Explosion.Explode("ParticleFX/Explosion", gameObject.transform.position + vector, deathExplosionDamageValue, deathExplosionRadius, deathExplosionImpulse, local: false, explosionEvent, worldIDsRecursive);
		}
	}

	public override Bounds GetLocalBounds(BoundsContext boundsContext)
	{
		if (boundsContext == BoundsContext.Insert || boundsContext == BoundsContext.BoxVisualization)
		{
			return new Bounds(Vector3.zero, Vector3.one * 2f);
		}
		return base.GetLocalBounds(boundsContext);
	}

	protected override LocalObjectsBase CreateLocalObjects(int seatID, MVAvatarLocal vehicleUser)
	{
		SmoothCharacterController smoothCharacterController = gameObject.AddComponent<SmoothCharacterController>();
		smoothCharacterController.Init(gameObject);
		smoothCharacterController.Controller.Init(1.3f, 2f, Vector3.up * 0.4f);
		smoothCharacterController.Controller.IgnoreWoIds = WorldIDsRecursive;
		MonoPlaneMotor monoPlaneMotor = gameObject.AddComponent<MonoPlaneMotor>();
		MVCameraBase camera = seatManager.seats[seatID].Camera;
		if (!(camera is AirCraftCamera))
		{
			Debug.LogError("Expected camera type is VehicleCamera.");
			return null;
		}
		monoPlaneMotor.VehicleCamera = (AirCraftCamera)camera;
		return new LocalObjectsSimpleVehicle(this, smoothCharacterController, monoPlaneMotor);
	}
}
