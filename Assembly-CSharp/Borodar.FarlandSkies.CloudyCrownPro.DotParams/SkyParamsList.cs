using System;
using UnityEngine;

namespace Borodar.FarlandSkies.CloudyCrownPro.DotParams;

[Serializable]
public class SkyParamsList : SortedParamsList<SkyParam>
{
	public SkyParam GetParamPerTime(float currentTime)
	{
		if (SortedParams.Count <= 0)
		{
			Debug.LogWarning("Sky params list is empty");
			SortedParams.Add(0f, new SkyParam());
		}
		int num = SortedParams.FindIndexPerTime(currentTime);
		if (num < 1)
		{
			num = SortedParams.Count;
		}
		float num2 = SortedParams.Keys[num - 1];
		SkyParam skyParam = SortedParams.Values[num - 1];
		Color topColor = skyParam.TopColor;
		Color bottomColor = skyParam.BottomColor;
		if (num >= SortedParams.Count)
		{
			num = 0;
		}
		float num3 = SortedParams.Keys[num];
		skyParam = SortedParams.Values[num];
		Color topColor2 = skyParam.TopColor;
		Color bottomColor2 = skyParam.BottomColor;
		float num4 = ((!(currentTime > num2)) ? (currentTime + (100f - num2)) : (currentTime - num2));
		float num5 = ((!(num2 < num3)) ? (100f + num3 - num2) : (num3 - num2));
		float t = num4 / num5;
		SkyParam skyParam2 = new SkyParam();
		skyParam2.TopColor = Color.Lerp(topColor, topColor2, t);
		skyParam2.BottomColor = Color.Lerp(bottomColor, bottomColor2, t);
		return skyParam2;
	}
}
