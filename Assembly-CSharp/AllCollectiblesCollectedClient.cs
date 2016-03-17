public class AllCollectiblesCollectedClient : AllCollectiblesCollected, IWinningConditionBriefing
{
	public AllCollectiblesCollectedClient(WinningCondition parent, int id, GameStatCounterManager gameCounterManager)
		: base(parent, id, gameCounterManager)
	{
	}

	public void GetBriefing(IBriefing winningConditionBriefingView)
	{
		winningConditionBriefingView.AddBriefing(WinningConditionType.Collectible, Limit);
	}

	public void GetDebriefing(IDebriefing winningConditionDebriefingView)
	{
		winningConditionDebriefingView.SetupDebriefing(WinningConditionType.Collectible, HighScores, IsTeamMode);
	}
}
