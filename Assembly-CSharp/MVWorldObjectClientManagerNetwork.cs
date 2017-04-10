using System;
using System.Collections;
using System.Collections.Generic;
using MV.WorldObject;
using UnityEngine;

public class MVWorldObjectClientManagerNetwork : MVWorldObjectClientManager
{
	public void Cleanup()
	{
		foreach (MVWorldObjectClient value in worldObjects.Values)
		{
			value.Destroy();
		}
		worldObjects.Clear();
	}

	public void OnCloneWorldObjectTreeResponse(bool success, int rootId)
	{
		if (CloneWorldObjectTreeResponse != null)
		{
			CloneWorldObjectTreeResponseEventArgs e = new CloneWorldObjectTreeResponseEventArgs(success, rootId);
			CloneWorldObjectTreeResponse(this, e);
		}
	}

	public void ResetWorld()
	{
		foreach (MVWorldObjectClient value in worldObjects.Values)
		{
			value.Reset();
			if (value is ILogicWorldObject)
			{
				((ILogicWorldObject)value).InputSignalReceiver.Reset();
			}
		}
		MVGameControllerBase.Game.GameCoinManager.Reset(MVGameControllerBase.Game);
		if (OnResetWorldDone != null)
		{
			OnResetWorldDone(this, new EventArgs());
		}
	}

	public void ResetLocalWorldObject()
	{
		List<MVWorldObjectClient> worldObjectsByType = GetWorldObjectsByType(WorldObjectType.CollectibleItem);
		foreach (MVWorldObjectClient item in worldObjectsByType)
		{
			item.Reset();
		}
	}

	public bool OnSetWorldObjectsToPurchasedEvent(int profileID, int itemID)
	{
		int num = 0;
		foreach (MVWorldObjectClient value in worldObjects.Values)
		{
			if (value.PreviewOwnerProfileId == profileID && value.ItemId == itemID)
			{
				value.SetWorldObjectToPurchased();
				num++;
			}
		}
		return num > 0;
	}

	public void OnTransferWorldObjectsToGroupEvent(int groupId, int[] worldObjectsToGroup)
	{
		MVGroup mVGroup = (MVGroup)worldObjects[groupId];
		foreach (int id in worldObjectsToGroup)
		{
			mVGroup.TransferChild(id);
		}
	}

	public void OnUpdateWorldObjectDataEvent(int worldObjectID, Dictionary<object, object> worldObjectData)
	{
		if (!worldObjects.ContainsKey(worldObjectID))
		{
			Debug.LogError("Attempt to update WorldObjectData on unknown WorldObject " + worldObjectID);
			return;
		}
		worldObjects[worldObjectID].Data = worldObjectData;
		worldObjects[worldObjectID].OnDataUpdate();
	}

	public void OnUpdateWorldObjectDataPartialEvent(int worldObjectID, Dictionary<object, object> worldObjectData)
	{
		if (!worldObjects.ContainsKey(worldObjectID))
		{
			Debug.LogError("Attempt to update WorldObjectData on unknown WorldObject " + worldObjectID);
		}
		else
		{
			worldObjects[worldObjectID].PartialUpdateWOData(worldObjectData);
		}
	}

	public void OnRemoveWorldObjectDataPartialEvent(int worldObjectID, Dictionary<object, object> worldObjectDataToRemove)
	{
		if (!worldObjects.ContainsKey(worldObjectID))
		{
			Debug.LogError("Attempt to remove data from WorldObjectData on unknown WorldObject " + worldObjectID);
		}
		else
		{
			worldObjects[worldObjectID].PartialRemoveFromWOData(worldObjectDataToRemove);
		}
	}

	public void OnUpdateWorldObjectRunTimeDataEvent(int worldObjectID, Dictionary<object, object> delta)
	{
		if (!worldObjects.ContainsKey(worldObjectID))
		{
			Debug.LogError("Attempt to update WorldObjectData on unknown WorldObject " + worldObjectID);
			return;
		}
		MVWorldObjectClient mVWorldObjectClient = worldObjects[worldObjectID];
		mVWorldObjectClient.RuntimeDataUpdate(delta);
	}

