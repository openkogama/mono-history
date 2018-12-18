using System;

namespace MV.WorldObject.GamePassSystem;

public class ProgressionTierThresholds
{
	public int goldPriceRequirement;

	public int gamePointRequirement;

	public TimeSpan estimatedRequiredPlaytime;

	public ProgressionTierThresholds()
	{
	}

	public ProgressionTierThresholds(int goldPriceRequirement, int gamePointRequirement, TimeSpan estimatedRequiredPlaytime)
	{
		this.goldPriceRequirement = goldPriceRequirement;
		this.gamePointRequirement = gamePointRequirement;
		this.estimatedRequiredPlaytime = estimatedRequiredPlaytime;
	}

	public override string ToString()
	{
		return $"goldPriceRequirement {goldPriceRequirement}. gamePointRequirement {gamePointRequirement}. estimatedRequiredPlaytime {estimatedRequiredPlaytime}.";
	}
}
