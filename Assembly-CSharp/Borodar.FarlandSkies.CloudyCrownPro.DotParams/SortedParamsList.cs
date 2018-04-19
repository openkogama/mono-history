using System;

namespace Borodar.FarlandSkies.CloudyCrownPro.DotParams;

[Serializable]
public abstract class SortedParamsList<T> where T : DotParam
{
	public T[] Params = new T[0];

	protected DotParamsList<T> SortedParams;

	public void Init()
	{
		SortedParams = new DotParamsList<T>(Params.Length);
		T[] array = Params;
		foreach (T val in array)
		{
			SortedParams[val.Time] = val;
		}
	}

	public void Update()
	{
		if (SortedParams == null)
		{
			SortedParams = new DotParamsList<T>(Params.Length);
		}
		else
		{
			SortedParams.Clear();
		}
		T[] array = Params;
		foreach (T val in array)
		{
			SortedParams[val.Time] = val;
		}
	}
}
