using System.Collections.Generic;
using MV.Common;

namespace MV.WorldObject.GamePassSystem.GamePassEarnings;

public class EarningsReport
{
	public Dictionary<GamePassTier, int> gamePassTierEarningsGold = new Dictionary<GamePassTier, int>();

	public Dictionary<string, int> gameBoosterEarningsGold = new Dictionary<string, int>();

	public int TotalEarningsGold => GetTotalEarningsGold();

	public EarningsReport()
	{
	}

	public EarningsReport(Dictionary<GamePassTier, int> gamePassTierEarningsGold, Dictionary<string, int> gameBoosterEarningsGold)
	{
		this.gamePassTierEarningsGold = gamePassTierEarningsGold;
		this.gameBoosterEarningsGold = gameBoosterEarningsGold;
	}

	public void AddTierGoldRevenue(int goldAmount, GamePassTier gamePassTier)
	{
		if (!gamePassTierEarningsGold.ContainsKey(gamePassTier))
		{
			gamePassTierEarningsGold.Add(gamePassTier, 0);
		}
		gamePassTierEarningsGold[gamePassTier] += goldAmount;
	}

	public void AddGameBoosterGoldRevenue(int goldAmount, string gameBooster)
	{
		if (!gameBoosterEarningsGold.ContainsKey(gameBooster))
		{
			gameBoosterEarningsGold.Add(gameBooster, 0);
		}
		gameBoosterEarningsGold[gameBooster] += goldAmount;
	}

	private int GetTotalEarningsGold()
	{
		int num = 0;
		foreach (KeyValuePair<GamePassTier, int> item in gamePassTierEarningsGold)
		{
			num += item.Value;
		}
		foreach (KeyValuePair<string, int> item2 in gameBoosterEarningsGold)
		{
			num += item2.Value;
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
