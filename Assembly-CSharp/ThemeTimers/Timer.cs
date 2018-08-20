using UnityEngine;

namespace ThemeTimers;

public class Timer : ITimer
{
	private float timeScale;

	public float Time { get; private set; }

	public Timer(float initialTime, float cycleLength)
	{
		Time = initialTime;
		timeScale = 100f / cycleLength;
	}

	public void Update()
	{
		Time += UnityEngine.Time.deltaTime * timeScale;
		while (Time > 100f)
		{
			Time -= 100f;
		}
	}
}
