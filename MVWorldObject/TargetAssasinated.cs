using System;

public class TargetAssasinated : WinningCondition
{
	private readonly int assassinatorActorNumber;

	private readonly int assassineeActorNumber;

	public override bool IsSingleton => false;

	public TargetAssasinated(WinningCondition parent, int id, GameStatCounterManager gameCounterManager, int assassinatorActorNumber, int assassineeActorNumber)
		: base(parent, id, gameCounterManager, 1, isBriefingNode: false, GameStatCounterType.Kill, WinningConditionPresentStyle.OneWinner)
	{
		this.assassinatorActorNumber = assassinatorActorNumber;
		this.assassineeActorNumber = assassineeActorNumber;
	}

	protected override void GameCountersQuery_OnCounterTypeChanged(object sender, OnCounterTypeChangedArgs e)
	{
		if (e.actorNumber == assassinatorActorNumber && e.otherID == assassineeActorNumber)
		{
			SendWinningConditionChangedEvent(new EventArgs(), e);
		}
	}
}
