using System;
using MV.Common;

namespace MV.WorldObject.GamePassSystem;

public class PlayerPlanetData
{
	public int gamePoints;

	public int paidGamePoints;

	public TimeSpan playtime = TimeSpan.Zero;

	public GamePassTier gamePassTier;

	public PlayerPlanetData()
	{
	}

	public PlayerPlanetData(int gamePoints, int paidGamePoints, TimeSpan playtime, GamePassTier gamePassTier)
	{
		this.gamePoints = gamePoints;
		this.gamePassTier = gamePassTier;
		this.paidGamePoints = paidGamePoints;
		this.playtime = playtime;
	}

	public override string ToString()
	{
		return $"gamePoints {gamePoints}. gamePassTier {gamePassTier}. paidGamePoints {paidGamePoints}. playtime {playtime}.";
	}
}
