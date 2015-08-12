public class FlagReachedClient : FlagReached, IWinningConditionBriefing
{
	public FlagReachedClient(WinningCondition parent, int id, GameStatCounterManager gameCounterManager)
		: base(parent, id, gameCounterManager)
	{
	}

	public void GetBriefing(MVGUIWinningConditionBriefingView winningConditionBriefingView)
	{
		winningConditionBriefingView.AddBriefing("Flag");
	}

	public void GetDebriefing(MVGUIWinningConditionDebriefingView winningConditionDebriefingView)
	{
		winningConditionDebriefingView.SetupDebriefing("Flag", HighScores, IsTeamMode);
	}
}
