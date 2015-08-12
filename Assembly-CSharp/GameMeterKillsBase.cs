using UnityEngine;

public abstract class GameMeterKillsBase : GameMeterBase
{
	[SerializeField]
	protected UXText score;

	protected void SetCount(GameStatCounterType gameStatCounterType, int limit)
	{
		int count = GetCount(gameStatCounterType);
		string text = count + "/" + limit;
		score.Text = text;
	}

	private static int GetCount(GameStatCounterType gameStatCounterType)
	{
		if (MVGameController.Game.TeamManager.TeamCount() > 1)
		{
			return MVGameController.Game.GameStatCounterManager.GetTeamCount(gameStatCounterType, MVGameController.Game.LocalPlayer.Team);
		}
		return MVGameController.Game.LocalPlayer.GetGameStat(gameStatCounterType);
	}
}
