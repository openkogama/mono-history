using System;

public class GameMeterKillLimit : GameMeterKillBase
{
	private KillLimitClient killClient;

	private int prevValue;

	public override GameMeterType GameMeterType => GameMeterType.Kills;

	private void Start()
	{
		SetGameMeterVisibility();
		MVGameControllerBase.Game.WinningConditionManager.OnWinningConditionReset += OnVictoryConditionMet;
	}

	private void OnVictoryConditionMet(object sender, EventArgs args)
	{
		UpdateValue();
	}

	private void OnDestroy()
	{
		if (MVGameControllerBase.Game != null)
		{
			MVGameControllerBase.Game.WinningConditionManager.OnWinningConditionReset -= OnVictoryConditionMet;
		}
	}

	public override void SetGameMeterVisibility()
	{
		killClient = MVGameControllerBase.Game.WinningConditionManager.GetSingletonWinnerConditionByType<KillLimitClient>();
		if (killClient != null)
		{
			if (!gameObject.activeSelf)
			{
				Show();
			}
			UpdateValue();
		}
		else
		{
			Hide();
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
