using System;
using System.Collections.Generic;
using UnityEngine;

public class TransformNetworkManager
{
	private Dictionary<int, MVNetworkObject> networkedObjects = new Dictionary<int, MVNetworkObject>();

	private List<int> removeList = new List<int>();

	public const int broadcastInterval = 200;

	public const int clientDelay = 200;

	public static int DelayedTime { get; private set; }

	public void RemoveNetworkObject(int woID)
	{
		networkedObjects.Remove(woID);
	}

	public void AddReporter(int woID, MVNetworkReporter networkReporter)
	{
		if (networkedObjects.ContainsKey(woID))
		{
			throw new Exception("Trying to add reporter while network object already in place " + networkedObjects[woID]);
		}
		networkedObjects.Add(woID, networkReporter);
	}

	public MVNetworkObject GetNetworkObject(int woID)
	{
		if (networkedObjects.ContainsKey(woID))
		{
			return networkedObjects[woID];
		}
		return null;
	}

	public void AddTransformPackage(int woID, NetworkTransformPackage p)
	{
		MVWorldObjectClient worldObjectClient = MVGameControllerBase.WOCM.GetWorldObjectClient(woID);
		if (worldObjectClient == null)
		{
			Debug.LogError("Attempt to update world object, but object not registered in world");
			return;
		}
		if (!networkedObjects.ContainsKey(woID))
		{
			networkedObjects.Add(woID, new MVNetworkListener(worldObjectClient));
		}
		MVNetworkObject mVNetworkObject = networkedObjects[woID];
		if (mVNetworkObject != null && mVNetworkObject.GetType() == typeof(MVNetworkListener))
		{
			(mVNetworkObject as MVNetworkListener).AddTransformPackage(p);
		}
		else if (mVNetworkObject != null)
		{
			Debug.LogWarning(string.Concat("worldObjectClientManager.WorldObjects[worldObjectID].NetworkObject is ", mVNetworkObject.GetType(), " this is probably due to ownership switching of vehicle"));
		}
	}

	public void Update(MVNetworkGame game)
	{
		DelayedTime = game.ServerTimeInMilliSeconds - 200 - 200 - 200;
		foreach (KeyValuePair<int, MVNetworkObject> networkedObject in networkedObjects)
		{
			MVWorldObjectClient worldObjectClient = MVGameControllerBase.WOCM.GetWorldObjectClient(networkedObject.Key);
			if (worldObjectClient != null)
			{
				networkedObject.Value.Update(game);
				if (networkedObject.Value.RemoveFromUpdate)
				{
					removeList.Add(networkedObject.Key);
				}
			}
			else
			{
				removeList.Add(networkedObject.Key);
			}
		}
		foreach (int remove in removeList)
		{
			networkedObjects.Remove(remove);
		}
		removeList.Clear();
	}
}
