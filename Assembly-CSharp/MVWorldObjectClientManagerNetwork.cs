using System;
using System.Collections;
using System.Collections.Generic;
using MV.Common;
using MV.WorldObject;
using UnityEngine;

public class MVWorldObjectClientManagerNetwork : MVWorldObjectClientManager
{
	private List<int> deleteList = new List<int>();

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
		}
		if (MVGameControllerBase.Game.IsPlaying)
		{
			MVGameControllerBase.Game.LocalPlayer.ResetCheckpoint();
			MVGameControllerBase.Game.GameCoinManager.Reset(MVGameControllerBase.Game);
			AvatarLocal.SetMode(AvatarRuntimeState.Hidden);
		}
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
			if (ownerActorNr != 0)
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
			SetOwnerRecursively(id, MVGameControllerBase.Game.LocalPlayerActorNumber);
		}
		else
		{
			SetOwnerRecursively(id, 0);
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
		SetOwnerRecursively(id, actorNr);
		return true;
	}

	private void SetOwnerRecursively(int id, int actorNr)
	{
		Debug.LogError("SetOwnerRecursively is not recursive! Idiot!");
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
		SetState(id, MVWorldObjectState.Destroyed);
		deleteList.Add(id);
		UnityEngine.Object.Destroy(worldObjects[id].GameObject);
	}

	public void SetState(int id, MVWorldObjectState state)
	{
		GetWorldObjectClient(id).State = state;
		if (state == MVWorldObjectState.Destroyed)
		{
			deleteList.Add(id);
		}
		if (!(GetWorldObjectClient(id) is MVGroup))
		{
			return;
		}
		foreach (MVWorldObjectClient child in ((MVGroup)GetWorldObjectClient(id)).Children)
		{
			SetState(child.Id, state);
		}
	}

	public void Update(MVNetworkGame game)
	{
		HandleDeletedObjects();
		worldObjectLOD.UpdateLOD();
	}

	private void HandleDeletedObjects()
	{
		foreach (int delete in deleteList)
		{
			MVWorldObjectClient worldObjectClient = GetWorldObjectClient(delete);
			worldObjectMapping.RemoveWorldObjectFromTypeSet(worldObjectClient);
			worldObjectClient.Destroy();
			worldObjects.Remove(worldObjectClient.Id);
		}
		deleteList.Clear();
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
		worldObjectLOD.AddWorldObjectToLOD(wo.Id);
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
		mVWorldObjectClient.State = MVWorldObjectState.Synced;
	}
}
