using System;
using UnityEngine;

namespace CodeStage.AntiCheat.Detectors;

[AddComponentMenu("")]
public abstract class ActDetectorBase : MonoBehaviour
{
	protected const string CONTAINER_NAME = "Anti-Cheat Toolkit Detectors";

	protected const string MENU_PATH = "GameObject/Create Other/Code Stage/Anti-Cheat Toolkit/";

	[Tooltip("Automatically dispose Detector after firing callback.")]
	public bool autoDispose = true;

	[Tooltip("Detector will survive new level (scene) load if checked.")]
	public bool keepAlive = true;

	protected static GameObject detectorsContainer;

	protected Action onDetection;

	private bool inited;

	private void Start()
	{
		inited = true;
	}

	protected virtual bool Init(ActDetectorBase instance, string detectorName)
	{
		if (instance != null && instance != this && instance.keepAlive)
		{
			UnityEngine.Object.Destroy(this);
			return false;
		}
		UnityEngine.Object.DontDestroyOnLoad(gameObject);
		return true;
	}

	private void OnDisable()
	{
		if (inited)
		{
			StopDetectionInternal();
		}
	}

	private void OnApplicationQuit()
	{
		DisposeInternal();
	}

	private void OnLevelWasLoaded(int index)
	{
		if (inited && !keepAlive)
		{
			DisposeInternal();
		}
	}

	protected abstract void StopDetectionInternal();

	protected virtual void DisposeInternal()
	{
		StopDetectionInternal();
		UnityEngine.Object.Destroy(this);
	}

	protected virtual void OnDestroy()
	{
		if (transform.childCount == 0 && GetComponentsInChildren<Component>().Length <= 2)
		{
			UnityEngine.Object.Destroy(gameObject);
		}
	}
}
