using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimeAttackFlagCountDownNotification : Notification
{
	[SerializeField]
	private Text countdownText;

	[SerializeField]
	private NotificationFade countDownFader;

	private float countDownStartTime;

	private const float countDownDuration = 3f;

	protected override NotificationLifetime Lifetime => NotificationLifetime.High;

	public override void Initialize(Dictionary<object, object> data)
	{
		base.Initialize(data);
		countDownFader.Activate();
		NotificationFade notificationFade = countDownFader;
		notificationFade.OnFinished = (Action)Delegate.Combine(notificationFade.OnFinished, new Action(DestroyNotification));
		countDownStartTime = Time.time;
	}

	protected override void Update()
	{
		float num = countDownStartTime + 3f - Time.time;
		countdownText.text = (Mathf.FloorToInt(num) + 1).ToString();
		if (num < 0f)
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
