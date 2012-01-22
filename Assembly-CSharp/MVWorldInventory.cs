using System.Collections;
using System.Collections.Generic;
using MV.WorldObject;
using UnityEngine;

public class MVWorldInventory
{
	public delegate void OnWorldInventoryChangeDelegate(MVWorldInventory inventory);

	private const int numberOfLowPriorityMeshGenerations = 1;

	private Dictionary<int, MVPrototype> prototypes = new Dictionary<int, MVPrototype>();

	private Dictionary<int, RuntimePrototypeCubeModel> runtimePrototypes = new Dictionary<int, RuntimePrototypeCubeModel>();

	private Dictionary<int, PendingPrototypeData> pendingRuntimePrototypes = new Dictionary<int, PendingPrototypeData>();

	private List<RuntimePrototypeCubeModel> dirtyRPCM = new List<RuntimePrototypeCubeModel>();

	public OnWorldInventoryChangeDelegate OnWorldInventoryChange;

	public Dictionary<int, MVPrototype> Prototypes => prototypes;

	public Dictionary<int, RuntimePrototypeCubeModel> RuntimePrototypes => runtimePrototypes;

	public void AddRuntimePrototypeToDirty(RuntimePrototypeCubeModel rpcm)
	{
		dirtyRPCM.Add(rpcm);
	}

	private void GenerateDirtyRPCM()
	{
		int meshUpdates = 1;
		for (int num = dirtyRPCM.Count - 1; num >= 0; num--)
		{
			if (dirtyRPCM[num].MeshGeneratePriority == MeshGeneratePriority.High && dirtyRPCM[num].MeshGenerateDirtyChunks(ref meshUpdates))
			{
				dirtyRPCM.RemoveAt(num);
			}
		}
		for (int num2 = dirtyRPCM.Count - 1; num2 >= 0; num2--)
		{
			if (dirtyRPCM[num2].MeshGeneratePriority == MeshGeneratePriority.Low)
			{
				if (!dirtyRPCM[num2].MeshGenerateDirtyChunks(ref meshUpdates))
				{
					break;
				}
				dirtyRPCM.RemoveAt(num2);
			}
		}
	}

	public void LateUpdate()
	{
		GenerateDirtyRPCM();
	}

	public MVPrototype GetPrototype(int Id)
	{
		if (prototypes.ContainsKey(Id))
		{
			return prototypes[Id];
		}
		Debug.LogError((object)("Id not present " + Id));
		return null;
	}

	public void AddPrototype(int id, int itemID, int typeID, string name, Hashtable data, float scale, int actorNrInstigator)
	{
		MVPrototype mVPrototype = new MVPrototype();
		mVPrototype.ID = id;
		mVPrototype.TypeID = typeID;
		mVPrototype.Name = name;
		mVPrototype.Data = data;
		mVPrototype.Scale = scale;
		mVPrototype.ItemID = itemID;
		prototypes.Add(id, mVPrototype);
		CreateRuntimePrototype(id);
		NotifyWorldInventoryChange();
	}

	public void RemovePrototype(int id)
	{
		prototypes.Remove(id);
		runtimePrototypes.Remove(id);
		NotifyWorldInventoryChange();
	}

	public void UnpendRuntimePrototype(int woId)
	{
		if (pendingRuntimePrototypes.ContainsKey(woId))
		{
			MVCubeModelInstance mVCubeModelInstance = (MVCubeModelInstance)MVGameController.Instance.WOCM.GetWorldObjectClient(woId);
			if (mVCubeModelInstance != null)
			{
				if (runtimePrototypes.ContainsKey(pendingRuntimePrototypes[woId].prevPrototypeId))
				{
					mVCubeModelInstance.PrototypeCubeModel = runtimePrototypes[pendingRuntimePrototypes[woId].prevPrototypeId];
					mVCubeModelInstance.PrototypeCubeModel.CreateInstance(mVCubeModelInstance);
				}
				else
				{
					Debug.LogError((object)"Trying to unpend but prev prototype does not exist anymore");
				}
			}
			else
			{
				Debug.Log((object)"Trying to unpend non existing cube model. This is probably because it was deleted");
			}
			pendingRuntimePrototypes.Remove(woId);
		}
		else
		{
			Debug.Log((object)"Trying to unpend runtime prototype but it is no longer pending. Probably because an event unpended it allready");
		}
	}

