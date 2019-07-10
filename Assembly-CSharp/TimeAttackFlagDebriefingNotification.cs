using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimeAttackFlagDebriefingNotification : Notification
{
	[SerializeField]
	private Text timeText;

	[SerializeField]
	private Text descriptionText;

	[SerializeField]
	private NotificationFade debriefingFader;

	[SerializeField]
	private Text countdownText;

	[SerializeField]
	private NotificationFade countDownFader;

	private float countDownStartTime;

	private const float countDownDuration = 3f;

	protected override NotificationLifetime Lifetime => NotificationLifetime.High;

	private void Start()
	{
		FlagDebriefingControl flagDebriefingControl = MVGameControllerBase.FlagDebriefingControl;
		flagDebriefingControl.OnFlagDebriefingEnd = (Action)Delegate.Combine(flagDebriefingControl.OnFlagDebriefingEnd, new Action(OnTimeFlagDebriefingEnd));
	}

	private void OnDestroy()
	{
		if (MVGameControllerBase.IsAlive)
		{
			FlagDebriefingControl flagDebriefingControl = MVGameControllerBase.FlagDebriefingControl;
			flagDebriefingControl.OnFlagDebriefingEnd = (Action)Delegate.Remove(flagDebriefingControl.OnFlagDebriefingEnd, new Action(OnTimeFlagDebriefingEnd));
		}
	}

	public override void Initialize(Dictionary<object, object> data)
	{
		base.Initialize(data);
		int score = (int)data[(byte)4];
		timeText.text = WinningConditionControl.MakeIntoScoreText(score, GameStatCounterType.TimeAttackFlag);
		descriptionText.text = (string)data[(byte)1];
		debriefingFader.Activate();
		debriefingFader.PauseAt(3f);
		NotificationFade notificationFade = debriefingFader;
		notificationFade.OnFinished = (Action)Delegate.Combine(notificationFade.OnFinished, new Action(StartCountDown));
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

	private void OnTimeFlagDebriefingEnd()
	{
		debriefingFader.Unpause();
	}

	private void StartCountDown()
	{
		countDownFader.Activate();
		NotificationFade notificationFade = countDownFader;
		notificationFade.OnFinished = (Action)Delegate.Combine(notificationFade.OnFinished, new Action(DestroyNotification));
		countDownStartTime = Time.time;
	}

	private void DestroyNotification()
	{
		timeSinceStart = (float)Lifetime;
	}
}
