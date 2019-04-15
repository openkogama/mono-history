using System.Collections.Generic;
using MV.Common;

namespace MV.WorldObject.GamePassSystem.GamePassProgressionDataObject;

public class GamePassProgressionDataObjectShared
{
	public class XPTierRewards
	{
		public Dictionary<GamePassTier, int> xpTierRewards = new Dictionary<GamePassTier, int>
		{
			{
				GamePassTier.Tier1,
				50
			},
			{
				GamePassTier.Tier2,
				100
			},
			{
				GamePassTier.Tier3,
				200
			}
		};

		public override string ToString()
		{
			string text = "Tier XP rewards\n";
			foreach (KeyValuePair<GamePassTier, int> xpTierReward in xpTierRewards)
			{
				text += $"   {xpTierReward.Key}. {xpTierReward.Value}.\n";
			}
			return text;
		}
	}

	public XPTierRewards xpTierRewards = new XPTierRewards();

	public GamePassProgressionDataObjectShared()
	{
	}

	public GamePassProgressionDataObjectShared(XPTierRewards xpTierRewards)
	{
		this.xpTierRewards = xpTierRewards;
	}

	public override string ToString()
	{
		return xpTierRewards.ToString();
	}
}
