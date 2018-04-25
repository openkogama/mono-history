public class FinishLineReachedClient : FinishLineReached, IWinningConditionBriefing
{
	public FinishLineReachedClient(WinningCondition parent, int id, GameStatCounterManager gameCounterManager)
		: base(parent, id, gameCounterManager)
	{
	}

	public void GetBriefing(IBriefing winningConditionBriefingView)
	{
		winningConditionBriefingView.AddBriefing(WinningConditionType.FinishLine);
	}

	public void GetDebriefing(IDebriefing winningConditionDebriefingView)
	{
		winningConditionDebriefingView.SetupDebriefing(WinningConditionType.FinishLine, HighScores, IsTeamMode);
	}
}
