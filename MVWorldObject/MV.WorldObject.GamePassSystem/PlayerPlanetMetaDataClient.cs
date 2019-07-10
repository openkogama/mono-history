using System;
using MV.Common;

namespace MV.WorldObject.GamePassSystem;

public class PlayerPlanetMetaDataClient
{
	public GamePassTier gamePassTierSeen;

	public bool welcomeRewardClaimed;

	public DateTime lastDailyWelcomeRewardClaim = DateTime.MinValue;

	public PlayerPlanetMetaDataClient()
	{
	}

	public PlayerPlanetMetaDataClient(GamePassTier gamePassTierSeen, bool welcomeRewardClaimed, DateTime lastDailyWelcomeRewardClaim)
	{
		this.gamePassTierSeen = gamePassTierSeen;
		this.welcomeRewardClaimed = welcomeRewardClaimed;
		this.lastDailyWelcomeRewardClaim = lastDailyWelcomeRewardClaim;
	}

	public override string ToString()
	{
		return $"gamePassTierSeen: {gamePassTierSeen}\nwelcomeRewardClaimed: {welcomeRewardClaimed}\nlastDailyWelcomeRewardClaim: {lastDailyWelcomeRewardClaim}";
	}

	public bool DailyWelcomeRewardClaimedToday()
	{
		DateTime utcNow = DateTime.UtcNow;
		if (lastDailyWelcomeRewardClaim.DayOfYear == utcNow.DayOfYear)
		{
			return lastDailyWelcomeRewardClaim.Year == utcNow.Year;
		}
		return false;
	}
}
