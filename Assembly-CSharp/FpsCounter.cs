using System.Linq;
using MV.Common;
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
			float num = Mathf.Clamp(averageFPS, 0f, 60f);
			if (num < 20f)
			{
				StatHatWrapper.Count("FPSBucket0-20", 1);
			}
			else if (num < 30f)
			{
				StatHatWrapper.Count("FPSBucket20-30", 1);
			}
			else
			{
				StatHatWrapper.Count("FPSBucket30+", 1);
			}
			StatHatWrapper.Value("FPS", num);
			metricsCollected = true;
			StatHatWrapper.Value("RoundTripTime", MVGameControllerBase.Game.Peer.RoundTripTime);
			Debug.Log("FPS");
			MVGameControllerBase.OperationRequests.IncrementStatRequest(IncrementStatRequestType.FPS, (int)num);
		}
	}

	private static FpsCounter instance;

	private int idx;

	private float[] frameTimes = new float[10];

	private FPSMetricCollector metricsCollector;

	private float fps;

	public static float Fps => instance.fps;

	protected void Awake()
	{
		instance = this;
	}

	protected void OnDestroy()
	{
		instance = null;
	}

	protected void Update()
	{
		frameTimes[idx] = 1f / Time.deltaTime;
		idx = (idx + 1) % frameTimes.Length;
		fps = frameTimes.Average();
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
