using System.Collections.Generic;
using UnityEngine;

public class EnumPoolManager : MonoBehaviour
{
	[SerializeField]
	private List<Pool> pool;

	private Pool[] lookupTable = new Pool[26];

	private void Awake()
	{
		for (int i = 0; i < pool.Count; i++)
		{
			pool[i].Initialize(transform);
			if (lookupTable[(int)pool[i].PoolType] == null)
			{
				lookupTable[(int)pool[i].PoolType] = pool[i];
			}
			else
			{
				Debug.LogError(string.Format("A pool of type %s does already exist!", pool[i].PoolType.ToString()));
			}
		}
	}

	public Pool GetPool(PoolEnums pEnum)
	{
		return lookupTable[(int)pEnum];
	}

	public T Instantiate<T>(PoolEnums pEnum) where T : MonoBehaviour
	{
		return lookupTable[(int)pEnum].Instantiate<T>();
	}

	public void Return(MonoBehaviour obj, PoolEnums pEnum)
	{
		lookupTable[(int)pEnum].ReturnObject(obj);
	}
}
