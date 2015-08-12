using System.Collections.Generic;

public class InitialLevelData
{
	public Dictionary<string, XPData> XPManagerData;

	public List<BadgeUrlData> BadgeUrlData;

	public int Level;

	public int XP;

	public XPLevelLimits XPLevelLimits;

	public int MinPlayersActivateXP = 2;

	public override string ToString()
	{
		return $"XPManagerData.Count {XPManagerData.Count}. BadgeUrlData.Length {BadgeUrlData.Count}. Level {Level}. XP {XP}. XPLevelLimits {XPLevelLimits}. MinPlayersActivateXP {MinPlayersActivateXP}.";
	}
}
