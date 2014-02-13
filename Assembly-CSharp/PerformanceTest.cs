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
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Expected Obj, but got Unknown
			if ((Object)(object)_instance == (Object)null)
			{
				_instance = Object.FindObjectOfType(typeof(PerformanceTest)) as PerformanceTest;
			}
			if ((Object)(object)_instance == (Object)null)
			{
				GameObject val = new GameObject("PerformanceTest");
				_instance = val.AddComponent(typeof(PerformanceTest)) as PerformanceTest;
			}
			return _instance;
		}
	}

	public void Init()
	{
		Debug.Log((object)"Performancetest Init");
	}

	private void Start()
	{
		serverStartTime = MVGameController.Instance.Game.Peer.ServerTimeInMilliSeconds;
		Debug.Log((object)"Performance Test Starting");
	}

	private void FixedUpdate()
	{
		int num = MVGameController.Instance.Game.Peer.ServerTimeInMilliSeconds - serverStartTime;
		clientTime += Time.fixedDeltaTime;
		debuglogInterval++;
		if (debuglogInterval > 59)
		{
			debuglogInterval = 0;
			Debug.Log((object)("Servertime : " + num + " clientTime: " + clientTime));
		}
	}
}
