using System;

public class GameMeterOculus : GameMeterKillBase
{
	private OculusKillLimitClient oculusClient;

	private int prevValue;

	public override GameMeterType GameMeterType => GameMeterType.OculusKills;

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
		oculusClient = MVGameControllerBase.Game.WinningConditionManager.GetSingletonWinnerConditionByType<OculusKillLimitClient>();
		WinningConditionControl.TryGetPrioritizedWinCondition(out var condition);
		if (condition != WinningConditionType.Oculus)
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
		if (oculusClient == null)
		{
			return;
		}
		SetCount(GameStatCounterType.OculusKill, oculusClient.Limit);
		int gameStat = MVGameControllerBase.Game.LocalPlayer.GetGameStat(GameStatCounterType.OculusKill);
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
