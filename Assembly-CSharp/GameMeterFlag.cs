using UnityEngine;

public class GameMeterFlag : GameMeterBase
{
	public override GameMeterType GameMeterType => GameMeterType.Flag;

	private void Update()
	{
		if (!(Application.loadedLevelName == "GUIDevScene"))
		{
			FlagReachedClient singletonWinnerConditionByType = MVGameController.Game.WinningConditionManager.GetSingletonWinnerConditionByType<FlagReachedClient>();
			if (singletonWinnerConditionByType != null)
			{
				MeterActive = true;
			}
			else
			{
				MeterActive = false;
			}
		}
	}
}
