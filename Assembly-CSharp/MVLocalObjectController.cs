using System;
using System.Collections.Generic;
using MV.Common;
using UnityEngine;

public class MVLocalObjectController : IUpdatecontrollerSubscriber
{
	internal interface IAttachInterface
	{
	}

	internal class DetachState : IAttachInterface
	{
	}

	internal class AttachState : IAttachInterface
	{
		private bool transformDataWasSuspended;

		private int woID;

		public AttachState(int worldObjectID)
		{
			MVNetworkObject networkObject = MVGameControllerBase.Game.TransformNetworkManager.GetNetworkObject(worldObjectID);
			if (networkObject != null && networkObject is MVNetworkReporter)
			{
				MVGameControllerBase.Game.TransformNetworkManager.RemoveNetworkObject(worldObjectID);
				transformDataWasSuspended = true;
			}
			woID = worldObjectID;
		}

		public void HandleAttachFailed()
		{
			MVNetworkObject networkObject = MVGameControllerBase.Game.TransformNetworkManager.GetNetworkObject(woID);
			if (networkObject == null && transformDataWasSuspended)
			{
				MVGameControllerBase.Game.TransformNetworkManager.AddReporter(woID, new MVNetworkReporter(MVGameControllerBase.WOCM.GetWorldObjectClient(woID)));
			}
		}

		public override string ToString()
		{
			return $"WoID {woID}, transformDataWasSuspended {transformDataWasSuspended}";
		}
	}

	internal class DismountedPlayerControlledObject
	{
		private float dismountTime;

		private ILocalObject playerControlledObject;

		public ILocalObject PlayerControlledObject => playerControlledObject;

		public DismountedPlayerControlledObject(ILocalObject playerControlledObject)
		{
			this.playerControlledObject = playerControlledObject;
			dismountTime = Time.time;
		}

		public bool ReadyToUnRegister()
		{
			return Time.time - dismountTime > 30f;
		}

		public void SetTimeBeforeUnregister(float newTimeBeforeUnregister)
		{
			float num = 30f - newTimeBeforeUnregister;
			dismountTime = Time.time - num;
		}
	}

	private const int maxLoclControlledObjects = 4;

	public const float TimeBeforeUnregister = 30f;

	private readonly IInputToPlayerMovement movementMap;

	private InputToInGameAction interactionInput = new InputToInGameAction();

	private IAttachInterface attachState;

	private Stack<ILocalObject> localControlledStack = new Stack<ILocalObject>();

	private Dictionary<int, DismountedPlayerControlledObject> dismountedLocalControlledObjects = new Dictionary<int, DismountedPlayerControlledObject>();

	private MVWorldObjectClientManagerNetwork worldObjectClientManagerNetwork;

	public bool IsEnteringVehicle
	{
		get
		{
			if (attachState != null && attachState is AttachState)
			{
				return true;
			}
			return false;
		}
	}

	public HashSet<int> LocalControlledWorldObjects
	{
		get
		{
			HashSet<int> hashSet = new HashSet<int>(dismountedLocalControlledObjects.Keys);
			foreach (ILocalObject item in localControlledStack)
			{
				hashSet.Add(item.Id);
			}
			return hashSet;
		}
	}

	public MVWorldObjectClient CurrentWorldObject
	{
		get
		{
			if (localControlledStack.Count == 0)
			{
				return null;
			}
			int id = localControlledStack.Peek().Id;
			MVWorldObjectClient worldObjectClient = MVGameControllerBase.WOCM.GetWorldObjectClient(id);
			if (worldObjectClient == null)
			{
				Debug.LogError("wo does not exist");
			}
			return worldObjectClient;
		}
	}

	public MVLocalObjectController(MVWorldObjectClientManagerNetwork worldObjectClientManagerNetwork, MVGameType gameType)
	{
		UpdateController.AddUpdateObject(this, UpdatePriority.PRE_UPDATEBUCKET_10);
		UpdateController.AddFixedUpdateObject(this, UpdatePriority.PRE_UPDATEBUCKET_10);
		this.worldObjectClientManagerNetwork = worldObjectClientManagerNetwork;
		movementMap = CreateInputToPlayerMovement(MVGameControllerBase.GameMode, gameType);
	}

	private static IInputToPlayerMovement CreateInputToPlayerMovement(MVGameMode gameMode, MVGameType gameType)
	{
		if (gameMode == MVGameMode.CharacterEditor)
		{
			return new InputToPlayerMovementAvatarEdit();
		}
		return gameType switch
		{
			MVGameType.Classic => (IInputToPlayerMovement)new InputToPlayerMovement(), 
			MVGameType.Platformer => new InputToPlayerMovement(), 
			_ => throw new Exception("gametype not supported: " + gameType), 
		};
	}

	public void Push(ILocalObject localObject)
	{
		localControlledStack.Push(localObject);
	}

	public void UpdateControllerUpdate()
	{
		if (MVGameControllerBase.JoinState == MVJoinState.Playing)
		{
			UpdateLocalControlledObjects();
			UpdateDismountedPlayerControlledObjects();
		}
	}

	public void UpdateControllerFixedUpdate()
	{
		if (MVGameControllerBase.JoinState == MVJoinState.Playing)
		{
			FixedUpdateLocalControlledObjects();
			FixedUpdateDismountedPlayerControlledObjects();
		}
	}

