using MV.Common;

namespace MV.WorldObject.GamePassSystem;

public class PlayerPlanetMetaDataClient
{
	public GamePassTier gamePassTierSeen;

	public bool welcomeRewardClaimed;

	public PlayerPlanetMetaDataClient()
	{
	}

	public PlayerPlanetMetaDataClient(GamePassTier gamePassTierSeen, bool welcomeRewardClaimed)
	{
		this.gamePassTierSeen = gamePassTierSeen;
		this.welcomeRewardClaimed = welcomeRewardClaimed;
	}

	public override string ToString()
	{
		return $"gamePassTierSeen {gamePassTierSeen}. welcomeRewardClaimed {welcomeRewardClaimed}.";
	}
}
