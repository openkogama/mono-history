using MV.WorldObject;

public class OculusKillLimitClient : OculusKillLimit, IWinningConditionBriefing
{
	public OculusKillLimitClient(WinningCondition parent, int id, GameStatCounterManager gameCounterManager, int killLimit)
		: base(parent, id, gameCounterManager, killLimit)
	{
	}

	public void GetBriefing(MVGUIWinningConditionBriefingView winningConditionBriefingView)
	{
		if (MVGameControllerBase.WOCM.GetWorldObjectsByType(WorldObjectType.AdvancedGhost).Count > 0)
		{
			winningConditionBriefingView.AddBriefing("Oculus", Limit);
		}
	}

	public void GetDebriefing(MVGUIWinningConditionDebriefingView winningConditionDebriefingView)
	{
		winningConditionDebriefingView.SetupDebriefing("Oculus", HighScores, IsTeamMode);
	}
}