	public bool TransferOwnershipProxy(int id, int ownerActorNr)
	{
		if (!worldObjects.ContainsKey(id))
		{
			return false;
		}
		worldObjects[id].OwnerActorNr = ownerActorNr;
		if (ownerActorNr == 0)
		{
			worldObjects[id].DeSelect();
		}
		else
		{
			worldObjects[id].Select(Color.blue);
		}
		return true;
	}

	public void HandleTransferWorldObjectsToGroup(bool success)
	{
		if (!success)
		{
			Debug.LogError("HandleTransferWorldObjectsToGroup failed");
		}
		if (OnTransferWosResponse != null)
		{
			OnTransferWosResponse(this, new OnTransferWosResponseEventArgs(success));
		}
	}

	public bool TransferOwnershipResponse(int id, int ownerActorNr, bool success)
	{
		if (!worldObjects.ContainsKey(id))
		{
			return false;
		}
		if (success)
		{
			worldObjects[id].OwnerActorNr = ownerActorNr;
			if (ownerActorNr == 0)
			{
				MVNetworkObject networkObject = MVGameControllerBase.Game.TransformNetworkManager.GetNetworkObject(id);
				if (networkObject is MVNetworkReporter)
				{
					MVGameControllerBase.Game.TransformNetworkManager.RemoveNetworkObject(id);
				}
			}
			else
			{
				MVGameControllerBase.Game.TransformNetworkManager.AddReporter(id, new MVNetworkReporter(worldObjects[id]));
			}
		}
		else
		{
			Debug.LogWarning("Failed to set ownership...");
		}
		if (OnWorldObjectTransferOwnershipResponse != null)
		{
			OnWorldObjectTransferOwnershipResponse(this, new OnTransferOwnershipResponseEventArgs(id, ownerActorNr, success));
		}
		return true;
	}

	public bool LockHierarchyResponse(int id, bool lockObject, bool success)
	{
		if (!worldObjects.ContainsKey(id))
		{
			return false;
		}
		if (lockObject)
		{
			SetOwnerInHierarchy(id, MVGameControllerBase.Game.LocalPlayerActorNumber);
		}
		else
		{
			SetOwnerInHierarchy(id, 0);
		}
		if (OnHierarchyLockedResponse != null)
		{
			OnHierarchyLockedResponse(this, new OnHierarchyLockedEventArgs(id, success));
		}
		return true;
	}

	public bool LockHierarchyProxy(int id, int actorNr)
	{
		Debug.Log("LockHierarchyProxy " + id);
		SetOwnerInHierarchy(id, actorNr);
		return true;
	}

	private void SetOwnerInHierarchy(int id, int actorNr)
	{
		Debug.LogError("SetOwnerInHierarchy has been called! Restart Session!");
		worldObjects[id].OwnerActorNr = actorNr;
		if (worldObjects[id].GetType() != typeof(MVGroup))
		{
			return;
		}
		foreach (MVWorldObjectClient child in ((MVGroup)worldObjects[id]).Children)
		{
			child.OwnerActorNr = actorNr;
		}
	}

	public bool UngroupResponse(bool success)
	{
		if (success)
		{
			if (pendingUngroupQueue.Count <= 0)
			{
				Debug.LogError("UngroupResponse, but no object on pendingUngroupQueue");
				return false;
			}
			int num = pendingUngroupQueue.Dequeue();
			UngroupExecute(num);
			if (OnUngroupResponse != null)
			{
				OnUngroupResponse(this, new OnUngroupResponseEventArgs(num, success));
			}
			return true;
		}
		if (pendingUngroupQueue.Count <= 0)
		{
			Debug.LogError("UngroupResponse, but no object on pendingUngroupQueue");
			return false;
		}
		int worldObjectID = pendingUngroupQueue.Dequeue();
		if (OnUngroupResponse != null)
		{
			OnUngroupResponse(this, new OnUngroupResponseEventArgs(worldObjectID, success));
		}
		return false;
	}

	public bool UngroupProxy(int id)
	{
		if (!worldObjects.ContainsKey(id))
		{
			return false;
		}
		if (worldObjects[id].GroupId != -1)
		{
			UngroupExecute(id);
		}
		return true;
	}

	private void UngroupExecute(int id)
	{
		int groupId = worldObjects[id].GroupId;
		ArrayList arrayList = new ArrayList(((MVGroup)worldObjects[id]).Children);
		foreach (MVWorldObjectClient item in arrayList)
		{
			((MVGroup)worldObjects[groupId]).TransferChild(item.Id);
		}
		DestroyWO(id);
		UnityEngine.Object.Destroy(worldObjects[id].GameObject);
	}

