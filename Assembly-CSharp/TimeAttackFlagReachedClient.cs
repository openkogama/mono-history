public class TimeAttackFlagReachedClient : TimeAttackFlagReached, IWinningConditionBriefing
{
	public TimeAttackFlagReachedClient(WinningCondition parent, int id, GameStatCounterManager gameCounterManager)
		: base(parent, id, gameCounterManager)
	{
	}

	public void GetBriefing(IBriefing winningConditionBriefingView)
	{
		winningConditionBriefingView.AddBriefing(WinningConditionType.TimeAttackFlag);
	}

	public void GetDebriefing(IDebriefing winningConditionDebriefingView)
	{
		winningConditionDebriefingView.SetupDebriefing(WinningConditionType.TimeAttackFlag, HighScores, IsTeamMode);
	}
}
