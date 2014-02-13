using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MVVehicleBase : MVBlueprintBase
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

		public virtual void Destroy()
		{
			if (onDestroy != null)
			{
				onDestroy();
			}
			foreach (Component localComponent in localComponents)
			{
				Object.Destroy((Object)(object)localComponent);
			}
		}

		public virtual void Enter()
		{
			if (onEnter != null)
			{
				onEnter();
			}
		}

		public virtual void Leave()
		{
			if (onLeave != null)
			{
				onLeave();
			}
		}

		public abstract MovementMap Update(MovementMap movementMap);

		public abstract MovementMap FixedUpdate(MovementMap movementMap);

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
			MVGameController.Instance.Game.PlayerController.OverrideRemoveTimeForDismountedWorldObject(Id, timeBeforeUnregisterAfterDeath);
			Owner.IsVehicleDead.Value = true;
		}
	}

	public MVRuntimeDataVariable IsVehicleDead;

	protected VehicleSeatManager seatManager;

	protected LocalObjectsBase localObjects;

	protected VehicleVisualizationBase visualization;

	public virtual bool IsDead => (bool)IsVehicleDead.Value;

	public virtual bool IsInSpawner { get; private set; }

	protected MVVehicleBase(Hashtable data, string vehiclePrefab, Dictionary<int, MVWorldObjectClient> worldObjects)
		: base(data, vehiclePrefab, worldObjects)
	{
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
	}

	public void LeaveLocal()
	{
		localObjects.Leave();
	}

	public override void ChangeLOD(float distance)
	{
		if ((Object)(object)visualization != (Object)null)
		{
			visualization.ChangeLOD(distance);
		}
	}

	protected abstract LocalObjectsBase CreateLocalObjects(int seatID, MVAvatarLocal vehicleUser);

	public void Enter(MVAvatar vehicleUser, int seatID)
	{
		int num = vehicleUser.OwnerActorNr;
		bool flag = MVGameController.Instance.Game.LocalPlayer.ActorNr == num;
		seatManager.AttachWorldObjectToSeat(num, flag, vehicleUser, seatID);
		if (flag)
		{
			if (localObjects == null)
			{
				localObjects = CreateLocalObjects(seatID, (MVAvatarLocal)vehicleUser);
			}
			MVGameController.Instance.Game.PlayerController.Push(localObjects);
			((MVAvatarLocal)vehicleUser).SetAnimation("Idle");
			localObjects.Enter();
		}
		else if (localObjects != null)
		{
			localObjects.Destroy();
			localObjects = null;
		}
		vehicleUser.VehicleEntered();
		VehicleEntered(vehicleUser, seatID);
	}

	protected virtual void VehicleEntered(MVAvatar vehicleUser, int seatID)
	{
	}
}