	public bool DetachWorldObjectFromVehicle(int worldObjectID, ref int vehicleID)
	{
		if (attachState != null && attachState is DetachState)
		{
			Debug.LogError("EnterDetachState is already pending with detach state ");
			return false;
		}
		if (localControlledStack.Count == 0)
		{
			Debug.LogError("Trying to detach but nothing in stack to detach");
			return false;
		}
		ILocalObject localObject = localControlledStack.Pop();
		if (!dismountedLocalControlledObjects.ContainsKey(localObject.Id))
		{
			dismountedLocalControlledObjects.Add(localObject.Id, new DismountedPlayerControlledObject(localObject));
		}
		attachState = new DetachState();
		Debug.Log("Detaching from vehicle");
		MVGameControllerBase.OperationRequests.DetachWorldObjectFromVehicle(worldObjectID);
		vehicleID = localObject.Id;
		return true;
	}

	public void OverrideRemoveTimeForDismountedWorldObject(int woID, float timeBeforeUnregister)
	{
		if (!dismountedLocalControlledObjects.ContainsKey(woID))
		{
			Debug.LogError("Dismounted object not found");
		}
		else
		{
			dismountedLocalControlledObjects[woID].SetTimeBeforeUnregister(timeBeforeUnregister);
		}
	}

	public void OnAttachWorldObjectToSeat(int instigatorActorNr, int seatOwnerWoID, int worldObjectID, int seatID)
	{
		if (dismountedLocalControlledObjects.ContainsKey(seatOwnerWoID))
		{
			Debug.Log("Removing dismountedPlayerObject because of seat change");
			dismountedLocalControlledObjects.Remove(seatOwnerWoID);
		}
		worldObjectClientManagerNetwork.OnAttachWorldObjectToSeat(instigatorActorNr, seatOwnerWoID, worldObjectID, seatID);
	}

	public bool AttachWorldObjectToSeat(int seatOwnerWoID, int worldObjectID, VehicleSeatBase seatBase)
	{
		if (attachState != null)
		{
			Debug.LogWarning("AttachWorldObjectToSeat is already pending with id " + attachState);
			return false;
		}
		attachState = new AttachState(worldObjectID);
		MVGameControllerBase.OperationRequests.AttachWorldObjectToSeat(seatOwnerWoID, worldObjectID, seatBase);
		return true;
	}

	public bool SpawnVehicleWithDriver(int worldObjectSpawnerVehicleID, int worldObjectID, VehicleSeatBase seatBase)
	{
		if (LocalControlledWorldObjects.Count >= 4)
		{
			Debug.LogWarning("Can't have more active Client Controlled WorldObjects. Count is " + LocalControlledWorldObjects.Count + " max is " + 4);
			return false;
		}
		if (attachState != null)
		{
			Debug.LogError("SpawnVehicleWithDriver is already pending");
			return false;
		}
		attachState = new AttachState(worldObjectID);
		MVGameControllerBase.OperationRequests.SpawnVehicleWithDriver(worldObjectSpawnerVehicleID, worldObjectID, seatBase);
		return true;
	}

	public void HandleDetachWorldObjectFromVehicle(bool success)
	{
		attachState = null;
		if (!success)
		{
			Debug.LogError("HandleDetachWorldObjectFromVehicle failed");
		}
	}

	public void HandleAttachWorldObjectToSeat(bool success)
	{
		if (success)
		{
			attachState = null;
		}
		else
		{
			HandleAttachFailed();
		}
	}

	private void UpdateLocalControlledObjects()
	{
		movementMap.HandleInputState(fromFrameUpdate: true);
		interactionInput.HandleInputState();
		if (localControlledStack.Count != 0)
		{
			ILocalObject[] array = localControlledStack.ToArray();
			ILocalObject[] array2 = array;
			foreach (ILocalObject localObject in array2)
			{
				interactionInput = localObject.Update(interactionInput);
			}
		}
	}

	private void UpdateDismountedPlayerControlledObjects()
	{
		List<int> list = new List<int>();
		foreach (KeyValuePair<int, DismountedPlayerControlledObject> dismountedLocalControlledObject in dismountedLocalControlledObjects)
		{
			if (dismountedLocalControlledObject.Value.ReadyToUnRegister() || MVGameControllerBase.Game.NetworkGameStateListener.CurrentGameState == MVGameStateType.RoundEnded)
			{
				list.Add(dismountedLocalControlledObject.Key);
			}
			else
			{
				dismountedLocalControlledObject.Value.PlayerControlledObject.Update(null);
			}
		}
		foreach (int item in list)
		{
			MVGameControllerBase.WOCM.UnregisterWorldObject(item);
			dismountedLocalControlledObjects.Remove(item);
		}
	}

	private void FixedUpdateLocalControlledObjects()
	{
		if (localControlledStack.Count != 0)
		{
			movementMap.HandleInputState(fromFrameUpdate: false);
			ILocalObject[] array = localControlledStack.ToArray();
			ILocalObject[] array2 = array;
			foreach (ILocalObject localObject in array2)
			{
				localObject.FixedUpdate(movementMap);
			}
		}
	}

	private void FixedUpdateDismountedPlayerControlledObjects()
	{
		foreach (KeyValuePair<int, DismountedPlayerControlledObject> dismountedLocalControlledObject in dismountedLocalControlledObjects)
		{
			dismountedLocalControlledObject.Value.PlayerControlledObject.FixedUpdate(null);
		}
	}

	private bool HandleAttachFailed()
	{
		if (attachState == null)
		{
			Debug.LogError("trying to leave attachState, but no attach state created");
			return false;
		}
		if (attachState is DetachState)
		{
			Debug.LogWarning("AttachState is detachState");
		}
		((AttachState)attachState).HandleAttachFailed();
		attachState = null;
		return true;
	}
}
