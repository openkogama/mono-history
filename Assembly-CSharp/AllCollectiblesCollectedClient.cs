public class AllCollectiblesCollectedClient : AllCollectiblesCollected, IWinningConditionBriefing
{
	public AllCollectiblesCollectedClient(WinningCondition parent, int id, GameStatCounterManager gameCounterManager)
		: base(parent, id, gameCounterManager)
	{
	}

	public void GetBriefing(MVGUIWinningConditionBriefingView winningConditionBriefingView)
	{
		winningConditionBriefingView.AddBriefing("Collectible", Limit);
	}

	public void GetDebriefing(MVGUIWinningConditionDebriefingView winningConditionDebriefingView)
	{
		winningConditionDebriefingView.SetupDebriefing("Collectible", HighScores, IsTeamMode);
	}
}
