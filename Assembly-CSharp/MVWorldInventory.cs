using System.Collections;
using System.Collections.Generic;
using MV.WorldObject;
using UnityEngine;

public class MVWorldInventory
{
	public delegate void OnWorldInventoryChangeDelegate(MVWorldInventory inventory);

	private const int numberOfLowPriorityMeshGenerations = 1;

	private Dictionary<int, RuntimePrototypeCubeModel> runtimePrototypes = new Dictionary<int, RuntimePrototypeCubeModel>();

	private Dictionary<int, PendingPrototypeData> pendingRuntimePrototypes = new Dictionary<int, PendingPrototypeData>();

	private List<RuntimePrototypeCubeModel> dirtyRPCM = new List<RuntimePrototypeCubeModel>();

	public OnWorldInventoryChangeDelegate OnWorldInventoryChange;

	public Dictionary<int, RuntimePrototypeCubeModel> RuntimePrototypes => runtimePrototypes;

	public void AddRuntimePrototypeToDirty(RuntimePrototypeCubeModel rpcm)
	{
		dirtyRPCM.Add(rpcm);
	}

	public void OnUpdatePrototypeEvent(int worldInventoryID, byte[] worldInventoryData)
	{
		RuntimePrototypeCubeModel runtimePrototypeCubeModel = runtimePrototypes[worldInventoryID];
		runtimePrototypeCubeModel.UpdatePrototype(new BytePacker(worldInventoryData));
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

	public void AddPrototype(Hashtable data)
	{
		int num = (int)data[PrototypeDataParameters.Id];
		float scale = (float)data[PrototypeDataParameters.Scale];
		int authorProfileId = (int)data[PrototypeDataParameters.AuthorProfileId];
		byte[] data2 = (byte[])data[PrototypeDataParameters.Data];
		RuntimePrototypeCubeModel value = new RuntimePrototypeCubeModel(num, authorProfileId, scale, data2);
		runtimePrototypes.Add(num, value);
		NotifyWorldInventoryChange();
	}

	public void RemovePrototype(int id)
	{
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
				MVCubeModelInstance mVCubeModelInstance = (MVCubeModelInstance)MVGameController.Instance.WOCM.GetWorldObjectClient(woId);
				mVCubeModelInstance.Data["protoTypeID"] = worldInventoryId;
			}
			else
			{
				Debug.LogError((object)"Pending runtime prototype allready in worldInventory!");
			}
			pendingRuntimePrototypes.Remove(woId);
		}
		else
		{
			MVCubeModelInstance mVCubeModelInstance2 = (MVCubeModelInstance)MVGameController.Instance.WOCM.GetWorldObjectClient(woId);
			int pid = mVCubeModelInstance2.Pid;
			RuntimePrototypeCubeModel runtimePrototypeCubeModel = runtimePrototypes[pid].CloneGeometry();
			runtimePrototypeCubeModel.PrototypeId = worldInventoryId;
			runtimePrototypeCubeModel.PrototypeState = PrototypeState.Registered;
			runtimePrototypes.Add(worldInventoryId, runtimePrototypeCubeModel);
			mVCubeModelInstance2.PrototypeCubeModel.RemoveInstance(woId);
			mVCubeModelInstance2.PrototypeCubeModel = runtimePrototypeCubeModel;
			runtimePrototypeCubeModel.CreateInstance(mVCubeModelInstance2);
			mVCubeModelInstance2.Data["protoTypeID"] = worldInventoryId;
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
		RuntimePrototypeCubeModel runtimePrototypeCubeModel = runtimePrototypes[prototypeId].CloneGeometry(withDeltaCubes: true);
		runtimePrototypeCubeModel.PrototypeState = PrototypeState.Pending;
		return runtimePrototypeCubeModel;
	}

	private void NotifyWorldInventoryChange()
	{
		if (OnWorldInventoryChange != null)
		{
			OnWorldInventoryChange(this);
		}
	}
}
