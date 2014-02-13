using UnityEngine;

public class IntervalWithRandomSeed
{
	private float range;

	private float currentDeltaTime;

	private bool newIteration;

	public IntervalWithRandomSeed(float interval)
	{
		range = interval;
		currentDeltaTime = Random.Range(0f, interval);
	}

	public bool Update()
	{
		bool result = false;
		currentDeltaTime += Time.deltaTime;
		if (newIteration && currentDeltaTime >= range)
		{
			result = true;
			newIteration = false;
		}
		WrapDeltaTime();
		return result;
	}

	private void WrapDeltaTime()
	{
		float num = currentDeltaTime - range;
		if (num > 0f)
		{
			newIteration = true;
			currentDeltaTime = num;
			WrapDeltaTime();
		}
	}
}
