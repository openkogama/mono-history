using System;
using UnityEngine;

public class DynamicLODDistance
{
	private float volumePercentChangePrSecond = 0.1f;

	private int prevTick = WaitForTicks.GetEnvironmentTick(0);

	private float currentRadius;

	private readonly float maxVolume;

	public readonly float minRadius;

	public readonly float maxRadius;

	public readonly int maxNumObjects;

	public float CurrentRadius => currentRadius;

	public DynamicLODDistance(float minRadius, float maxRadius, int maxNumObjects)
	{
		if (minRadius <= 0f)
		{
			throw new Exception("minRadius <= 0.0f");
		}
		if (maxRadius <= 0f)
		{
			throw new Exception("maxRadius <= 0.0f");
		}
		if (maxNumObjects <= 0)
		{
			throw new Exception("maxNumObjects <= 0");
		}
		this.minRadius = minRadius;
		this.maxRadius = maxRadius;
		this.maxNumObjects = maxNumObjects;
		maxVolume = RadiusToVolume(maxRadius);
		currentRadius = minRadius;
	}

	public void Update(int numObjects)
	{
		float num = (float)numObjects / (float)maxNumObjects;
		float deltaTime = GetDeltaTime();
		float volume;
		if (num > 0f)
		{
			volume = maxVolume / num;
		}
		else
		{
			volume = RadiusToVolume(currentRadius);
			float num2 = volumePercentChangePrSecond * deltaTime * maxVolume;
			volume += num2;
		}
		currentRadius = VolumeToRadius(volume);
		currentRadius = Mathf.Clamp(currentRadius, minRadius, maxRadius);
	}

	public float GetDeltaTime()
	{
		int environmentTick = WaitForTicks.GetEnvironmentTick(0);
		int num = WaitForTicks.Diff(prevTick);
		prevTick = environmentTick;
		return (float)num / 1000f;
	}

	private static float RadiusToVolume(float radius)
	{
		return (float)Math.PI * radius * radius * radius;
	}

	private static float VolumeToRadius(float volume)
	{
		return Mathf.Pow(volume / 1f / (float)Math.PI, 1f / 3f);
	}

	private float GetTargetVolume(float numObjectsMaxObjectsRatio)
	{
		if (numObjectsMaxObjectsRatio == 0f)
		{
			return maxVolume;
		}
		return maxVolume / numObjectsMaxObjectsRatio;
	}

	public static void Test()
	{
		MathTest();
		UpdateTest();
		TickTest();
	}

	private static void TickTest()
	{
		float num = 1f;
		float num2 = 100f;
		int num3 = 10;
		DynamicLODDistance dynamicLODDistance = new DynamicLODDistance(num, num2, num3);
		for (int i = 0; i < 5; i++)
		{
			Debug.Log("DeltaTime: " + dynamicLODDistance.GetDeltaTime());
		}
	}

	private static void UpdateTest()
	{
		float num = 1f;
		float num2 = 100f;
		int num3 = 10;
		DynamicLODDistance dynamicLODDistance = new DynamicLODDistance(num, num2, num3);
		dynamicLODDistance.prevTick = WaitForTicks.GetEnvironmentTick(0);
		dynamicLODDistance.Update(0);
		for (int i = 0; i < 6; i++)
		{
			dynamicLODDistance.Update(num3 - 1);
			Debug.Log("Radius: " + dynamicLODDistance.CurrentRadius);
		}
		for (int j = 0; j < 6; j++)
		{
			dynamicLODDistance.Update(num3 + 1);
			Debug.Log("Radius: " + dynamicLODDistance.CurrentRadius);
		}
	}

	private static void MathTest()
	{
		float num = 10f;
		Debug.Log("Base radius: " + num);
		float num2 = RadiusToVolume(num);
		Debug.Log("Area: " + num2);
		Debug.Log("VolumeToRadius: " + VolumeToRadius(num2));
		float num3 = Mathf.Abs(VolumeToRadius(num2) - num);
		if (num3 > 1E-05f)
		{
			throw new Exception("Failed math test");
		}
	}
}
