using System;
using MV.Common;

namespace MV.WorldObject.GamePassSystem;

public class PlayerPlanetData
{
	public int highScoreGamePoints;

	public int rank;

	public int progressionGamePoints;

	public TimeSpan playtime = TimeSpan.Zero;

	public GamePassTier gamePassTier;

	public PlayerPlanetMetaDataClient playerPlanetMetaData = new PlayerPlanetMetaDataClient();

	public PlayerPlanetData()
	{
	}

	public PlayerPlanetData(int highScoreGamePoints, int progressionGamePoints, TimeSpan playtime, GamePassTier gamePassTier, int rank, PlayerPlanetMetaDataClient playerPlanetMetaData)
	{
		this.highScoreGamePoints = highScoreGamePoints;
		this.rank = rank;
		this.progressionGamePoints = progressionGamePoints;
		this.gamePassTier = gamePassTier;
		this.playtime = playtime;
		this.playerPlanetMetaData = new PlayerPlanetMetaDataClient(playerPlanetMetaData.gamePassTierSeen, playerPlanetMetaData.welcomeRewardClaimed);
	}

	public void UpdateWithPurchase(int deltaGamePoints, GamePassTier gamePassTier)
	{
		progressionGamePoints += deltaGamePoints;
		this.gamePassTier = gamePassTier;
	}

	public override string ToString()
	{
		return $"rank {rank}. highScoreGamePoints {highScoreGamePoints}. gamePoints {progressionGamePoints}. gamePassTier {gamePassTier}. playtime {playtime}. PlayerPlanetMetaData {playerPlanetMetaData}";
	}
}
