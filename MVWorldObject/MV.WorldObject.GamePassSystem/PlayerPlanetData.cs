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

	public GamePassTier previewGamePassTier;

	public PlayerPlanetData()
	{
	}

	public PlayerPlanetData(int highScoreGamePoints, int progressionGamePoints, TimeSpan playtime, GamePassTier gamePassTier, GamePassTier previewGamePassTier, int rank, PlayerPlanetMetaDataClient playerPlanetMetaData)
	{
		this.highScoreGamePoints = highScoreGamePoints;
		this.rank = rank;
		this.progressionGamePoints = progressionGamePoints;
		this.gamePassTier = gamePassTier;
		this.previewGamePassTier = previewGamePassTier;
		this.playtime = playtime;
		this.playerPlanetMetaData = new PlayerPlanetMetaDataClient(playerPlanetMetaData.gamePassTierSeen, playerPlanetMetaData.welcomeRewardClaimed, playerPlanetMetaData.lastDailyWelcomeRewardClaim);
	}

	public GamePassTier GetGamePassTierWithPreview()
	{
		return (GamePassTier)Math.Max((byte)gamePassTier, (byte)previewGamePassTier);
	}

	public void UpdateWithPurchase(int deltaGamePoints, GamePassTier gamePassTier)
	{
		progressionGamePoints += deltaGamePoints;
		this.gamePassTier = gamePassTier;
	}

	public override string ToString()
	{
		return $"rank {rank}. highScoreGamePoints {highScoreGamePoints}. gamePoints {progressionGamePoints}. gamePassTier {gamePassTier}. previewGamePassTier {previewGamePassTier}. playtime {playtime}. PlayerPlanetMetaData {playerPlanetMetaData}";
	}
}
