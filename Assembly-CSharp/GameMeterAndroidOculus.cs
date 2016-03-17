public class GameMeterAndroidOculus : GameMeterAndroidKillBase
{
	private OculusKillLimitClient oculusClient;

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
		}
		else
		{
			Hide();
		}
	}
}
