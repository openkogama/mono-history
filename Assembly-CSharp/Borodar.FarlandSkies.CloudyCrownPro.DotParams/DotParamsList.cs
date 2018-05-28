using System;
using System.Collections.Generic;

namespace Borodar.FarlandSkies.CloudyCrownPro.DotParams;

public class DotParamsList<T> : SortedList<float, T>
{
	public DotParamsList(int capacity)
		: base(capacity)
	{
	}

	public int FindIndexPerTime(float time)
	{
		return BinarySearch(Keys, time);
	}

	private static int BinarySearch<T>(IList<T> list, T value)
	{
		if (list == null)
		{
			throw new ArgumentNullException("list");
		}
		Comparer<T> comparer = Comparer<T>.Default;
		int num = 0;
		int num2 = list.Count - 1;
		while (num < num2)
		{
			int num3 = (num2 + num) / 2;
			if (comparer.Compare(list[num3], value) < 0)
			{
				num = num3 + 1;
			}
			else
			{
				num2 = num3 - 1;
			}
		}
		if (comparer.Compare(list[num], value) < 0)
		{
			num++;
		}
		return num;
	}
}
