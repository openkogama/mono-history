using System;

public class GameMeterAndroidOculus : GameMeterAndroidKillBase
{
	private OculusKillLimitClient oculusClient;

	private int prevValue;

	public override GameMeterType GameMeterType => GameMeterType.OculusKills;

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
		oculusClient = MVGameControllerBase.Game.WinningConditionManager.GetSingletonWinnerConditionByType<OculusKillLimitClient>();
		if (oculusClient != null)
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
		if (oculusClient != null)
		{
			SetCount(GameStatCounterType.OculusKill, oculusClient.Limit);
			int gameStat = MVGameControllerBase.Game.LocalPlayer.GetGameStat(GameStatCounterType.OculusKill);
			if (prevValue != gameStat && gameStat != 0)
			{
				prevValue = gameStat;
				NotificationController.PushNotification(string.Format(TM._("Destroyed oculus! {0}/{1}"), gameStat, oculusClient.Limit));
			}
		}
	}
}
