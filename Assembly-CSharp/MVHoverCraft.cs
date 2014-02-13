using System;
using System.Collections;
using System.Collections.Generic;
using MV.WorldObject;
using UnityEngine;

public class MVHoverCraft : MVSimpleOneSeatVehicle
{
	private const string _vehiclePrefab = "Prefabs/Blueprints/Vehicles/HoverCraft";

	private float deathExplosionDamageValue = 40f;

	private float deathExplosionRadius = 10f;

	private float deathExplosionImpulse = 2000f;

	public override bool IsDead => (bool)IsVehicleDead.Value;

	public MVHoverCraft(Hashtable data, Dictionary<int, MVWorldObjectClient> worldObjects)
		: base(data, "Prefabs/Blueprints/Vehicles/HoverCraft", worldObjects)
	{
		interactionFlags |= InteractionFlags.CanEdit;
	}

	public override void Initialize()
	{
		base.Initialize();
		MVRuntimeDataVariable isVehicleDead = IsVehicleDead;
		isVehicleDead.OnChange = (MVRuntimeDataVariable.OnChangeDelegate)Delegate.Combine(isVehicleDead.OnChange, new MVRuntimeDataVariable.OnChangeDelegate(OnIsDeadChange));
		MVCubeModelBase mVCubeModelBase = (MVCubeModelBase)GetChild("HoverCraftHull");
		HoverCraftVisualization component = gameObject.GetComponent<HoverCraftVisualization>();
		component.Init(mVCubeModelBase.GameObject.transform, seatManager, (float)RuntimeVariablesRepository.GetRuntimeVariables(WorldObjectType)["health"], Health, IsInSpawner);
		visualization = component;
		editableCubeModelWrapper = new EditableCubeModelWrapper(mVCubeModelBase, new IntVector(-16, -2, -5), new IntVector(2, 5, 6), 50);
	}

	public override void InitializeInventory()
	{
		base.InitializeInventory();
		visualization = gameObject.GetComponent<HoverCraftVisualization>();
		((Behaviour)visualization).enabled = false;
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
		mvCharacterController.Init(1.3f, 2f, Vector3.up * 0.3f);
		mvCharacterController.IgnoreWoIds = WorldIDsRecursive;
		HoverCraftMotor hoverCraftMotor = gameObject.AddComponent<HoverCraftMotor>();
		MVCameraBase camera = seatManager.seats[seatID].Camera;
		if (!(camera is VehicleCamera))
		{
			Debug.LogError((object)"Expected camera type is VehicleCamera.");
			return null;
		}
		hoverCraftMotor.VehicleCamera = (VehicleCamera)camera;
		return new LocalObjectsSimpleVehicle(this, mvCharacterController, hoverCraftMotor);
	}
}
