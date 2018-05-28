using System;
using UnityEngine;

namespace Borodar.FarlandSkies.CloudyCrownPro.DotParams;

[Serializable]
public class CelestialParamsList : SortedParamsList<CelestialParam>
{
	public CelestialParam GetParamPerTime(float currentTime)
	{
		if (SortedParams.Count <= 0)
		{
			Debug.LogWarning("Celestial params list is empty");
			SortedParams.Add(0f, new CelestialParam());
		}
		int num = SortedParams.FindIndexPerTime(currentTime);
		if (num < 1)
		{
			num = SortedParams.Count;
		}
		float num2 = SortedParams.Keys[num - 1];
		CelestialParam celestialParam = SortedParams.Values[num - 1];
		Color tintColor = celestialParam.TintColor;
		Color lightColor = celestialParam.LightColor;
		float lightIntencity = celestialParam.LightIntencity;
		if (num >= SortedParams.Count)
		{
			num = 0;
		}
		float num3 = SortedParams.Keys[num];
		celestialParam = SortedParams.Values[num];
		Color tintColor2 = celestialParam.TintColor;
		Color lightColor2 = celestialParam.LightColor;
		float lightIntencity2 = celestialParam.LightIntencity;
		float num4 = ((!(currentTime > num2)) ? (currentTime + (100f - num2)) : (currentTime - num2));
		float num5 = ((!(num2 < num3)) ? (100f + num3 - num2) : (num3 - num2));
		float t = num4 / num5;
		CelestialParam celestialParam2 = new CelestialParam();
		celestialParam2.TintColor = Color.Lerp(tintColor, tintColor2, t);
		celestialParam2.LightColor = Color.Lerp(lightColor, lightColor2, t);
		celestialParam2.LightIntencity = Mathf.Lerp(lightIntencity, lightIntencity2, t);
		return celestialParam2;
	}
}
