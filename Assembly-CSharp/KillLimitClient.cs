public class KillLimitClient : KillLimit, IWinningConditionBriefing
{
	public KillLimitClient(WinningCondition parent, int id, GameStatCounterManager gameCounterManager, int killLimit)
		: base(parent, id, gameCounterManager, killLimit)
	{
	}

	public void GetBriefing(MVGUIWinningConditionBriefingView winningConditionBriefingView)
	{
		winningConditionBriefingView.AddBriefing("Kill", Limit);
	}

	public void GetDebriefing(MVGUIWinningConditionDebriefingView winningConditionDebriefingView)
	{
		winningConditionDebriefingView.SetupDebriefing("Kill", HighScores, IsTeamMode);
	}
}
