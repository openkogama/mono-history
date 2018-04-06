using System;
using System.Collections.Generic;
using MV.Common;
using UnityEngine;
using UnityEngine.UI;

public class RoundTimer : MonoBehaviour
{
	[SerializeField]
	private Text timeLeftText;

	[SerializeField]
	private ProgressBar progressBar;

	private WorldObjectClientRef<MVRoundCube> roundCube;

	private List<int> timeNotifications;

	public Action<NotificationType, Dictionary<object, object>> OnTimeNotificationSend;

	public void Initialize(WorldObjectClientRef<MVRoundCube> roundCube)
	{
		this.roundCube = roundCube;
		timeNotifications = new List<int>();
		ResetTimeNotifications();
	}

	public void ResetOnRoundEnd()
	{
		ResetTimeNotifications();
	}

	private void Update()
	{
		if (roundCube.WorldObjectClient != null)
		{
			int timeLeft = roundCube.WorldObjectClient.GetTimeLeft();
			timeLeftText.text = roundCube.WorldObjectClient.MakeTimeIntoText(timeLeft);
			HandleTimeNotifications(timeLeft);
			progressBar.Progress = (float)timeLeft / (float)roundCube.WorldObjectClient.DurationInMilliseconds;
		}
	}

	private void HandleTimeNotifications(int timeLeft)
	{
		int item = (int)((float)timeLeft / 1000f);
		if (timeNotifications.Contains(item))
		{
			timeNotifications.Remove(item);
			if (OnTimeNotificationSend != null)
			{
				Dictionary<object, object> dictionary = new Dictionary<object, object>();
				dictionary.Add((byte)17, MVGameControllerBase.Game.ServerTimeInMilliSeconds);
				dictionary.Add((byte)4, timeLeft);
				OnTimeNotificationSend(NotificationType.HurryUp, dictionary);
			}
		}
	}

	private void ResetTimeNotifications()
	{
		if (timeNotifications != null)
		{
			if (!timeNotifications.Contains(10))
			{
				timeNotifications.Add(10);
			}
			if (!timeNotifications.Contains(30))
			{
				timeNotifications.Add(30);
			}
			if (!timeNotifications.Contains(60))
			{
				timeNotifications.Add(60);
			}
			if (!timeNotifications.Contains(300))
			{
				timeNotifications.Add(300);
			}
		}
	}

	private void OnDestroy()
	{
		roundCube = null;
	}
}
