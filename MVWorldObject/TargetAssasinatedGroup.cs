public class TargetAssasinatedGroup : WinningConditionOr
{
	public override bool IsSingleton => true;

	public TargetAssasinatedGroup(WinningCondition parent, int id, GameStatCounterManager gameCounterManager)
		: base(parent, id, gameCounterManager, isBriefingNode: true, GameStatCounterType.Kill, WinningConditionPresentStyle.OneWinner)
	{
	}
}
