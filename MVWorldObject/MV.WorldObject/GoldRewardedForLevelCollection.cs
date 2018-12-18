using System.Collections.Generic;

namespace MV.WorldObject;

public class GoldRewardedForLevelCollection
{
	public Dictionary<int, int> levelGoldRewards;

	public GoldRewardedForLevelCollection()
	{
	}

	public GoldRewardedForLevelCollection(Dictionary<int, int> levelGoldRewards)
	{
		this.levelGoldRewards = new Dictionary<int, int>(levelGoldRewards);
	}

	public override string ToString()
	{
		string text = "";
		foreach (KeyValuePair<int, int> levelGoldReward in levelGoldRewards)
		{
			text += $"Level {levelGoldReward.Key}. GoldReward {levelGoldReward.Value}.\n";
		}
		return text;
	}
}
