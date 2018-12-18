using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WaitCountdownNotification : Notification
{
	[SerializeField]
	private Text countdownText;

	[SerializeField]
	private NotificationFade countDownFader;

	protected override NotificationLifetime Lifetime => NotificationLifetime.High;

	public override void Initialize(Dictionary<object, object> data)
	{
		base.Initialize(data);
		countDownFader.Activate();
		NotificationFade notificationFade = countDownFader;
		notificationFade.OnFinished = (Action)Delegate.Combine(notificationFade.OnFinished, new Action(DestroyNotification));
		countDownFader.PauseAt(2f);
	}

	protected override void Update()
	{
		float num = MVGameControllerBase.Game.NetworkGameStateListener.TimeLeftMS;
		countdownText.text = (Mathf.FloorToInt(num / 1000f) + 1).ToString();
		if (num <= 0f)
		{
			countdownText.text = "GO!";
			countDownFader.Unpause();
		}
		base.Update();
		timeSinceStart = 0f;
	}

	private void DestroyNotification()
	{
		timeSinceStart = (float)Lifetime;
	}
}
