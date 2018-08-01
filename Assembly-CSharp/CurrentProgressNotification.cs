using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CurrentProgressNotification : Notification
{
	[Serializable]
	private class WinninConditionImage
	{
		[SerializeField]
		public GameStatCounterType Key;

		[SerializeField]
		public Image Value;
	}

	[SerializeField]
	private Text scoreText;

	[SerializeField]
	private Text warningText;

	[SerializeField]
	private Text currentProgressText;

	[SerializeField]
	private NotificationFade fader;

	[SerializeField]
	private List<WinninConditionImage> winningConditionImages;

	private int avatarStartTime;

	private bool shouldShowCurrentTime;

	protected override NotificationLifetime Lifetime => NotificationLifetime.High;

	public override void Initialize(Dictionary<object, object> data)
	{
		avatarStartTime = MVGameControllerBase.Game.LocalPlayer.JoinTime;
		int actorNr = MVGameControllerBase.Game.LocalPlayer.ActorNr;
		WinningConditionControl.TryGetPrioritizedStat(out var statType);
		int num = GetScoreLeftToWin(scoreCount: (MVGameControllerBase.Game.TeamManager.GetTeamList().Count <= 1) ? MVGameControllerBase.Game.GameStatCounterManager.GetActorCount(statType, MVGameControllerBase.Game.LocalPlayer.Team, actorNr) : MVGameControllerBase.Game.GameStatCounterManager.GetTeamCount(statType, MVGameControllerBase.Game.LocalPlayer.Team), counterType: statType);
		if (MVGameControllerBase.Game.MVPlayerContainer.TryGetValue(actorNr, out var player))
		{
			base.Initialize(data);
			if (statType == GameStatCounterType.Flag || statType == GameStatCounterType.TimeAttackFlag)
			{
				currentProgressText.text = string.Empty;
				scoreText.text = WinningConditionControl.MakeIntoScoreText(CalculateCurrentTime(GetStartTime()), GameStatCounterType.Flag);
				shouldShowCurrentTime = true;
			}
			else
			{
				currentProgressText.text = "YOU HAVE";
				scoreText.text = WinningConditionControl.MakeIntoScoreText(num, statType);
				shouldShowCurrentTime = false;
			}
			fader.Activate();
			SelectWinningConditionImage(statType, player);
			SetWarningText(statType, num);
			NotificationFade notificationFade = fader;
			notificationFade.OnFinished = (Action)Delegate.Combine(notificationFade.OnFinished, new Action(DestroyNotification));
		}
	}

	private void SelectWinningConditionImage(GameStatCounterType winningConditionType, MVPlayer player)
	{
		for (int i = 0; i < winningConditionImages.Count; i++)
		{
			if (winningConditionImages[i].Key == winningConditionType)
			{
				winningConditionImages[i].Value.gameObject.SetActive(value: true);
			}
			else
			{
				winningConditionImages[i].Value.gameObject.SetActive(value: false);
			}
		}
	}

	private void SetWarningText(GameStatCounterType winningConditionType, int scoreLeft)
	{
		string text = string.Empty;
		switch (winningConditionType)
		{
		case GameStatCounterType.Collectible:
			text = "STAR";
			break;
		case GameStatCounterType.Kill:
		case GameStatCounterType.OculusKill:
			text = "KILL";
			break;
		}
		if (scoreLeft > 1)
		{
			text += "S";
		}
		text += " LEFT!";
		if (winningConditionType == GameStatCounterType.Flag || winningConditionType == GameStatCounterType.TimeAttackFlag)
		{
			text = "CURRENT TIME";
		}
		warningText.text = text;
	}

	private void DestroyNotification()
	{
		timeSinceStart = (float)Lifetime;
	}

	protected override void Update()
	{
		base.Update();
		if (shouldShowCurrentTime)
		{
			scoreText.text = WinningConditionControl.MakeIntoScoreText(CalculateCurrentTime(GetStartTime()), GameStatCounterType.Flag);
		}
	}

	private int GetStartTime()
	{
		int num = 0;
		if (MVGameControllerBase.Game.NetworkGameStateListener.StartTime < avatarStartTime)
		{
			return avatarStartTime;
		}
		return MVGameControllerBase.Game.NetworkGameStateListener.StartTime;
	}

	private int CalculateCurrentTime(int startTime)
	{
		return MVGameControllerBase.Game.ServerTimeInMilliSeconds - startTime;
	}

	private static int GetScoreLeftToWin(GameStatCounterType counterType, int scoreCount)
	{
		int result = 0;
		int num = 0;
		switch (counterType)
		{
		case GameStatCounterType.Collectible:
		{
			AllCollectiblesCollectedClient singletonWinnerConditionByType2 = MVGameControllerBase.Game.WinningConditionManager.GetSingletonWinnerConditionByType<AllCollectiblesCollectedClient>();
			num = 0;
			if (singletonWinnerConditionByType2 != null)
			{
				num = singletonWinnerConditionByType2.Limit;
			}
			result = num - scoreCount;
			break;
		}
		case GameStatCounterType.Kill:
		{
			KillLimitClient singletonWinnerConditionByType3 = MVGameControllerBase.Game.WinningConditionManager.GetSingletonWinnerConditionByType<KillLimitClient>();
			num = 0;
			if (singletonWinnerConditionByType3 != null)
			{
				num = singletonWinnerConditionByType3.Limit;
			}
			result = num - scoreCount;
			break;
		}
		case GameStatCounterType.OculusKill:
		{
			OculusKillLimitClient singletonWinnerConditionByType = MVGameControllerBase.Game.WinningConditionManager.GetSingletonWinnerConditionByType<OculusKillLimitClient>();
			num = 0;
			if (singletonWinnerConditionByType != null)
			{
				num = singletonWinnerConditionByType.Limit;
			}
			result = num - scoreCount;
			break;
		}
		case GameStatCounterType.Flag:
		case GameStatCounterType.TimeAttackFlag:
			result = scoreCount;
			break;
		}
		return result;
	}
}
