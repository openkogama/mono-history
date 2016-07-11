public class GameMeterAndroidOculus : GameMeterAndroidKillBase
{
	private OculusKillLimitClient oculusClient;

	private int prevValue;

	public override GameMeterType GameMeterType => GameMeterType.OculusKills;

	private void Start()
	{
		UpdateShowGameMeter();
	}

	public override void UpdateShowGameMeter()
	{
		oculusClient = MVGameControllerBase.Game.WinningConditionManager.GetSingletonWinnerConditionByType<OculusKillLimitClient>();
		if (oculusClient != null)
		{
			SetCount(GameStatCounterType.OculusKill, oculusClient.Limit);
			if (!gameObject.activeSelf)
			{
				Show();
			}
			int gameStat = MVGameControllerBase.Game.LocalPlayer.GetGameStat(GameStatCounterType.OculusKill);
			if (prevValue != gameStat && gameStat != 0)
			{
				prevValue = gameStat;
				NotificationController.PushNotification(string.Format(TM._("Destroyed oculus! {0}/{1}"), gameStat, oculusClient.Limit));
			}
		}
		else
		{
			Hide();
		}
	}
}
