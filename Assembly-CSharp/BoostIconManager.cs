using System.Collections.Generic;
using UnityEngine;

public class BoostIconManager : MonoBehaviour
{
	[SerializeField]
	private List<BoosterIcon> boosterIconPrefabs;

	public GameObject CreateBoosterIcon(BoostType type)
	{
		for (int i = 0; i < boosterIconPrefabs.Count; i++)
		{
			if (boosterIconPrefabs[i].type == type)
			{
				return Object.Instantiate(boosterIconPrefabs[i].icon);
			}
		}
		Debug.LogError("BoostIconManager couldn't find icon for " + type.ToString() + " boost");
		return null;
	}
}
