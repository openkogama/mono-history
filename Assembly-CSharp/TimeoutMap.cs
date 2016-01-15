using System.Collections.Generic;
using UnityEngine;

public class TimeoutMap
{
	private readonly float timeOut;

	private Dictionary<int, float> weaponTimeOutMap = new Dictionary<int, float>();

	private HashSet<int> removeSet = new HashSet<int>();

	public TimeoutMap(float timeOut)
	{
		this.timeOut = timeOut;
	}

	public void Update()
	{
		removeSet.Clear();
		foreach (KeyValuePair<int, float> item in weaponTimeOutMap)
		{
			if (item.Value + timeOut <= Time.time)
			{
				removeSet.Add(item.Key);
			}
		}
		foreach (int item2 in removeSet)
		{
			weaponTimeOutMap.Remove(item2);
		}
	}

	public void Add(int id)
	{
		weaponTimeOutMap[id] = Time.time;
	}

	public bool Contains(int id)
	{
		return weaponTimeOutMap.ContainsKey(id);
	}
}
