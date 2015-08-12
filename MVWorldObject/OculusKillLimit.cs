using System;

public class OculusKillLimit : WinningCondition
{
	public override bool IsSingleton => true;

	public OculusKillLimit(WinningCondition parent, int id, GameStatCounterManager gameCounterManager, int killLimit)
		: base(parent, id, gameCounterManager, killLimit, isBriefingNode: true, GameStatCounterType.OculusKill, WinningConditionPresentStyle.MultipleWinners)
	{
	}

	protected override void GameCountersQuery_OnCounterTypeChanged(object sender, OnCounterTypeChangedArgs e)
	{
		if (!IsTeamMode)
		{
			if (e.count >= Limit)
			{
				SendWinningConditionChangedEvent(new EventArgs(), e);
			}
			return;
		}
		int teamCount = gameCounterManager.GetTeamCount(GameStatCounterType, e.team);
		if (teamCount >= Limit)
		{
			SendWinningConditionChangedEvent(new EventArgs(), e);
		}
	}
}
