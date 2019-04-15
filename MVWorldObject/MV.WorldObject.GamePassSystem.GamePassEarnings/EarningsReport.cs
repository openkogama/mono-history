using System.Collections.Generic;
using MV.Common;

namespace MV.WorldObject.GamePassSystem.GamePassEarnings;

public class EarningsReport
{
	public Dictionary<GamePassTier, int> gamePassTierEarningsGold = new Dictionary<GamePassTier, int>();

	public int TotalEarningsGold => GetTotalEarningsGold();

	public EarningsReport()
	{
	}

	public EarningsReport(Dictionary<GamePassTier, int> gamePassTierEarningsGold)
	{
		this.gamePassTierEarningsGold = gamePassTierEarningsGold;
	}

	public void AddGoldRevenue(int goldAmount, GamePassTier gamePassTier)
	{
		if (!gamePassTierEarningsGold.ContainsKey(gamePassTier))
		{
			gamePassTierEarningsGold.Add(gamePassTier, 0);
		}
		gamePassTierEarningsGold[gamePassTier] += goldAmount;
	}

	private int GetTotalEarningsGold()
	{
		int num = 0;
		foreach (KeyValuePair<GamePassTier, int> item in gamePassTierEarningsGold)
		{
			num += item.Value;
		}
		return num;
	}

	public override string ToString()
	{
		string text = $"total earnings gold {GetTotalEarningsGold()}.\n";
		foreach (KeyValuePair<GamePassTier, int> item in gamePassTierEarningsGold)
		{
			text += $"{item.Key}. {item.Value}.";
		}
		return text;
	}
}
