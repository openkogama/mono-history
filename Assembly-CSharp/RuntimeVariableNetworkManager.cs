using System;
using System.Collections.Generic;
using UnityEngine;

public class RuntimeVariableNetworkManager
{
	private HashSet<int> runtimeDataVariables = new HashSet<int>();

	private List<int> removeList = new List<int>();

	public void AddRuntimeDataVariables(int woID)
	{
		if (runtimeDataVariables.Contains(woID))
		{
			throw new Exception("RuntimeDataVariables allready exists");
		}
		runtimeDataVariables.Add(woID);
	}

	public bool ContainsRuntimeVariables(int woID)
	{
		return runtimeDataVariables.Contains(woID);
	}

	public void RemoveRuntimeDataVariables(int woID)
	{
		if (!runtimeDataVariables.Contains(woID))
		{
			throw new Exception("wo Id not found");
		}
		runtimeDataVariables.Remove(woID);
	}

	public void SendRuntimeData()
	{
		foreach (int runtimeDataVariable in runtimeDataVariables)
		{
			MVWorldObjectClient worldObjectClient = MVGameControllerBase.WOCM.GetWorldObjectClient(runtimeDataVariable);
			if (SendRuntimeData(worldObjectClient, immediateSend: false))
			{
				removeList.Add(runtimeDataVariable);
			}
		}
		foreach (int remove in removeList)
		{
			runtimeDataVariables.Remove(remove);
		}
		removeList.Clear();
	}

	public bool SendRuntimeData(MVWorldObjectClient wo, bool immediateSend)
	{
		if (wo == null)
		{
			Debug.LogError("Attempt to update world object, but object not registered in world");
			return true;
		}
		Dictionary<object, object> dictionary = wo.RuntimeDataVariables.Send(immediateSend);
		if (dictionary.Count > 0)
		{
			MVGameControllerBase.OperationRequests.UpdateWorldObjectRunTimeData(wo.Id, dictionary);
		}
		return false;
	}
}
