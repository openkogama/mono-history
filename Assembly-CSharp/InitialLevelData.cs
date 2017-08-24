using System.Collections.Generic;

public class InitialLevelData
{
	public List<BadgeUrlData> BadgeUrlData;

	public int Level;

	public int XP;

	public XPLevelLimits XPLevelLimits;

	public override string ToString()
	{
		return $"BadgeUrlData.Length {BadgeUrlData.Count}. Level {Level}. XP {XP}. XPLevelLimits {XPLevelLimits}.";
	}
}
