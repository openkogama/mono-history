public class KillLimitClient : KillLimit, IWinningConditionBriefing
{
	public KillLimitClient(WinningCondition parent, int id, GameStatCounterManager gameCounterManager, int killLimit)
		: base(parent, id, gameCounterManager, killLimit)
	{
	}

	public void GetBriefing(IBriefing winningConditionBriefingView)
	{
		winningConditionBriefingView.AddBriefing(WinningConditionType.Kill, Limit);
	}

	public void GetDebriefing(IDebriefing winningConditionDebriefingView)
	{
		winningConditionDebriefingView.SetupDebriefing(WinningConditionType.Kill, HighScores, IsTeamMode);
	}
}
