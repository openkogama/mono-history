public class GameMeterKills : GameMeterKillsBase
{
	public override GameMeterType GameMeterType => GameMeterType.Kills;

	private void Update()
	{
		KillLimitClient singletonWinnerConditionByType = MVGameController.Game.WinningConditionManager.GetSingletonWinnerConditionByType<KillLimitClient>();
		if (singletonWinnerConditionByType != null)
		{
			MeterActive = true;
			SetCount(GameStatCounterType.Kill, singletonWinnerConditionByType.Limit);
		}
		else
		{
			MeterActive = false;
			score.text = string.Empty;
		}
	}
}
