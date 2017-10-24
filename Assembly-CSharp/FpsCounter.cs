using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class FpsCounter : MonoBehaviour
{
	private class FPSMetricCollector
	{
		private readonly float startTime;

		private const float timeBeforeMetricCollectionInSeconds = 30f;

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
			StatHatWrapper.Value("FPS", Mathf.Clamp(averageFPS, 0f, 60f));
			metricsCollected = true;
			StatHatWrapper.Value("RoundTripTime", MVGameControllerBase.Game.Peer.RoundTripTime);
		}
	}

	public static float Fps;

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
		Fps = frameTimes.Average();
		if (MVGameControllerBase.JoinState == MVJoinState.Playing)
		{
			if (metricsCollector == null)
			{
				metricsCollector = new FPSMetricCollector();
			}
			if (!metricsCollector.IsFPSCollected && metricsCollector.IsTimeForCollect)
			{
				metricsCollector.CollectFPSMetric(Fps);
			}
		}
	}
}
public class FPSCounter : MonoBehaviour
{
	[SerializeField]
	private Text fpsText;

	private float frameUpdateRate = 0.5f;

	private float currTime;

	private void Update()
	{
		currTime += Time.deltaTime;
		if (currTime > frameUpdateRate)
		{
			fpsText.text = FpsCounter.Fps.ToString("F0");
			currTime = 0f;
		}
	}
}
