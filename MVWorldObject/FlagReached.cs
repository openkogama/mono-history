public class FlagReached : WinningCondition
{
	public override bool IsSingleton => true;

	public FlagReached(WinningCondition parent, int id, GameStatCounterManager gameCounterManager)
		: base(parent, id, gameCounterManager, 0, isBriefingNode: true, GameStatCounterType.Flag, WinningConditionPresentStyle.OneWinner)
	{
	}

	protected override void GameCountersQuery_OnCounterTypeChanged(object sender, OnCounterTypeChangedArgs e)
	{
	}
}
