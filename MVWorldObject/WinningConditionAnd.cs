using System;
using System.Collections.Generic;

public class WinningConditionAnd : WinningConditionGroup
{
	public override bool IsSingleton => false;

	public WinningConditionAnd(WinningCondition parent, int id, GameStatCounterManager gameCounterManager)
		: base(parent, id, gameCounterManager, 0, isBriefingNode: false, GameStatCounterType.None, WinningConditionPresentStyle.NoWinner)
	{
	}

	protected override void winnerCondition_OnWinningConditionChanged(object sender, EventArgs e)
	{
		if (AllWinConditionForfilled())
		{
			SendWinningConditionChangedEvent(new EventArgs(), null);
		}
	}

	private bool AllWinConditionForfilled()
	{
		foreach (KeyValuePair<int, IWinningCondition> winnerCondition in winnerConditions)
		{
			if (!winnerCondition.Value.Forfilled)
			{
				return false;
			}
		}
		return true;
	}

	public override string ToString()
	{
		string text = "";
		foreach (KeyValuePair<int, IWinningCondition> winnerCondition in winnerConditions)
		{
			if (!string.IsNullOrEmpty(text))
			{
				text += " AND ";
			}
			text += winnerCondition.Value;
		}
		return text;
	}
}
