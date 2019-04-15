namespace MV.WorldObject.GamePassSystem;

public class PlayerGamePassProgressionPackage
{
	public PlayerPlanetData playerPlanetData;

	public PlayerTierStateCalculator playerTierStateCalculator;

	public override string ToString()
	{
		return $"playerPlanetData {playerPlanetData}.\nplayerTierStateCalculator {playerTierStateCalculator}.\n";
	}
}
