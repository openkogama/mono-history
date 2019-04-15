using System;

namespace MV.WorldObject.GamePassSystem;

public class PlayerTierThresholds
{
	public int goldPriceRequirement;

	public int gamePointRequirement;

	public TimeSpan estimatedRequiredPlaytime;

	public PlayerTierThresholds()
	{
	}

	public PlayerTierThresholds(int goldPriceRequirement, int gamePointRequirement, TimeSpan estimatedRequiredPlaytime)
	{
		this.goldPriceRequirement = goldPriceRequirement;
		this.gamePointRequirement = gamePointRequirement;
		this.estimatedRequiredPlaytime = estimatedRequiredPlaytime;
	}

	public static PlayerTierThresholds operator +(PlayerTierThresholds a, PlayerTierThresholds b)
	{
		return new PlayerTierThresholds(a.goldPriceRequirement + b.goldPriceRequirement, a.gamePointRequirement + b.gamePointRequirement, a.estimatedRequiredPlaytime + b.estimatedRequiredPlaytime);
	}

	public override string ToString()
	{
		return $"goldPriceRequirement {goldPriceRequirement}. gamePointRequirement {gamePointRequirement}. estimatedRequiredPlaytime {estimatedRequiredPlaytime}.";
	}
}
