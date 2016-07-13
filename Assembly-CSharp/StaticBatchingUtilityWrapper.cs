using System.Collections.Generic;
using UnityEngine;

public static class StaticBatchingUtilityWrapper
{
	public const bool enableStaticBatching = false;

	private static List<GameObject> staticGameObjects = new List<GameObject>();

	public static void Add(GameObject go)
	{
		staticGameObjects.Add(go);
		go.SetActive(value: false);
		go.SetActive(value: true);
	}

	public static void Combine()
	{
		StaticBatchingUtility.Combine(staticGameObjects.ToArray(), MVGameControllerBase.WOCM.GetSingletonWorldObject<MVCubeModelPrototypeTerrain>().GameObject);
	}
}
