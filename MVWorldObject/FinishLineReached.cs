public class FinishLineReached : WinningCondition
{
	public override bool IsSingleton => true;

	public FinishLineReached(WinningCondition parent, int id, GameStatCounterManager gameCounterManager)
		: base(parent, id, gameCounterManager, 0, isBriefingNode: true, GameStatCounterType.FinishLine, WinningConditionPresentStyle.OneWinner)
	{
	}

	protected override void GameCountersQuery_OnCounterTypeChanged(object sender, OnCounterTypeChangedArgs e)
	{
	}
}
