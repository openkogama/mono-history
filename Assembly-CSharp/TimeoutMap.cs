using System.Collections.Generic;
using UnityEngine;

public class TimeoutMap
{
	private readonly float timeOut;

	private Dictionary<int, float> weaponTimeOutMap = new Dictionary<int, float>();

	public TimeoutMap(float timeOut)
	{
		this.timeOut = timeOut;
	}

	public void Update()
	{
		HashSet<int> hashSet = new HashSet<int>();
		foreach (KeyValuePair<int, float> item in weaponTimeOutMap)
		{
			if (item.Value + timeOut <= Time.time)
			{
				hashSet.Add(item.Key);
			}
		}
		foreach (int item2 in hashSet)
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
