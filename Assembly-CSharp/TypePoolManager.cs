using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TypePoolManager : MonoBehaviour
{
	[SerializeField]
	private List<Pool> pool;

	private Dictionary<Type, Pool> poolAsDictionary;

	private void Awake()
	{
		for (int i = 0; i < pool.Count; i++)
		{
			pool[i].Initialize(transform);
		}
		poolAsDictionary = pool.ToDictionary((Pool p) => p.Prefab.GetType());
	}

	public Pool GetPool<T>() where T : MonoBehaviour
	{
		return poolAsDictionary[typeof(T)];
	}

	public T Instantiate<T>() where T : MonoBehaviour
	{
		return poolAsDictionary[typeof(T)].Instantiate<T>();
	}

	public void Return<T>(T obj) where T : MonoBehaviour
	{
		poolAsDictionary[typeof(T)].ReturnObject(obj);
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.A))
		{
			if (GetPool<Bullet>() == null)
			{
				Debug.LogError("Noooo!");
			}
			else
			{
				Debug.Log("YEAH!");
			}
		}
		if (Input.GetKeyDown(KeyCode.B))
		{
			if (GetPool<Bullet>() == null)
			{
				Debug.Log("Intended");
			}
			else
			{
				Debug.LogError("What?");
			}
		}
	}
}
