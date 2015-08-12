using System;

public class WinningConditionOr : WinningConditionGroup
{
	public WinningConditionOr(WinningCondition parent, int id, GameStatCounterManager gameCounterManager, bool isBriefingNode, GameStatCounterType gameStatCounterType, WinningConditionPresentStyle winningConditionPresentStyle)
		: base(parent, id, gameCounterManager, 1, isBriefingNode, gameStatCounterType, winningConditionPresentStyle)
	{
	}

	protected override void winnerCondition_OnWinningConditionChanged(object sender, EventArgs e)
	{
		SendWinningConditionChangedEvent(e, null);
	}
}
