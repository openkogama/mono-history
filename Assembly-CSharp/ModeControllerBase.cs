using UnityEngine;
using UnityEngine.EventSystems;

public abstract class ModeControllerBase : MonoBehaviour, IToggleFps, IEventSystemHandler
{
	[SerializeField]
	private GameObject fpsCounterPrefab;

	private GameObject fpsCounter;

	public virtual void Initialize()
	{
		MVGameControllerBase.CameraController.Init();
	}

	public void ToggleFps()
	{
		if (fpsCounter != null)
		{
			Object.Destroy(fpsCounter);
			return;
		}
		fpsCounter = Object.Instantiate(fpsCounterPrefab);
		fpsCounter.transform.SetParent(transform, worldPositionStays: false);
	}

	protected void HandleFpsShortcut()
	{
		if (Input.GetKey(KeyCode.Alpha8) && Input.GetKeyUp(KeyCode.Alpha9))
		{
			ToggleFps();
		}
		if (Input.GetKey(KeyCode.Alpha8) && Input.GetKeyUp(KeyCode.Alpha0))
		{
			FirstTimeEventManager.ResetFirstTimeEvents(overrideValue: false);
		}
	}
}
