using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HurryUpNotification : Notification
{
	[SerializeField]
	private Text timeText;

	[SerializeField]
	private NotificationFade fader;

	[SerializeField]
	private NotificationFade countdownFader;

	[SerializeField]
	private AudioSource countDownSound;

	private int timeStamp;

	private int timeLeftFromTimeStamp;

	protected override NotificationLifetime Lifetime => NotificationLifetime.High;

	public override void Initialize(Dictionary<object, object> data)
	{
		base.Initialize(data);
		timeStamp = (int)data[(byte)17];
		timeLeftFromTimeStamp = (int)data[(byte)4];
		int serverTimeInMilliSeconds = MVGameControllerBase.Game.ServerTimeInMilliSeconds;
		int num = timeLeftFromTimeStamp - (serverTimeInMilliSeconds - timeStamp);
		int num2 = (int)((float)num / 1000f);
		if (num2 > 10)
		{
			fader.Activate();
		}
		else
		{
			countdownFader.Activate();
			timeSinceStart = (float)(Lifetime - num2) - 2f;
		}
		NotificationFade notificationFade = fader;
		notificationFade.OnFinished = (Action)Delegate.Combine(notificationFade.OnFinished, new Action(DestroyNotification));
		MVNetworkGame game = MVGameControllerBase.Game;
		game.OnWinningConditionFulfilled = (Action<IWinningCondition>)Delegate.Combine(game.OnWinningConditionFulfilled, new Action<IWinningCondition>(OnRoundEnd));
	}

	protected override void Update()
	{
		base.Update();
		int serverTimeInMilliSeconds = MVGameControllerBase.Game.ServerTimeInMilliSeconds;
		int num = timeLeftFromTimeStamp - (serverTimeInMilliSeconds - timeStamp) + 1000;
		string text = WinningConditionControl.MakeIntoScoreText(num, GameStatCounterType.Flag);
		if (timeText.text != text)
		{
			timeText.text = text;
			if (num <= 10000)
			{
				countDownSound.Play();
			}
		}
		if (num < 0)
		{
			fader.Deactivate();
			countdownFader.Deactivate();
		}
	}

	private void DestroyNotification()
	{
		timeSinceStart = (float)Lifetime;
	}

	private void OnRoundEnd(IWinningCondition winningCondition)
	{
		DestroyNotification();
	}

	private void OnDestroy()
	{
		if (MVGameControllerBase.Game != null)
		{
			MVNetworkGame game = MVGameControllerBase.Game;
			game.OnWinningConditionFulfilled = (Action<IWinningCondition>)Delegate.Remove(game.OnWinningConditionFulfilled, new Action<IWinningCondition>(OnRoundEnd));
		}
	}
}
