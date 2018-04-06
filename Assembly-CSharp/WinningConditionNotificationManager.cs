using System.Collections.Generic;
using MV.Common;
using MV.WorldObject;

public class WinningConditionNotificationManager
{
	private Dictionary<MVTeam, int> flagHighScores;

	private int flagHighScore = int.MaxValue;

	public void Initialize()
	{
		flagHighScores = new Dictionary<MVTeam, int>();
		for (int i = 0; i < MVGameControllerBase.Game.TeamManager.GetTeamList().Count; i++)
		{
			int num = MVGameControllerBase.Game.GameStatCounterManager.GetTeamCount(GameStatCounterType.Flag, MVGameControllerBase.Game.TeamManager.GetTeamList()[i]);
			if (num <= 0)
			{
				num = int.MaxValue;
			}
			flagHighScores.Add(MVGameControllerBase.Game.TeamManager.GetTeamList()[i], num);
		}
	}

	public void Reset()
	{
		flagHighScores.Clear();
		for (int i = 0; i < MVGameControllerBase.Game.TeamManager.GetTeamList().Count; i++)
		{
			flagHighScores.Add(MVGameControllerBase.Game.TeamManager.GetTeamList()[i], int.MaxValue);
		}
	}

	public void UpdateNotification(int actorNumber, GameStatCounterType counterType, int scoreCount)
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
			SendNotificaion(notificationType, dictionary);
		}
	}

	public void SendNotificaion(NotificationType type, Dictionary<object, object> data)
	{
		NotificationController.PushNotification(type, data);
	}

	private int GetScoreLeftToWin(GameStatCounterType counterType, int scoreCount)
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
			result = scoreCount;
			break;
		}
		return result;
	}

	private bool ShouldShowNotification(int actorNumber, GameStatCounterType counterType, int scoreLeftToWin, out NotificationType notificationType)
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
		{
			MVPlayer mVPlayer = MVGameControllerBase.Game.MVPlayerContainer[actorNumber];
			if (mVPlayer == null)
			{
				return false;
			}
			if (!flagHighScores.ContainsKey(mVPlayer.Team))
			{
				flagHighScores.Add(mVPlayer.Team, int.MaxValue);
			}
			if (scoreLeftToWin < flagHighScores[mVPlayer.Team])
			{
				notificationType = NotificationType.FlagHighScore;
				return true;
			}
			break;
		}
		}
		return false;
	}

	private string GenerateNotificationText(int actorNumber, GameStatCounterType counterType, int scoreLeftToWin)
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
			result = WinningConditionControl.MakeIntoScoreText(scoreLeftToWin, GameStatCounterType.Flag);
			if (WinningConditionControl.IsNewScoreBetter(scoreLeftToWin, flagHighScore, GameStatCounterType.Flag))
			{
				flagHighScore = scoreLeftToWin;
			}
			break;
		}
		return result;
	}

	private string GetContestantName(int actorNumber)
	{
		MVPlayer playerUnsafe = MVGameControllerBase.Game.MVPlayerContainer.GetPlayerUnsafe(actorNumber);
		if (MVGameControllerBase.Game.TeamManager.GetTeamList().Count <= 1)
		{
			return playerUnsafe.Username;
		}
		return GetTeamName(playerUnsafe.Team);
	}

	private string GetPlayerName(int actorNumber)
	{
		string result = string.Empty;
		MVPlayer playerUnsafe = MVGameControllerBase.Game.MVPlayerContainer.GetPlayerUnsafe(actorNumber);
		if (playerUnsafe != null)
		{
			result = playerUnsafe.Username;
		}
		return result;
	}

	private string GetTeamName(MVTeam team)
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
}
