using System;
using System.Collections.Generic;
using MV.Common;
using UnityEngine;
using UnityEngine.UI;

public class LoadingScreenHandler : MonoBehaviour
{
	private const float lerpSpeed = 1f;

	[SerializeField]
	private ProgressBarAndroid loadingBar;

	[SerializeField]
	private Text uiText;

	private Dictionary<MVGameMode, int> eventCountLookup = new Dictionary<MVGameMode, int>
	{
		{
			MVGameMode.Play,
			7
		},
		{
			MVGameMode.Edit,
			16
		},
		{
			MVGameMode.CharacterEditor,
			14
		}
	};

	private int currentEventCount = 1;

	private int eventsCount = 17;

	private float currentProgress;

	private float targetProgress;

	private float currentTime;

	private void Start()
	{
		uiText.text = string.Empty;
		loadingBar.Progress = 0f;
		MVGameControllerBase.OnReceivedGameMsg = (MVGameControllerBase.OnReceivedGameMsgDelegate)Delegate.Combine(MVGameControllerBase.OnReceivedGameMsg, new MVGameControllerBase.OnReceivedGameMsgDelegate(OnGameMessageReceived));
		eventsCount = eventCountLookup[MVGameControllerBase.GameMode];
	}

	private void Update()
	{
		currentTime += Time.deltaTime;
		loadingBar.Progress = Mathf.Lerp(currentProgress, targetProgress, currentTime);
	}

	private void OnGameMessageReceived(MVGameMsgType gameMsgType, Dictionary<object, object> gameMsgData)
	{
		if (gameMsgType == MVGameMsgType.JoinFlowStatus)
		{
			string key = (string)gameMsgData[(byte)5];
			uiText.text = TM._(key);
			currentEventCount++;
			currentProgress = loadingBar.Progress;
			currentTime = 0f;
			targetProgress = (float)currentEventCount / (float)eventsCount;
		}
	}

	private void OnDestroy()
	{
		MVGameControllerBase.OnReceivedGameMsg = (MVGameControllerBase.OnReceivedGameMsgDelegate)Delegate.Remove(MVGameControllerBase.OnReceivedGameMsg, new MVGameControllerBase.OnReceivedGameMsgDelegate(OnGameMessageReceived));
	}
}
