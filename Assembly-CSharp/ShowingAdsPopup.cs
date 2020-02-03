using System;
using UnityEngine;
using UnityEngine.UI;

public class ShowingAdsPopup : MonoBehaviour
{
	[SerializeField]
	private Button button;

	private float startTime;

	private float timeoutTime;

	private Action skipAction;

	private bool previousMuteState;

	public void Initialize(float timeoutTime, Action OnSkipPressed)
	{
		skipAction = OnSkipPressed;
		startTime = Time.time;
		this.timeoutTime = timeoutTime;
	}

	private void OnEnable()
	{
		previousMuteState = MainCameraManager.Mute;
		Debug.Log("Muting audio");
		MainCameraManager.Mute = true;
	}

	private void OnDestroy()
	{
		Debug.Log("Resuming audio");
		MainCameraManager.Mute = previousMuteState;
	}

	private void Update()
	{
		if (Time.time - startTime >= timeoutTime)
		{
			button.gameObject.SetActive(value: true);
			enabled = false;
		}
	}

	public void OnSkip()
	{
		if (skipAction != null)
		{
			skipAction();
		}
	}
}
