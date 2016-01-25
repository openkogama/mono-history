using System.Linq;
using UnityEngine;

public class FpsCounter : MonoBehaviour
{
	private class FPSMetricCollector
	{
		private const float timeBeforeMetricCollectionInSeconds = 30f;

		private readonly float startTime;

		private bool metricsCollected;

		public bool IsFPSCollected => metricsCollected;

		public bool IsTimeForCollect => Time.time - startTime > 30f;

		public FPSMetricCollector()
		{
			startTime = Time.time;
			metricsCollected = false;
		}

		public void CollectFPSMetric(float averageFPS)
		{
			StatHatWrapper.Value("FPS", averageFPS);
			metricsCollected = true;
		}
	}

	private int idx;

	private float[] frameTimes = new float[10];

	private FPSMetricCollector metricsCollector;

	private void Start()
	{
		Object.DontDestroyOnLoad(gameObject);
	}

	private void Update()
	{
		frameTimes[idx] = 1f / Time.deltaTime;
		idx = (idx + 1) % frameTimes.Length;
		if (MVGameControllerBase.JoinState == MVJoinState.Playing)
		{
			if (metricsCollector == null)
			{
				metricsCollector = new FPSMetricCollector();
			}
			if (!metricsCollector.IsFPSCollected && metricsCollector.IsTimeForCollect)
			{
				metricsCollector.CollectFPSMetric(frameTimes.Average());
			}
		}
	}
}