	public void OnReplaceWoPrototype(int woId, int worldInventoryId)
	{
		if (pendingRuntimePrototypes.ContainsKey(woId))
		{
			pendingRuntimePrototypes[woId].pendingRuntimePrototype.PrototypeId = worldInventoryId;
			if (!runtimePrototypes.ContainsKey(worldInventoryId))
			{
				runtimePrototypes.Add(worldInventoryId, pendingRuntimePrototypes[woId].pendingRuntimePrototype);
				pendingRuntimePrototypes[woId].pendingRuntimePrototype.PrototypeState = PrototypeState.Registered;
			}
			else
			{
				Debug.LogError((object)"Pending runtime prototype allready in worldInventory!");
			}
			pendingRuntimePrototypes.Remove(woId);
		}
		else
		{
			Debug.Log((object)"Create new prototype and update target cubemodel");
			MVCubeModelInstance mVCubeModelInstance = (MVCubeModelInstance)MVGameController.Instance.WOCM.WorldObjects[woId];
			int pid = mVCubeModelInstance.Pid;
			RuntimePrototypeCubeModel runtimePrototypeCubeModel = runtimePrototypes[pid].CloneGeometry();
			runtimePrototypeCubeModel.PrototypeId = worldInventoryId;
			runtimePrototypeCubeModel.PrototypeState = PrototypeState.Registered;
			runtimePrototypes.Add(worldInventoryId, runtimePrototypeCubeModel);
			mVCubeModelInstance.PrototypeCubeModel.RemoveInstance(woId);
			mVCubeModelInstance.PrototypeCubeModel = runtimePrototypeCubeModel;
			runtimePrototypeCubeModel.CreateInstance(mVCubeModelInstance);
		}
	}

	public void RequestWoMakeUniquePrototype(int woId)
	{
		MVCubeModelInstance mVCubeModelInstance = (MVCubeModelInstance)MVGameController.Instance.WOCM.GetWorldObjectClient(woId);
		if (runtimePrototypes[mVCubeModelInstance.Pid].InstancesCount == 1)
		{
			Debug.LogError((object)"The cubemodel is allready unique");
			return;
		}
		if (pendingRuntimePrototypes.ContainsKey(woId))
		{
			Debug.Log((object)"allready pending!");
			return;
		}
		ReplaceWithPendingRuntimePrototype(woId);
		MVGameController.Instance.Game.RequestWoUniquePrototype(woId);
	}

	private void ReplaceWithPendingRuntimePrototype(int woId)
	{
		MVCubeModelInstance mVCubeModelInstance = (MVCubeModelInstance)MVGameController.Instance.WOCM.GetWorldObjectClient(woId);
		RuntimePrototypeCubeModel prototypeCubeModel = mVCubeModelInstance.PrototypeCubeModel;
		RuntimePrototypeCubeModel runtimePrototypeCubeModel = CreatePendingPrototype(mVCubeModelInstance.Pid);
		prototypeCubeModel.DeltaCubes.Clear();
		prototypeCubeModel.RemoveInstance(woId);
		mVCubeModelInstance.PrototypeCubeModel = runtimePrototypeCubeModel;
		runtimePrototypeCubeModel.CreateInstance(mVCubeModelInstance);
		PendingPrototypeData value = new PendingPrototypeData(mVCubeModelInstance.Pid, runtimePrototypeCubeModel);
		pendingRuntimePrototypes.Add(woId, value);
	}

	private RuntimePrototypeCubeModel CreatePendingPrototype(int prototypeId)
	{
		RuntimePrototypeCubeModel runtimePrototypeCubeModel = MVGameController.Instance.WOCM.WorldInventory.RuntimePrototypes[prototypeId].CloneGeometry(withDeltaCubes: true);
		runtimePrototypeCubeModel.PrototypeState = PrototypeState.Pending;
		return runtimePrototypeCubeModel;
	}

	private void CreateRuntimePrototype(int id)
	{
		if (!prototypes.ContainsKey(id))
		{
			Debug.LogError((object)("Attempt to instantiate prototype with id '" + id + "', but no such id in dictionary"));
			return;
		}
		RuntimePrototypeCubeModel value = new RuntimePrototypeCubeModel(MVGameController.Instance.WOCM.WorldInventory.GetPrototype(id));
		runtimePrototypes.Add(id, value);
	}

	public RuntimePrototypeCubeModel GetUnchangedPrototypeByItem(MVItem item)
	{
		if (item.itemID == -1)
		{
			return null;
		}
		RuntimePrototypeCubeModel rpcm = new RuntimePrototypeCubeModel(item);
		foreach (KeyValuePair<int, RuntimePrototypeCubeModel> runtimePrototype in runtimePrototypes)
		{
			if (runtimePrototype.Value.MVItemId == item.itemID && runtimePrototype.Value.CompareGeometry(rpcm))
			{
				return runtimePrototype.Value;
			}
		}
		return null;
	}

	private void NotifyWorldInventoryChange()
	{
		if (OnWorldInventoryChange != null)
		{
			OnWorldInventoryChange(this);
		}
	}
}
