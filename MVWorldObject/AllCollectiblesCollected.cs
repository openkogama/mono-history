using System;

public class AllCollectiblesCollected : WinningCondition
{
	public override HighScores HighScores => gameCounterManager.GetHighScores(GameStatCounterType, presentAsTeamScore: false, WinningConditionPresentStyle.OneWinner, byAscending: false);

	public override bool IsSingleton => true;

	public AllCollectiblesCollected(WinningCondition parent, int id, GameStatCounterManager gameCounterManager)
		: base(parent, id, gameCounterManager, 0, isBriefingNode: true, GameStatCounterType.Collectible, WinningConditionPresentStyle.MultipleWinners)
	{
	}

	protected override void GameCountersQuery_OnCounterTypeChanged(object sender, OnCounterTypeChangedArgs e)
	{
		if (e.count == Limit)
		{
			SendWinningConditionChangedEvent(new EventArgs(), e);
		}
	}
}
