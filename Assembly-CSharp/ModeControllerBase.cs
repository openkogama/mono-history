using UnityEngine;

public abstract class ModeControllerBase : MonoBehaviour
{
	[SerializeField]
	private GameObject fpsCounterPrefab;

	private GameObject fpsCounter;

	public virtual void Initialize()
	{
		MVGameControllerBase.CameraController.Init();
		if (!MVGameControllerBase.IsTouristSession)
		{
			MVGameControllerBase.TimeReward.Init();
		}
	}

	protected void HandleFpsShortcut()
	{
		if (Input.GetKey(KeyCode.Alpha8) && Input.GetKeyUp(KeyCode.Alpha9))
		{
			if (fpsCounter != null)
			{
				Object.Destroy(fpsCounter);
				return;
			}
			fpsCounter = Object.Instantiate(fpsCounterPrefab);
			fpsCounter.transform.SetParent(transform, worldPositionStays: false);
		}
	}
}
