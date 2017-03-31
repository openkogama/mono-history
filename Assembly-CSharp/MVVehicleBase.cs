using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class MVVehicleBase : MVBlueprintBase, IBulletImpactVisualizer
{
	protected abstract class LocalObjectsBase : ILocalObject
	{
		public Action onDestroy;

		public Action onLeave;

		public Action onEnter;

		protected float timeBeforeUnregisterAfterDeath = 3f;

		protected List<Component> localComponents = new List<Component>();

		public abstract int Id { get; }

		protected abstract MVVehicleBase Owner { get; }

		public List<T> GetLocalComponents<T>() where T : Component
		{
			List<T> list = new List<T>();
			foreach (Component localComponent in localComponents)
			{
				if (localComponent is T)
				{
					list.Add((T)localComponent);
				}
			}
			return list;
		}

		public virtual void Destroy()
		{
			if (onDestroy != null)
			{
				onDestroy();
			}
			foreach (Component localComponent in localComponents)
			{
				UnityEngine.Object.Destroy(localComponent);
			}
		}

		public virtual void Enter()
		{
			if (onEnter != null)
			{
				onEnter();
			}
			List<MVRigidBody> list = GetLocalComponents<MVRigidBody>();
			List<VehicleInteractable> list2 = GetLocalComponents<VehicleInteractable>();
			if (list2.Count == 0)
			{
				Debug.LogWarning("Failed to get VehicleInteractable.");
			}
			if (list2.Count == 1)
			{
				list2[0].RemoveModifier(AvatarModifierPackageType.DisableVehiclePickup);
			}
			if (list.Count == 0)
			{
				Debug.LogWarning("Failed to get rigid bodies");
			}
			if (list.Count > 1)
			{
				Debug.LogWarning("More than 1 rigidBody. This is unexpected");
			}
			if (list.Count == 1)
			{
				list[0].IsPlayerControlled = true;
			}
		}

		public virtual void Leave()
		{
			if (onLeave != null)
			{
				onLeave();
			}
			List<VehicleInteractable> list = GetLocalComponents<VehicleInteractable>();
			if (list.Count == 0)
			{
				Debug.LogWarning("Failed to get VehicleInteractable.");
			}
			if (list.Count == 1)
			{
				list[0].AddModifier(AvatarModifierPackageType.DisableVehiclePickup);
			}
			List<MVRigidBody> list2 = GetLocalComponents<MVRigidBody>();
			if (list2.Count == 0)
			{
				Debug.LogWarning("Failed to get rigid bodies");
			}
			if (list2.Count > 1)
			{
				Debug.LogWarning("More than 1 rigidBody. This is unexpected");
			}
			if (list2.Count > 0)
			{
				list2[0].IsPlayerControlled = false;
			}
		}

		public abstract InputToInGameAction Update(InputToInGameAction interactionInput);

		public abstract IInputToPlayerMovement FixedUpdate(IInputToPlayerMovement movementMap);

		protected void OnHealthChange(object v)
		{
			float num = (float)v;
			if (!(num <= 0f) || (bool)Owner.IsVehicleDead.Value)
			{
				return;
			}
			MVAvatarLocal localAvatar = null;
			CallBackDelegate callBack = (MVWorldObjectClient wo) =>
			{
				if (wo is MVAvatarLocal)
				{
					localAvatar = (MVAvatarLocal)wo;
				}
			};
			Owner.TraverseRecursiveTail(callBack);
			if (localAvatar != null)
			{
				localAvatar.LeaveVehicle();
			}
			MVGameControllerBase.Game.PlayerController.OverrideRemoveTimeForDismountedWorldObject(Id, timeBeforeUnregisterAfterDeath);
			Owner.IsVehicleDead.Value = true;
		}
	}

	protected MVWorldObjectDocumentationType documentationType;

	public MVRuntimeDataVariable IsVehicleDead;

	protected VehicleSeatManager seatManager;

	protected LocalObjectsBase localObjects;

	protected VehicleVisualizationBase visualization;

	protected VehicleBaseObject vehicleBaseObject;

	public override MVWorldObjectDocumentationType DocumentationType => documentationType;

	public virtual bool IsDead => (bool)IsVehicleDead.Value;

	public virtual bool IsInSpawner { get; private set; }

	public VehicleVisualizationBase Visualization => visualization;

	protected MVVehicleBase(Dictionary<object, object> data, ObjectPrefab vehiclePrefab, Dictionary<int, MVWorldObjectClient> worldObjects)
		: base(data, vehiclePrefab, worldObjects)
	{
		vehicleBaseObject = (VehicleBaseObject)component;
	}

	public override void OnDataUpdate()
	{
		base.OnDataUpdate();
	}

	public override void Initialize()
	{
		base.Initialize();
		IsVehicleDead = RuntimeDataVariables.New("isDead", 0f, writeThrough: true);
		seatManager = gameObject.GetComponent<VehicleSeatManager>();
		seatManager.Init(this, IsVehicleDead);
		if (Group is MVWorldObjectSpawnerVehicle)
		{
			IsInSpawner = true;
		}
		LayerUtil.SetLayerRecursively(transform, "Default", "Player");
		interactionFlags |= InteractionFlags.CanUseGameCoins;
		interactionFlags |= InteractionFlags.CanUseLevel;
	}

	public void LeaveLocal()
	{
		localObjects.Leave();
	}

	public void Enter(MVAvatar vehicleUser, int seatID)
	{
		int num = vehicleUser.OwnerActorNr;
		vehicleUser.BeforeVehicleEntered();
		bool flag = MVGameControllerBase.Game.LocalPlayer.ActorNr == num;
		seatManager.AttachWorldObjectToSeat(num, flag, vehicleUser, seatID);
		if (flag)
		{
			if (localObjects == null)
			{
				localObjects = CreateLocalObjects(seatID, (MVAvatarLocal)vehicleUser);
			}
			MVGameControllerBase.Game.PlayerController.Push(localObjects);
			((MVAvatarLocal)vehicleUser).SetAnimation("Idle");
			localObjects.Enter();
		}
		else if (localObjects != null)
		{
			localObjects.Destroy();
			localObjects = null;
		}
		vehicleUser.OnEnterVehicle();
		VehicleEntered(vehicleUser, seatID);
	}

	protected abstract LocalObjectsBase CreateLocalObjects(int seatID, MVAvatarLocal vehicleUser);

	protected virtual void VehicleEntered(MVAvatar vehicleUser, int seatID)
	{
	}

	public void VisualizeBulletImpact(VoxelHit voxelHit, Ray lineOfFire, int shooterActorNumber, float damage)
	{
		if (!MVGameControllerBase.Game.TeamManager.IsOnSameTeam(OwnerActorNr, shooterActorNumber) && !IsDead)
		{
			vehicleBaseObject.BulletImpactVisualizer.VisualizeBulletImpact(voxelHit, lineOfFire, shooterActorNumber, damage);
			if (shooterActorNumber == MVGameControllerBase.Game.LocalPlayer.ActorNr)
			{
				MVGameControllerBase.CameraController.PlayPlingSound();
				MVGameControllerBase.IPlayModeUI.GetCrossHair().ShowHasHitEffect();
			}
		}
	}
}
