using System;
using UnityEngine;

namespace Borodar.FarlandSkies.CloudyCrownPro.DotParams;

[Serializable]
public class StarsParamsList : SortedParamsList<StarsParam>
{
	public StarsParam GetParamPerTime(float currentTime)
	{
		if (SortedParams.Count <= 0)
		{
			Debug.LogWarning("Stars params list is empty");
			SortedParams.Add(0f, new StarsParam());
		}
		int num = SortedParams.FindIndexPerTime(currentTime);
		if (num < 1)
		{
			num = SortedParams.Count;
		}
		float num2 = SortedParams.Keys[num - 1];
		StarsParam starsParam = SortedParams.Values[num - 1];
		Color tintColor = starsParam.TintColor;
		if (num >= SortedParams.Count)
		{
			num = 0;
		}
		float num3 = SortedParams.Keys[num];
		starsParam = SortedParams.Values[num];
		Color tintColor2 = starsParam.TintColor;
		float num4 = ((!(currentTime > num2)) ? (currentTime + (100f - num2)) : (currentTime - num2));
		float num5 = ((!(num2 < num3)) ? (100f + num3 - num2) : (num3 - num2));
		float t = num4 / num5;
		StarsParam starsParam2 = new StarsParam();
		starsParam2.TintColor = Color.Lerp(tintColor, tintColor2, t);
		return starsParam2;
	}
}
