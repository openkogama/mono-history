using System.Linq;
using UnityEngine;

public class FpsCounter : MonoBehaviour
{
	private class FpsMetricCollector
	{
		private const float timeBeforeMetricCollectionInSeconds = 30f;

		private readonly float startTime;

		private bool fpsCollected;

		public FpsMetricCollector()
		{
			startTime = Time.time;
		}

		public void Update(float fps)
		{
			if (!fpsCollected && Time.time - startTime > 30f)
			{
				StatHatWrapper.Value("FPS", fps);
				fpsCollected = true;
			}
		}
	}

	private int idx;

	private float[] frameTimes = new float[10];

	public bool showFPS = true;

	private FpsMetricCollector fpsMetricCollector;

	private void Start()
	{
		Object.DontDestroyOnLoad(gameObject);
	}

	private void Update()
	{
		frameTimes[idx] = 1f / Time.deltaTime;
		idx = (idx + 1) % frameTimes.Length;
		if (Input.GetKey(KeyCode.Alpha8) && Input.GetKeyUp(KeyCode.Alpha9))
		{
			showFPS = !showFPS;
		}
		if (fpsMetricCollector == null && MVGameController.Game != null && MVGameController.Game.JoinState == MVJoinState.Playing)
		{
			fpsMetricCollector = new FpsMetricCollector();
		}
		if (fpsMetricCollector != null)
		{
			fpsMetricCollector.Update(frameTimes.Average());
		}
	}

	private void OnGUI()
	{
		if (showFPS)
		{
			float num = frameTimes.Min();
			float num2 = frameTimes.Max();
			float num3 = frameTimes.Average();
			GUI.BeginGroup(new Rect(5f, 5f, 80f, 70f), new GUIStyle("box"));
			GUI.Label(new Rect(5f, 5f, 80f, 20f), $"Min: {num:0.0}");
			GUI.Label(new Rect(5f, 25f, 80f, 20f), $"Avg: {num3:0.0}");
			GUI.Label(new Rect(5f, 45f, 80f, 20f), $"Max: {num2:0.0}");
			GUI.EndGroup();
		}
	}
}
