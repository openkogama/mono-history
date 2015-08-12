using System;

public class CaptureTheFlag : WinningCondition
{
	public override bool IsSingleton => true;

	public CaptureTheFlag(WinningCondition parent, int id, GameStatCounterManager gameCounterManager)
		: base(parent, id, gameCounterManager, 1, isBriefingNode: true, GameStatCounterType.FlagCaptured, WinningConditionPresentStyle.OneWinner)
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
