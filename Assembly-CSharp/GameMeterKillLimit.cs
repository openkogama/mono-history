using System;

public class GameMeterKillLimit : GameMeterKillBase
{
	private KillLimitClient killClient;

	private int prevValue;

	public override GameMeterType GameMeterType => GameMeterType.Kills;

	public override void Initialize()
	{
		MVGameControllerBase.Game.WinningConditionManager.OnWinningConditionReset += OnVictoryConditionMet;
	}

	private void OnVictoryConditionMet(object sender, EventArgs args)
	{
		UpdateValue();
	}

	private void OnDestroy()
	{
		if (MVGameControllerBase.IsAlive && MVGameControllerBase.Game != null)
		{
			MVGameControllerBase.Game.WinningConditionManager.OnWinningConditionReset -= OnVictoryConditionMet;
		}
	}

	public override void SetGameMeterVisibility()
	{
		killClient = MVGameControllerBase.Game.WinningConditionManager.GetSingletonWinnerConditionByType<KillLimitClient>();
		WinningConditionControl.TryGetPrioritizedWinCondition(out var condition);
		if (condition != WinningConditionType.Kill)
		{
			Hide();
		}
		else
		{
			Show();
		}
	}

	public override void UpdateValue()
	{
		if (killClient == null)
		{
			return;
		}
		SetCount(GameStatCounterType.Kill, killClient.Limit);
		int gameStat = MVGameControllerBase.Game.LocalPlayer.GetGameStat(GameStatCounterType.Kill);
		if (prevValue != gameStat && gameStat != 0)
		{
			prevValue = gameStat;
			for (int i = 0; i < gameMeterVisualEffects.Count; i++)
			{
				gameMeterVisualEffects[i].ExecuteEffect();
			}
		}
	}
}
