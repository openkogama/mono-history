public class FlagReachedClient : FlagReached, IWinningConditionBriefing
{
	public FlagReachedClient(WinningCondition parent, int id, GameStatCounterManager gameCounterManager)
		: base(parent, id, gameCounterManager)
	{
	}

	public void GetBriefing(IBriefing winningConditionBriefingView)
	{
		winningConditionBriefingView.AddBriefing(WinningConditionType.Flag);
	}

	public void GetDebriefing(IDebriefing winningConditionDebriefingView)
	{
		winningConditionDebriefingView.SetupDebriefing(WinningConditionType.Flag, HighScores, IsTeamMode);
	}
}
