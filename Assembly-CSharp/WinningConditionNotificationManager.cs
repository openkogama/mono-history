using System.Collections.Generic;
using MV.Common;
using MV.WorldObject;

public static class WinningConditionNotificationManager
{
	public static void UpdateNotification(int actorNumber, GameStatCounterType counterType, int scoreCount)
	{
		WinningConditionControl.TryGetPrioritizedStat(out var statType);
		if (counterType == statType)
		{
			int scoreLeftToWin = GetScoreLeftToWin(counterType, scoreCount);
			NotificationType notificationType = NotificationType.None;
			if (ShouldShowNotification(actorNumber, counterType, scoreLeftToWin, out notificationType))
			{
				string key = GenerateNotificationText(actorNumber, counterType, scoreLeftToWin);
				Dictionary<object, object> dictionary = new Dictionary<object, object>();
				dictionary.Add((byte)1, TM._(key));
				dictionary.Add((byte)9, actorNumber);
				dictionary.Add((byte)5, counterType);
				dictionary.Add((byte)4, scoreLeftToWin);
				SendNotificaion(notificationType, dictionary);
			}
		}
	}

	public static void SendNotificaion(NotificationType type, Dictionary<object, object> data)
	{
		NotificationController.PushNotification(type, data);
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

	private static bool ShouldShowNotification(int actorNumber, GameStatCounterType counterType, int scoreLeftToWin, out NotificationType notificationType)
	{
		notificationType = NotificationType.None;
		if (!MVGameControllerBase.Game.MVPlayerContainer.ContainsKey(actorNumber))
		{
			return false;
		}
		switch (counterType)
		{
		case GameStatCounterType.Kill:
		case GameStatCounterType.Collectible:
		case GameStatCounterType.OculusKill:
			if (scoreLeftToWin == 1 || scoreLeftToWin == 5 || scoreLeftToWin == 15)
			{
				notificationType = NotificationType.WinningWarning;
				return true;
			}
			break;
		case GameStatCounterType.Flag:
		case GameStatCounterType.TimeAttackFlag:
		{
			if (scoreLeftToWin == 0)
			{
				return false;
			}
			MVPlayer mVPlayer = MVGameControllerBase.Game.MVPlayerContainer[actorNumber];
			if (mVPlayer == null)
			{
				return false;
			}
			if (IsFlagScoreBestInGame(scoreLeftToWin, actorNumber))
			{
				notificationType = NotificationType.FlagHighScore;
				return true;
			}
			break;
		}
		}
		return false;
	}

	private static string GenerateNotificationText(int actorNumber, GameStatCounterType counterType, int scoreLeftToWin)
	{
		string result = string.Empty;
		switch (counterType)
		{
		case GameStatCounterType.Kill:
		case GameStatCounterType.Collectible:
		case GameStatCounterType.OculusKill:
			result = scoreLeftToWin.ToString();
			break;
		case GameStatCounterType.Flag:
		case GameStatCounterType.TimeAttackFlag:
			result = WinningConditionControl.MakeIntoScoreText(scoreLeftToWin, GameStatCounterType.TimeAttackFlag);
			break;
		}
		return result;
	}

	private static string GetContestantName(int actorNumber)
	{
		string empty = string.Empty;
		if (!MVGameControllerBase.Game.MVPlayerContainer.TryGetValue(actorNumber, out var player))
		{
			return empty;
		}
		if (MVGameControllerBase.Game.TeamManager.GetTeamList().Count <= 1)
		{
			return player.Username;
		}
		return GetTeamName(player.Team);
	}

	private static string GetPlayerName(int actorNumber)
	{
		string empty = string.Empty;
		if (!MVGameControllerBase.Game.MVPlayerContainer.TryGetValue(actorNumber, out var player))
		{
			return empty;
		}
		return player.Username;
	}

	private static string GetTeamName(MVTeam team)
	{
		string result = string.Empty;
		switch (team)
		{
		case MVTeam.Blue:
			result = "Blue team";
			break;
		case MVTeam.Red:
			result = "Red team";
			break;
		case MVTeam.Green:
			result = "Green team";
			break;
		case MVTeam.Yellow:
			result = "Yellow team";
			break;
		case MVTeam.None:
			result = "None team";
			break;
		}
		return result;
	}

	private static bool IsFlagScoreBestInGame(int score, int actorNumber)
	{
		int playerRanking = GetPlayerRanking(GameStatCounterType.TimeAttackFlag, actorNumber, score);
		if (playerRanking == 1)
		{
			return true;
		}
		return false;
	}

	private static int GetPlayerRanking(GameStatCounterType statType, int actorNumber, int score)
	{
		int num = 1;
		foreach (KeyValuePair<int, MVPlayer> item in MVGameControllerBase.Game.MVPlayerContainer)
		{
			if (item.Value == null)
			{
				continue;
			}
			int actorNr = item.Value.ActorNr;
			if (actorNr != actorNumber)
			{
				int actorCount = MVGameControllerBase.Game.GameStatCounterManager.GetActorCount(statType, item.Value.Team, actorNr);
				if (WinningConditionControl.IsNewScoreBetter(actorCount, score, statType))
				{
					num++;
				}
			}
		}
		return num;
	}
}
