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
		if (MVGameControllerBase.Game.TeamManager.TeamCount() > 1)
		{
			return MVGameControllerBase.Game.GameStatCounterManager.GetTeamCount(gameStatCounterType, MVGameControllerBase.Game.LocalPlayer.Team);
		}
		return MVGameControllerBase.Game.LocalPlayer.GetGameStat(gameStatCounterType);
	}
}
