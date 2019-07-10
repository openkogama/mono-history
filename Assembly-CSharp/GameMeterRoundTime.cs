using System;
using System.Collections.Generic;
using MV.Common;
using UnityEngine;
using UnityEngine.UI;

public class GameMeterRoundTime : GameMeterBase
{
	[SerializeField]
	private Image roundTimeBar;

	[SerializeField]
	private Text roundTime;

	private MVRoundCube roundCube;

	private List<int> timeNotifications;

	public override GameMeterType GameMeterType => GameMeterType.Time;

	public override void Initialize()
	{
		timeNotifications = new List<int>();
		ResetTimeNotifications();
		MVNetworkGame game = MVGameControllerBase.Game;
		game.OnWinningConditionFulfilled = (Action<IWinningCondition>)Delegate.Combine(game.OnWinningConditionFulfilled, new Action<IWinningCondition>(ResetOnRoundEnd));
	}

	private void OnDestroy()
	{
		if (MVGameControllerBase.IsAlive && MVGameControllerBase.Game != null)
		{
			MVNetworkGame game = MVGameControllerBase.Game;
			game.OnWinningConditionFulfilled = (Action<IWinningCondition>)Delegate.Remove(game.OnWinningConditionFulfilled, new Action<IWinningCondition>(ResetOnRoundEnd));
		}
	}

	public override void SetGameMeterVisibility()
	{
		roundCube = MVGameControllerBase.WOCM.GetSingletonWorldObject<MVRoundCube>();
		if (roundCube != null)
		{
			if (!gameObject.activeSelf)
			{
				Show();
			}
			int timeLeft = GetTimeLeft(roundCube);
			if (timeLeft > 0)
			{
				int num = (int)((float)timeLeft / 1000f) + 1;
				roundTime.text = $"{num / 60:00}:{num % 60:00}";
			}
		}
		else
		{
			Hide();
			roundTime.text = string.Empty;
		}
	}

	public override void UpdateValue()
	{
	}

	private void Update()
	{
		if (roundCube == null)
		{
			Hide();
			return;
		}
		int num = GetTimeLeft(roundCube);
		if (MVGameControllerBase.Game.NetworkGameStateListener.CurrentGameState == MVGameStateType.RoundEnded)
		{
			num = 0;
			roundTime.text = $"{0:00}:{0:00}";
		}
		if (num > 0)
		{
			int num2 = (int)((float)num / 1000f) + 1;
			roundTime.text = $"{num2 / 60:00}:{num2 % 60:00}";
		}
		HandleTimeNotifications(num);
	}

	public override void SetShowGameMeter(bool show)
	{
		roundTimeBar.enabled = show;
		roundTime.enabled = show;
	}

	private void Hide()
	{
		gameObject.SetActive(value: false);
	}

	private void Show()
	{
		gameObject.SetActive(value: true);
	}

	private int GetTimeLeft(MVRoundCube roundCube)
	{
		int num = roundCube.DurationInMilliseconds - (MVGameControllerBase.Game.ServerTimeInMilliSeconds - MVGameControllerBase.Game.NetworkGameStateListener.StartTime);
		if (num < 0)
		{
			num = 0;
		}
		return num;
	}

	private void HandleTimeNotifications(int timeLeft)
	{
		int item = (int)((float)timeLeft / 1000f) + 1;
		if (timeNotifications.Contains(item))
		{
			timeNotifications.Remove(item);
			Dictionary<object, object> dictionary = new Dictionary<object, object>();
			dictionary.Add((byte)17, MVGameControllerBase.Game.ServerTimeInMilliSeconds);
			dictionary.Add((byte)4, timeLeft);
			WinningConditionNotificationManager.SendNotification(NotificationType.HurryUp, dictionary);
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

	private void ResetOnRoundEnd(IWinningCondition winningCondition)
	{
		ResetTimeNotifications();
	}
}
