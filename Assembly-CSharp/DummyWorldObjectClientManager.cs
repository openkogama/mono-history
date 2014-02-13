using System.Collections.Generic;
using UnityEngine;

public static class DummyWorldObjectClientManager
{
	private static int dummyCounter = 0;

	private static Dictionary<int, DummyWorldObjectClient> dummyWos = new Dictionary<int, DummyWorldObjectClient>();

	private static Dictionary<int, int> gameObjectIdToWorldObjectIdMap = new Dictionary<int, int>();

	public static void AddDummyWorldObjectClient(DummyWorldObjectClient dummyWorldObjectClient)
	{
		dummyWorldObjectClient.Id = dummyCounter;
		dummyWos.Add(dummyCounter, dummyWorldObjectClient);
		gameObjectIdToWorldObjectIdMap.Add(((Object)dummyWorldObjectClient.GameObject).GetInstanceID(), dummyCounter);
		dummyCounter++;
	}

	public static bool Contains(int id)
	{
		return dummyWos.ContainsKey(id);
	}

	public static DummyWorldObjectClient GetDummyWorldObjectClient(int id)
	{
		if (!dummyWos.ContainsKey(id))
		{
			return null;
		}
		return dummyWos[id];
	}

	public static DummyWorldObjectClient GetWorldObjectByGoId(int goId)
	{
		if (gameObjectIdToWorldObjectIdMap.TryGetValue(goId, out var value))
		{
			return dummyWos[value];
		}
		return null;
	}

	public static DummyWorldObjectClient GetMVObject(Transform t)
	{
		if (gameObjectIdToWorldObjectIdMap.ContainsKey(((Object)((Component)t).gameObject).GetInstanceID()))
		{
			return GetWorldObjectByGoId(((Object)((Component)t).gameObject).GetInstanceID());
		}
		if ((Object)(object)t.parent != (Object)null)
		{
			return GetMVObject(t.parent);
		}
		return null;
	}
}
