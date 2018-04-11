using System;
using System.Collections.Generic;
using MV.Common;
using UnityEngine;
using UnityEngine.UI;

public class LoadingScreenHandler : MonoBehaviour
{
	[SerializeField]
	private ProgressBarAndroid loadingBar;

	[SerializeField]
	private Text uiText;

	private int eventsCount = 17;

	private int currentEventCount = 1;

	private void Awake()
	{
		loadingBar.Progress = 0f;
		uiText.text = string.Empty;
		MVGameControllerBase.OnReceivedGameMsg = (MVGameControllerBase.OnReceivedGameMsgDelegate)Delegate.Combine(MVGameControllerBase.OnReceivedGameMsg, new MVGameControllerBase.OnReceivedGameMsgDelegate(OnGameMessageReceived));
	}

	private void OnGameMessageReceived(MVGameMsgType gameMsgType, Dictionary<object, object> gameMsgData)
	{
		if (gameMsgType == MVGameMsgType.JoinFlowStatus)
		{
			string key = (string)gameMsgData[(byte)5];
			uiText.text = TM._(key);
			currentEventCount++;
			loadingBar.Progress = (float)currentEventCount / (float)eventsCount;
		}
	}

	private void OnDestroy()
	{
		MVGameControllerBase.OnReceivedGameMsg = (MVGameControllerBase.OnReceivedGameMsgDelegate)Delegate.Remove(MVGameControllerBase.OnReceivedGameMsg, new MVGameControllerBase.OnReceivedGameMsgDelegate(OnGameMessageReceived));
	}
}
