using System;
using System.Collections.Generic;
using MV.Common;
using MV.WorldObject.AntiCheat;

namespace MV.WorldObject.GamePassSystem.GamePassProgressionDataObject;

public class GamePassProgressionDataObjectSharedValidator
{
	public class XPTiersRewardsValidator
	{
		public class XPTierRewardValidator
		{
			public bool isRemovalAllowed;

			public RangeValidator<int> rangeValidator;

			public XPTierRewardValidator()
			{
			}

			public XPTierRewardValidator(bool isRemovalAllowed, RangeValidator<int> rangeValidator)
			{
				this.isRemovalAllowed = isRemovalAllowed;
				this.rangeValidator = rangeValidator;
			}

			public int Validate(int xpTierReward, bool fixIfInValid)
			{
				if (xpTierReward == 0 && isRemovalAllowed)
				{
					return xpTierReward;
				}
				if (xpTierReward == 0 && !isRemovalAllowed)
				{
					if (!fixIfInValid)
					{
						throw new Exception("Reward removal is illegal");
					}
					xpTierReward = rangeValidator.min;
				}
				xpTierReward = rangeValidator.Validate(xpTierReward, fixIfInValid);
				return xpTierReward;
			}

			public override string ToString()
			{
				return $"isRemovalAllowed {isRemovalAllowed}. xpRewardRange {rangeValidator}.";
			}
		}

		public Dictionary<GamePassTier, XPTierRewardValidator> xpTierRewardsValidators = new Dictionary<GamePassTier, XPTierRewardValidator>();

		public XPTiersRewardsValidator()
		{
		}

		public XPTiersRewardsValidator(Dictionary<GamePassTier, XPTierRewardValidator> xpTierRewardsValidators)
		{
			this.xpTierRewardsValidators = xpTierRewardsValidators;
		}

		public XPTiersRewardsValidator(Dictionary<GamePassTier, RangeValidator<int>> xpRangeValidators, Dictionary<GamePassTier, bool> xpRewardRemovalAllowed)
		{
			foreach (KeyValuePair<GamePassTier, RangeValidator<int>> xpRangeValidator in xpRangeValidators)
			{
				GamePassTier key = xpRangeValidator.Key;
				bool isRemovalAllowed = xpRewardRemovalAllowed[key];
				XPTierRewardValidator value = new XPTierRewardValidator(isRemovalAllowed, xpRangeValidator.Value);
				xpTierRewardsValidators.Add(key, value);
			}
		}

		public void Validate(Dictionary<GamePassTier, int> xpTierRewards, bool fixIfInValid)
		{
			if (xpTierRewards.Count != xpTierRewardsValidators.Count)
			{
				throw new Exception("rewards count not matching");
			}
			foreach (KeyValuePair<GamePassTier, XPTierRewardValidator> xpTierRewardsValidator in xpTierRewardsValidators)
			{
				int xpTierReward = xpTierRewards[xpTierRewardsValidator.Key];
				xpTierReward = xpTierRewardsValidator.Value.Validate(xpTierReward, fixIfInValid);
				xpTierRewards[xpTierRewardsValidator.Key] = xpTierReward;
			}
		}

		public override string ToString()
		{
			string text = "";
			foreach (KeyValuePair<GamePassTier, XPTierRewardValidator> xpTierRewardsValidator in xpTierRewardsValidators)
			{
				text += $"{xpTierRewardsValidator.Key}. {xpTierRewardsValidator.Value}\n";
			}
			return text;
		}
	}

	public XPTiersRewardsValidator XpTiersRewardsValidator = new XPTiersRewardsValidator();

	public GamePassProgressionDataObjectSharedValidator()
	{
	}

	public GamePassProgressionDataObjectSharedValidator(XPTiersRewardsValidator xpTiersRewardsValidator)
	{
		XpTiersRewardsValidator = xpTiersRewardsValidator;
	}

	public GamePassProgressionDataObjectSharedValidator(Dictionary<GamePassTier, RangeValidator<int>> xpRangeValidators, Dictionary<GamePassTier, bool> xpRewardRemovalAllowed)
	{
		XpTiersRewardsValidator = new XPTiersRewardsValidator(xpRangeValidators, xpRewardRemovalAllowed);
	}

	public GamePassProgressionDataObjectShared Validate(GamePassProgressionDataObjectShared gamePassProgressionDataObjectShared, bool fixIfInValid)
	{
		XpTiersRewardsValidator.Validate(gamePassProgressionDataObjectShared.xpTierRewards.xpTierRewards, fixIfInValid);
		return gamePassProgressionDataObjectShared;
	}

	public override string ToString()
	{
		return $"{XpTiersRewardsValidator}";
	}
}
