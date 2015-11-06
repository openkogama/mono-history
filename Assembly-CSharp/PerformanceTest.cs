using UnityEngine;

public class PerformanceTest : MonoBehaviour
{
	private int serverStartTime;

	private int debuglogInterval;

	private float clientTime;

	private static PerformanceTest _instance;

	public static PerformanceTest Instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = Object.FindObjectOfType(typeof(PerformanceTest)) as PerformanceTest;
			}
			if (_instance == null)
			{
				GameObject gameObject = new GameObject("PerformanceTest");
				_instance = gameObject.AddComponent(typeof(PerformanceTest)) as PerformanceTest;
			}
			return _instance;
		}
	}

	public void Init()
	{
		Debug.Log("Performancetest Init");
	}

	private void Start()
	{
		serverStartTime = MVGameControllerBase.Game.Peer.ServerTimeInMilliSeconds;
		Debug.Log("Performance Test Starting");
	}

	private void FixedUpdate()
	{
		int num = MVGameControllerBase.Game.Peer.ServerTimeInMilliSeconds - serverStartTime;
		clientTime += Time.fixedDeltaTime;
		debuglogInterval++;
		if (debuglogInterval > 59)
		{
			debuglogInterval = 0;
			Debug.Log("Servertime : " + num + " clientTime: " + clientTime);
		}
	}
}
