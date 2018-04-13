using System;
using System.Collections.Generic;

public abstract class WinningConditionGroup(WinningCondition parent, int id, GameStatCounterManager gameCounterManager, int limit, bool isBriefingNode, GameStatCounterType gameStatCounterType, WinningConditionPresentStyle winningConditionPresentStyle) : WinningCondition(parent, id, gameCounterManager, limit, isBriefingNode, gameStatCounterType, winningConditionPresentStyle)
{
	protected Dictionary<int, IWinningCondition> winnerConditions = new Dictionary<int, IWinningCondition>();

	public int Length => winnerConditions.Count;

	public override bool IsSingleton => false;

	public void AddWinnerCondition(WinningCondition winnerCondition)
	{
		winnerConditions.Add(winnerCondition.ID, winnerCondition);
		winnerCondition.OnWinningConditionChanged += winnerCondition_OnWinningConditionChanged;
	}

	public void RemoveWinnerCondition(int id)
	{
		winnerConditions[id].OnWinningConditionChanged -= winnerCondition_OnWinningConditionChanged;
		winnerConditions.Remove(id);
	}

	public override bool Traverse(Func<IWinningCondition, bool> callBack)
	{
		if (base.Traverse(callBack))
		{
			return true;
		}
		foreach (IWinningCondition value in winnerConditions.Values)
		{
			if (value.Traverse(callBack))
			{
				return true;
			}
		}
		return false;
	}

	protected abstract void winnerCondition_OnWinningConditionChanged(object sender, EventArgs e);
}
