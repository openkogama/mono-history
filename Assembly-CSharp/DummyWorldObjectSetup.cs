using System;
using System.Collections.Generic;
using UnityEngine;

public class DummyWorldObjectSetup : MonoBehaviour
{
	public List<GameObject> dummyWorldObjects = new List<GameObject>();

	private void Awake()
	{
		foreach (GameObject dummyWorldObject in dummyWorldObjects)
		{
			DummyWorldObjectClientManager.AddDummyWorldObjectClient(new DummyWorldObjectClient(dummyWorldObject));
		}
		Debug.Log(CalcOffset(3, 5, 9));
	}

	public int CalcOffset(int curTime, int offset, int range)
	{
		int num = curTime % range;
		Debug.Log(num);
		if (num > offset)
		{
			offset = range - (num - offset);
			return offset;
		}
		if (num < offset)
		{
			offset -= num;
			return offset;
		}
		return offset;
	}

	public int GetIntWithinRange(int seed, int range)
	{
		seed = Math.Abs(Noise(seed));
		return seed % range;
	}

	private int Noise(int seed)
	{
		seed = 36969 * (seed & 0xFFFF) + (seed >> 16);
		seed = 18000 * (seed & 0xFFFF) + (seed >> 16);
		return (seed << 16) + seed;
	}
}
