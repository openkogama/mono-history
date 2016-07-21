using System;
using System.Collections.Generic;
using MV.Common;
using MV.WorldObject;
using MV.WorldObject.RuntimeEvents;
using UnityEngine;

public class MVHoverCraft : MVSimpleOneSeatVehicle
{
	protected class LocalObjectsHoverCraft : LocalObjectsSimpleVehicle
	{
		public LocalObjectsHoverCraft(MVSimpleOneSeatVehicle vehicleBase, SmoothCharacterController smoothController, SimpleVehicleMotorBase hoverCraftMotor)
			: base(vehicleBase, smoothController, hoverCraftMotor)
		{
		}

		public override void Enter()
		{
			base.Enter();
			GameSessionCounters.SetCount(GameSessionCounterType.InHoverCraft, 1);
		}

		public override void Leave()
		{
			base.Leave();
			GameSessionCounters.SetCount(GameSessionCounterType.InHoverCraft, 0);
		}
	}

	private float deathExplosionDamageValue = 40f;

	private float deathExplosionRadius = 10f;

	private float deathExplosionImpulse = 2000f;

	private CullingSubscriberDynamic cullingSubscriberDynamic;

	public override bool IsDead => (bool)IsVehicleDead.Value;

	public MVHoverCraft(Dictionary<object, object> data, Dictionary<int, MVWorldObjectClient> worldObjects)
		: base(data, PrefabPool.Instance.MVHoverCraftPrefab, worldObjects)
	{
		interactionFlags |= InteractionFlags.CanEdit;
	}

	public override void Initialize()
	{
		base.Initialize();
		MVCubeModelInstance mVCubeModelInstance = (MVCubeModelInstance)GetChild("HoverCraftHull");
		HoverCraftVisualization componentInChildren = gameObject.GetComponentInChildren<HoverCraftVisualization>();
		componentInChildren.gameObject.SetActive(value: true);
		if (!IsInSpawner)
		{
			gameObject.AddComponent<InteractionDataHandler>();
			cullingSubscriberDynamic = new CullingSubscriberDynamic(4f, 3, componentInChildren.gameObject);
			mVCubeModelInstance.Visible = true;
			componentInChildren.enabled = true;
		}
		MVRuntimeDataVariable isVehicleDead = IsVehicleDead;
		isVehicleDead.OnChange = (MVRuntimeDataVariable.OnChangeDelegate)Delegate.Combine(isVehicleDead.OnChange, new MVRuntimeDataVariable.OnChangeDelegate(OnIsDeadChange));
		componentInChildren.Init(mVCubeModelInstance.GameObject.transform, seatManager, (float)RuntimeVariablesRepository.GetRuntimeVariables(WorldObjectType)["health"], Health, IsInSpawner);
		visualization = componentInChildren;
		editableCubeModelWrapper = new EditableCubeModelWrapper(mVCubeModelInstance, new IntVector(-16, -2, -5), new IntVector(2, 5, 6), 50);
	}

	public override void InitializeInventory()
	{
		base.InitializeInventory();
		visualization = gameObject.GetComponentInChildren<HoverCraftVisualization>();
		visualization.enabled = false;
	}

	public override void Destroy()
	{
		if (cullingSubscriberDynamic != null)
		{
			cullingSubscriberDynamic.Destroy();
			cullingSubscriberDynamic = null;
		}
	}

	private void OnIsDeadChange(object isDead)
	{
		if ((bool)isDead)
		{
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
		if (boundsContext == BoundsContext.Insert || boundsContext == BoundsContext.BoxVisualization)
		{
			return new Bounds(Vector3.zero, Vector3.one * 2f);
		}
		return base.GetLocalBounds(boundsContext);
	}

	protected override LocalObjectsBase CreateLocalObjects(int seatID, MVAvatarLocal vehicleUser)
	{
		SmoothCharacterController smoothCharacterController = gameObject.AddComponent<SmoothCharacterController>();
		smoothCharacterController.Init(gameObject, null);
		smoothCharacterController.Controller.Init(1.3f, 2f, Vector3.up * 0.3f);
		smoothCharacterController.Controller.IgnoreWoIds = WorldIDsRecursive;
		HoverCraftMotor hoverCraftMotor = gameObject.AddComponent<HoverCraftMotor>();
		MVCameraBase camera = seatManager.seats[seatID].Camera;
		if (!(camera is IVehicleCamera))
		{
			Debug.LogError("Expected camera type is VehicleCamera.");
			return null;
		}
		hoverCraftMotor.VehicleCamera = (IVehicleCamera)camera;
		return new LocalObjectsHoverCraft(this, smoothCharacterController, hoverCraftMotor);
	}
}
