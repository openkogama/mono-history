using MV.WorldObject;

public class OculusKillLimitClient : OculusKillLimit, IWinningConditionBriefing
{
	public OculusKillLimitClient(WinningCondition parent, int id, GameStatCounterManager gameCounterManager, int killLimit)
		: base(parent, id, gameCounterManager, killLimit)
	{
	}

	public void GetBriefing(IBriefing winningConditionBriefingView)
	{
		if (MVGameControllerBase.WOCM.GetWorldObjectsByType(WorldObjectType.AdvancedGhost).Count > 0)
		{
			winningConditionBriefingView.AddBriefing(WinningConditionType.Oculus, Limit);
		}
	}

	public void GetDebriefing(IDebriefing winningConditionDebriefingView)
	{
		winningConditionDebriefingView.SetupDebriefing(WinningConditionType.Oculus, HighScores, IsTeamMode);
	}
}
