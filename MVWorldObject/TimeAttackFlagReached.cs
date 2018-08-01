public class TimeAttackFlagReached : WinningCondition
{
	public override bool IsSingleton => true;

	public TimeAttackFlagReached(WinningCondition parent, int id, GameStatCounterManager gameCounterManager)
		: base(parent, id, gameCounterManager, 0, isBriefingNode: true, GameStatCounterType.TimeAttackFlag, WinningConditionPresentStyle.OneWinner)
	{
	}

	protected override void GameCountersQuery_OnCounterTypeChanged(object sender, OnCounterTypeChangedArgs e)
	{
	}
}
