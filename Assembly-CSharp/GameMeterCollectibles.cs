using UnityEngine;

public class GameMeterCollectibles : GameMeterBase
{
	[SerializeField]
	private UXText score;

	public override GameMeterType GameMeterType => GameMeterType.Collectibles;

	private void Update()
	{
		if (!(Application.loadedLevelName == "GUIDevScene"))
		{
			AllCollectiblesCollectedClient singletonWinnerConditionByType = MVGameControllerBase.Game.WinningConditionManager.GetSingletonWinnerConditionByType<AllCollectiblesCollectedClient>();
			if (singletonWinnerConditionByType != null)
			{
				MeterActive = true;
				string text = MVGameControllerBase.Game.LocalPlayer.GetGameStat(GameStatCounterType.Collectible) + "/" + singletonWinnerConditionByType.Limit;
				score.Text = text;
			}
			else
			{
				MeterActive = false;
				score.text = string.Empty;
			}
		}
	}
}
