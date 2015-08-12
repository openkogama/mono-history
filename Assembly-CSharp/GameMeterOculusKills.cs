public class GameMeterOculusKills : GameMeterKillsBase
{
	public override GameMeterType GameMeterType => GameMeterType.OculusKills;

	private void Update()
	{
		OculusKillLimitClient singletonWinnerConditionByType = MVGameController.Game.WinningConditionManager.GetSingletonWinnerConditionByType<OculusKillLimitClient>();
		if (singletonWinnerConditionByType != null)
		{
			MeterActive = true;
			SetCount(GameStatCounterType.OculusKill, singletonWinnerConditionByType.Limit);
		}
		else
		{
			MeterActive = false;
			score.text = string.Empty;
		}
	}
}
