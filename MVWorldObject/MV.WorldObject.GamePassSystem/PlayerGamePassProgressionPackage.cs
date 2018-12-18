namespace MV.WorldObject.GamePassSystem;

public class PlayerGamePassProgressionPackage
{
	public PlayerPlanetData playerPlanetData;

	public ProgressionTierThresholdsManager progressionTierThresholdsManager;

	public override string ToString()
	{
		return $"playerPlanetData {playerPlanetData}.\nprogressionTierThresholdsManager {progressionTierThresholdsManager}.\n";
	}
}
