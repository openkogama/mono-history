using UnityEngine;

public class WinningConditionControl : MonoBehaviour
{
	public static bool TryGetPrioritizedWinCondition(out WinningConditionType condition)
	{
		condition = WinningConditionType.None;
		if (MVGameControllerBase.Game.WinningConditionManager.GetSingletonWinnerConditionByType<FlagReachedClient>() != null)
		{
			condition = WinningConditionType.Flag;
			return true;
		}
		if (MVGameControllerBase.Game.WinningConditionManager.GetSingletonWinnerConditionByType<AllCollectiblesCollectedClient>() != null)
		{
			condition = WinningConditionType.Collectible;
			return true;
		}
		if (MVGameControllerBase.Game.WinningConditionManager.GetSingletonWinnerConditionByType<KillLimitClient>() != null)
		{
			condition = WinningConditionType.Kill;
			return true;
		}
		if (MVGameControllerBase.Game.WinningConditionManager.GetSingletonWinnerConditionByType<OculusKillLimitClient>() != null)
		{
			condition = WinningConditionType.Oculus;
			return true;
		}
		return false;
	}

	public static bool TryGetPrioritizedStat(out GameStatCounterType statType)
	{
		statType = GameStatCounterType.None;
		if (MVGameControllerBase.Game.WinningConditionManager.GetSingletonWinnerConditionByType<FlagReachedClient>() != null)
		{
			statType = GameStatCounterType.Flag;
			return true;
		}
		if (MVGameControllerBase.Game.WinningConditionManager.GetSingletonWinnerConditionByType<AllCollectiblesCollectedClient>() != null)
		{
			statType = GameStatCounterType.Collectible;
			return true;
		}
		if (MVGameControllerBase.Game.WinningConditionManager.GetSingletonWinnerConditionByType<KillLimitClient>() != null)
		{
			statType = GameStatCounterType.Kill;
			return true;
		}
		if (MVGameControllerBase.Game.WinningConditionManager.GetSingletonWinnerConditionByType<OculusKillLimitClient>() != null)
		{
			statType = GameStatCounterType.OculusKill;
			return true;
		}
		return false;
	}

	public static int GetPrioritizedStatLimit(GameStatCounterType gameStatType)
	{
		return gameStatType switch
		{
			GameStatCounterType.Flag => 0, 
			GameStatCounterType.Collectible => MVGameControllerBase.Game.WinningConditionManager.GetSingletonWinnerConditionByType<AllCollectiblesCollectedClient>().Limit, 
			GameStatCounterType.Kill => MVGameControllerBase.Game.WinningConditionManager.GetSingletonWinnerConditionByType<KillLimitClient>().Limit, 
			GameStatCounterType.OculusKill => MVGameControllerBase.Game.WinningConditionManager.GetSingletonWinnerConditionByType<OculusKillLimitClient>().Limit, 
			_ => 0, 
		};
	}

	public static bool IsNewScoreBetter(int newScore, int oldScore, GameStatCounterType statType)
	{
		switch (statType)
		{
		case GameStatCounterType.Kill:
		case GameStatCounterType.Collectible:
		case GameStatCounterType.OculusKill:
			if (newScore > oldScore)
			{
				return true;
			}
			break;
		case GameStatCounterType.Flag:
			if (oldScore < 0)
			{
				return true;
			}
			if (newScore <= 0)
			{
				return false;
			}
			if (newScore < oldScore || oldScore == 0)
			{
				return true;
			}
			break;
		}
		return false;
	}

	public static string MakeIntoScoreText(int score, GameStatCounterType statType)
	{
		switch (statType)
		{
		case GameStatCounterType.Kill:
		case GameStatCounterType.Collectible:
		case GameStatCounterType.OculusKill:
			return score.ToString();
		case GameStatCounterType.Flag:
		{
			string text = string.Empty;
			if (score == 0)
			{
				return "--:--";
			}
			score = (int)((float)score / 1000f);
			int num = score % 60;
			int num2 = Mathf.FloorToInt((float)score / 60f);
			if (num2 >= 60)
			{
				int num3 = Mathf.FloorToInt((float)num2 / 60f);
				num2 %= 60;
				text = text + num3 + ":";
			}
			string text2 = string.Empty;
			if (num < 10)
			{
				text2 += "0";
			}
			text2 += num;
			string text3 = string.Empty;
			if (num2 < 10)
			{
				text3 += "0";
			}
			text3 += num2;
			return text + text3 + ":" + text2;
		}
		default:
			return score.ToString();
		}
	}
}
