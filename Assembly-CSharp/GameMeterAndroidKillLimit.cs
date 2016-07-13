public class GameMeterAndroidKillLimit : GameMeterAndroidKillBase
{
	private KillLimitClient killClient;

	private int prevValue;

	public override GameMeterType GameMeterType => GameMeterType.Kills;

	private void Start()
	{
		SetGameMeterVisibility();
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
		if (killClient != null)
		{
			SetCount(GameStatCounterType.Kill, killClient.Limit);
			int gameStat = MVGameControllerBase.Game.LocalPlayer.GetGameStat(GameStatCounterType.Kill);
			if (prevValue != gameStat && gameStat != 0)
			{
				prevValue = gameStat;
				NotificationController.PushNotification(string.Format(TM._("Killed enemy! {0}/{1}"), gameStat, killClient.Limit));
			}
		}
	}
}
