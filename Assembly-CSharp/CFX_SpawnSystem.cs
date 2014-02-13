using System.Collections.Generic;
using UnityEngine;

public class CFX_SpawnSystem : MonoBehaviour
{
	private static CFX_SpawnSystem instance;

	public GameObject[] objectsToPreload = new GameObject[0];

	public int[] objectsToPreloadTimes = new int[0];

	public bool hideObjectsInHierarchy;

	private bool allObjectsLoaded;

	private Dictionary<int, List<GameObject>> instantiatedObjects = new Dictionary<int, List<GameObject>>();

	private Dictionary<int, int> poolCursors = new Dictionary<int, int>();

	public static bool AllObjectsLoaded => instance.allObjectsLoaded;

	public static GameObject GetNextObject(GameObject sourceObj, bool activateObject = true)
	{
		int instanceID = ((Object)sourceObj).GetInstanceID();
		if (!instance.poolCursors.ContainsKey(instanceID))
		{
			Debug.LogError((object)("[CFX_SpawnSystem.GetNextPoolObject()] Object hasn't been preloaded: " + ((Object)sourceObj).name + " (ID:" + instanceID + ")"));
			return null;
		}
		int index = instance.poolCursors[instanceID];
		Dictionary<int, int> dictionary2;
		Dictionary<int, int> dictionary = (dictionary2 = instance.poolCursors);
		int key2;
		int key = (key2 = instanceID);
		key2 = dictionary2[key2];
		dictionary[key] = key2 + 1;
		if (instance.poolCursors[instanceID] >= instance.instantiatedObjects[instanceID].Count)
		{
			instance.poolCursors[instanceID] = 0;
		}
		GameObject val = instance.instantiatedObjects[instanceID][index];
		if (activateObject)
		{
			val.SetActiveRecursively(true);
		}
		return val;
	}

	public static void PreloadObject(GameObject sourceObj, int poolSize = 1)
	{
		instance.addObjectToPool(sourceObj, poolSize);
	}

	public static void UnloadObjects(GameObject sourceObj)
	{
		instance.removeObjectsFromPool(sourceObj);
	}

	private void addObjectToPool(GameObject sourceObject, int number)
	{
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Expected Obj, but got Unknown
		int instanceID = ((Object)sourceObject).GetInstanceID();
		if (!instantiatedObjects.ContainsKey(instanceID))
		{
			instantiatedObjects.Add(instanceID, new List<GameObject>());
			poolCursors.Add(instanceID, 0);
		}
		for (int i = 0; i < number; i++)
		{
			GameObject val = (GameObject)Object.Instantiate((Object)(object)sourceObject);
			val.SetActiveRecursively(false);
			CFX_AutoDestructShuriken[] componentsInChildren = val.GetComponentsInChildren<CFX_AutoDestructShuriken>(true);
			CFX_AutoDestructShuriken[] array = componentsInChildren;
			foreach (CFX_AutoDestructShuriken cFX_AutoDestructShuriken in array)
			{
				cFX_AutoDestructShuriken.OnlyDeactivate = true;
			}
			CFX_LightIntensityFade[] componentsInChildren2 = val.GetComponentsInChildren<CFX_LightIntensityFade>(true);
			CFX_LightIntensityFade[] array2 = componentsInChildren2;
			foreach (CFX_LightIntensityFade cFX_LightIntensityFade in array2)
			{
				cFX_LightIntensityFade.autodestruct = false;
			}
			instantiatedObjects[instanceID].Add(val);
			if (hideObjectsInHierarchy)
			{
				((Object)val).hideFlags = (HideFlags)1;
			}
		}
	}

	private void removeObjectsFromPool(GameObject sourceObject)
	{
		int instanceID = ((Object)sourceObject).GetInstanceID();
		if (!instantiatedObjects.ContainsKey(instanceID))
		{
			Debug.LogWarning((object)("[CFX_SpawnSystem.removeObjectsFromPool()] There aren't any preloaded object for: " + ((Object)sourceObject).name + " (ID:" + instanceID + ")"));
			return;
		}
		for (int num = instantiatedObjects[instanceID].Count - 1; num >= 0; num--)
		{
			GameObject val = instantiatedObjects[instanceID][num];
			instantiatedObjects[instanceID].RemoveAt(num);
			Object.Destroy((Object)(object)val);
		}
		instantiatedObjects.Remove(instanceID);
		poolCursors.Remove(instanceID);
	}

	private void Awake()
	{
		if ((Object)(object)instance != (Object)null)
		{
			Debug.LogWarning((object)"CFX_SpawnSystem: There should only be one instance of CFX_SpawnSystem per Scene!");
		}
		instance = this;
	}

	private void Start()
	{
		allObjectsLoaded = false;
		for (int i = 0; i < objectsToPreload.Length; i++)
		{
			PreloadObject(objectsToPreload[i], objectsToPreloadTimes[i]);
		}
		allObjectsLoaded = true;
	}
}
