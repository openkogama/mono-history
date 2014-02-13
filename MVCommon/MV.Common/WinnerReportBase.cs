using System.Collections.Generic;

namespace MV.Common;

public class WinnerReportBase
{
	public MVWinningState winningState;

	public MVWinningCondition winningType;

	public List<WinnerListNode> winnerList;

	public bool isTeamGame;

	public WinnerReportBase()
	{
		winningState = MVWinningState.NoWinner;
		winningType = MVWinningCondition.ReachTheFlagFirst;
		winnerList = new List<WinnerListNode>();
	}

	public void AddWinner(int actorNr, int timeMS)
	{
		WinnerListNode item = new WinnerListNode(actorNr, timeMS);
		winnerList.Add(item);
	}

	public void AddWinners(Dictionary<int, int> orderedList)
	{
		foreach (KeyValuePair<int, int> ordered in orderedList)
		{
			AddWinner(ordered.Key, ordered.Value);
		}
	}

	public int[] GetWinnerListActors()
	{
		int[] array = new int[winnerList.Count];
		int num = 0;
		foreach (WinnerListNode winner in winnerList)
		{
			array[num++] = winner.actorNr;
		}
		return array;
	}

	public int[] GetWinnerListTimes()
	{
		int[] array = new int[winnerList.Count];
		int num = 0;
		foreach (WinnerListNode winner in winnerList)
		{
			array[num++] = winner.data;
		}
		return array;
	}
}
