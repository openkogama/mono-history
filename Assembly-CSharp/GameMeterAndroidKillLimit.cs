public class GameMeterAndroidKillLimit : GameMeterAndroidKillBase
{
	private KillLimitClient killClient;

	public override GameMeterType GameMeterType => GameMeterType.Kills;

	private void Start()
	{
		UpdateShowGameMeter();
	}

	public override void UpdateShowGameMeter()
	{
		killClient = MVGameControllerBase.Game.WinningConditionManager.GetSingletonWinnerConditionByType<KillLimitClient>();
		if (killClient != null)
		{
			SetCount(GameStatCounterType.Kill, killClient.Limit);
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