	public void DestroyWO(int id)
	{
		if (GetWorldObjectClient(id) is MVGroup)
		{
			foreach (MVWorldObjectClient child in ((MVGroup)GetWorldObjectClient(id)).Children)
			{
				DestroyWO(child.Id);
			}
		}
		DeleteWO(id);
	}

	private void DeleteWO(int id)
	{
		MVWorldObjectClient worldObjectClient = GetWorldObjectClient(id);
		if (worldObjectClient == null)
		{
			Debug.LogError("Wo not found. This is most likely because non recursive delete server side");
			return;
		}
		worldObjectMapping.RemoveWorldObjectFromTypeSet(worldObjectClient);
		OnWorldObjectDestroyed(worldObjectClient.Id);
		int groupId = worldObjectClient.GroupId;
		if (groupId != -1)
		{
			if (worldObjects.TryGetValue(groupId, out var value))
			{
				if (value is MVGroup)
				{
					((MVGroup)value).RemoveChild(id);
				}
				else
				{
					Debug.LogError("World object is attached to non group. This should not be possible: " + value.WorldObjectType);
				}
			}
			else
			{
				Debug.LogError("Could not find group when destroying wo");
			}
		}
		worldObjectClient.Destroy();
		worldObjects.Remove(worldObjectClient.Id);
		MVGameControllerBase.Game.LogicObjectManager.RemoveLogicObjectFromUpdate(id);
	}

	public void OnWorldObjectDestroyed(int woID)
	{
		if (woDestroyedEventSubscribers.TryGetValue(woID, out var value))
		{
			value(this, new WorldObjectDestroyedEventArgs(woID));
			woDestroyedEventSubscribers.Remove(woID);
		}
	}

	public void OnAttachWorldObjectToSeat(int instigatorActorNr, int seatOwnerWoID, int worldObjectID, int seatID)
	{
		MVWorldObjectClient value2;
		if (!worldObjects.TryGetValue(seatOwnerWoID, out var value))
		{
			Debug.LogError("SeatOwnerWorldObject Not found");
		}
		else if (!worldObjects.TryGetValue(worldObjectID, out value2))
		{
			Debug.LogError("worldObjectClient not found");
		}
		else
		{
			((MVVehicleBase)value).Enter((MVAvatar)value2, seatID);
		}
	}

	public void AddToWorldObjects(MVWorldObjectClient wo)
	{
		if (worldObjects.ContainsKey(wo.Id))
		{
			Debug.LogError("Key already in WorldObjects dictionary");
			return;
		}
		worldObjects.Add(wo.Id, wo);
		worldObjectMapping.AddWorldObjectToTypeSet(wo);
		Type type = wo.GetType();
		woCreatedEventSubscribers.TryGetValue(type, out var value);
		value?.Invoke(this, new WorldObjectCreatedEventArgs(wo));
		if (wo.GroupId == -1)
		{
			RootGroup = (MVGroup)wo;
		}
	}

	public bool Ungroup(int id)
	{
		if (!worldObjects.ContainsKey(id))
		{
			return false;
		}
		pendingUngroupQueue.Enqueue(id);
		return true;
	}

	public MVWorldObjectClient Clone(int ownerActorNumber, MVWorldObjectClient rootOriginal, CloneBookkeeping cloneBookkeeping, MVWorldInventory worldInventory)
	{
		return rootOriginal.Clone(ownerActorNumber, rootOriginal.GroupId, cloneBookkeeping, worldObjects, worldInventory.RuntimePrototypes);
	}

	public void AddWorldObject(Dictionary<object, object> data, MVWorldInventory worldInventory)
	{
		MVWorldObjectClient mVWorldObjectClient = KoGaMaPackageClient.WorldObjectFactory(data, worldObjects, worldInventory.RuntimePrototypes);
		if (mVWorldObjectClient != null)
		{
			MVNetworkObject networkObject = MVGameControllerBase.Game.TransformNetworkManager.GetNetworkObject(mVWorldObjectClient.Id);
			if (networkObject == null)
			{
				mVWorldObjectClient.SetNetworkObject(local: false);
			}
		}
		AddToWorldObjects(mVWorldObjectClient);
	}
}
