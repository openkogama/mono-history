using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Pool
{
	[SerializeField]
	private MonoBehaviour prefab;

	[SerializeField]
	private int poolSize;

	[SerializeField]
	private PoolEnums poolEnum;

	private Transform parent;

	private List<int> available;

	private MonoBehaviour[] pool;

	public int ObjectsAvailable => available.Count;

	public PoolEnums PoolType => poolEnum;

	public MonoBehaviour Prefab => prefab;

	public MonoBehaviour Next
	{
		get
		{
			if (ObjectsAvailable > 0)
			{
				int num = available[ObjectsAvailable - 1];
				available.RemoveAt(ObjectsAvailable - 1);
				pool[num].gameObject.SetActive(value: true);
				return pool[num];
			}
			return UnityEngine.Object.Instantiate(prefab);
		}
	}

	public MonoBehaviour Return
	{
		set
		{
			for (int i = 0; i < pool.Length; i++)
			{
				if (pool[i] == value)
				{
					value.gameObject.SetActive(value: false);
					value.transform.parent = parent;
					available.Add(i);
					return;
				}
			}
			UnityEngine.Object.Destroy(value.gameObject);
		}
	}

	public T Instantiate<T>() where T : MonoBehaviour
	{
		return Next as T;
	}

	public void ReturnObject(MonoBehaviour obj)
	{
		for (int i = 0; i < pool.Length; i++)
		{
			if (pool[i] == obj)
			{
				obj.gameObject.SetActive(value: false);
				obj.transform.parent = parent;
				available.Add(i);
				return;
			}
		}
		UnityEngine.Object.Destroy(obj.gameObject);
	}

	public void Initialize(Transform parent)
	{
		this.parent = parent;
		pool = new MonoBehaviour[poolSize];
		available = new List<int>(poolSize);
		for (int i = 0; i < poolSize; i++)
		{
			pool[i] = UnityEngine.Object.Instantiate(prefab);
			available.Add(i);
			pool[i].gameObject.SetActive(value: false);
			pool[i].transform.parent = parent;
		}
	}
}
