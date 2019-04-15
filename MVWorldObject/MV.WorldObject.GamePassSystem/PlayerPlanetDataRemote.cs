using MV.Common;

namespace MV.WorldObject.GamePassSystem;

public class PlayerPlanetDataRemote
{
	public int highScoreGamePoints;

	public GamePassTier gamePassTier;

	public PlayerPlanetDataRemote()
	{
	}

	public PlayerPlanetDataRemote(int highScoreGamePoints, GamePassTier gamePassTier)
	{
		this.highScoreGamePoints = highScoreGamePoints;
		this.gamePassTier = gamePassTier;
	}

	public override string ToString()
	{
		return $"highScoreGamePoints {highScoreGamePoints}. gamePassTier {gamePassTier}.";
	}
}
