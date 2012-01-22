using System;
using System.Collections;
using UnityEngine;

public class pTween
{
	public static IEnumerator To(float duration, float startValue, float endValue, Action<float> callback)
	{
		float start = Time.time;
		float end = start + duration;
		float durationInv = 1f / duration;
		float startMulDurationInv = start / duration;
		for (float t = Time.time; t < end; t = Time.time)
		{
			callback(Mathf.Lerp(startValue, endValue, t * durationInv - startMulDurationInv));
			yield return 0;
		}
		callback(endValue);
	}

	public static IEnumerator RealtimeTo(float duration, float startValue, float endValue, Action<float> callback)
	{
		float start = Time.realtimeSinceStartup;
		float end = start + duration;
		float durationInv = 1f / duration;
		float startMulDurationInv = start / duration;
		for (float t = Time.realtimeSinceStartup; t < end; t = Time.realtimeSinceStartup)
		{
			callback(Mathf.Lerp(startValue, endValue, t * durationInv - startMulDurationInv));
			yield return 0;
		}
		callback(endValue);
	}

	public static IEnumerator To(float duration, Action<float> callback)
	{
		return To(duration, 0f, 1f, callback);
	}

	public static void WorkerTo(float duration, float startValue, float endValue, Action<float> callback)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		new GameObject().AddComponent<Worker>().To(duration, startValue, endValue, callback);
	}

	public static void WorkerTo(float duration, Action<float> callback)
	{
		WorkerTo(duration, 0f, 1f, callback);
	}
}
