using System;
using System.Collections.Generic;
using System.Linq;
using MV.Common;

namespace MV.WorldObject.GamePassSystem;

public class ProgressionTierThresholdsManager
{
	public bool gamePassRewardsActivated;

	public Dictionary<GamePassTier, ProgressionTierThresholds> progressionThresholds = new Dictionary<GamePassTier, ProgressionTierThresholds> { 
	{
		GamePassTier.Tier0,
		new ProgressionTierThresholds(0, 0, TimeSpan.Zero)
	} };

	public ProgressionTierThresholdsManager()
	{
	}

	public ProgressionTierThresholdsManager(bool gamePassRewardsActivated)
	{
		this.gamePassRewardsActivated = gamePassRewardsActivated;
	}

	public void Add(GamePassTier gamePassTier, ProgressionTierThresholds progressionTierThresholds)
	{
		progressionThresholds.Add(gamePassTier, progressionTierThresholds);
	}

	public Dictionary<GamePassTier, TierState> GetTierPricingState(int playerGamePoints, GamePassTier playerGamePassTier)
	{
		byte b = Enum.GetValues(typeof(GamePassTier)).Cast<byte>().Max();
		int num = 0;
		Dictionary<GamePassTier, TierState> dictionary = new Dictionary<GamePassTier, TierState>();
		for (byte b2 = 0; b2 <= b; b2++)
		{
			GamePassTier gamePassTier = (GamePassTier)b2;
			num += progressionThresholds[gamePassTier].gamePointRequirement;
			if (playerGamePoints >= num || (int)playerGamePassTier >= (int)gamePassTier)
			{
				TierState value = new TierState(TierLockState.Unlocked, 0, progressionThresholds[gamePassTier].gamePointRequirement, 0, progressionThresholds[gamePassTier].goldPriceRequirement);
				dictionary.Add(gamePassTier, value);
			}
			else
			{
				GamePassTier key = (GamePassTier)(b2 - 1);
				if (dictionary[key].tierLockState == TierLockState.Unlocked)
				{
					int gamePointRequirement = progressionThresholds[gamePassTier].gamePointRequirement;
					int num2 = num - gamePointRequirement;
					int num3 = Math.Max(playerGamePoints - num2, 0);
					double num4 = (double)num3 / (double)gamePointRequirement;
					int remainingGamePointRequired = gamePointRequirement - num3;
					int goldPriceRequirement = progressionThresholds[gamePassTier].goldPriceRequirement;
					int remainingGoldPriceRequired = goldPriceRequirement - (int)((double)goldPriceRequirement * num4);
					TierState value2 = new TierState(TierLockState.PurchaseUnlock, remainingGamePointRequired, gamePointRequirement, remainingGoldPriceRequired, goldPriceRequirement);
					dictionary.Add(gamePassTier, value2);
				}
				else
				{
					TierState value3 = new TierState(TierLockState.Locked, progressionThresholds[gamePassTier].gamePointRequirement, progressionThresholds[gamePassTier].gamePointRequirement, progressionThresholds[gamePassTier].goldPriceRequirement, progressionThresholds[gamePassTier].goldPriceRequirement);
					dictionary.Add(gamePassTier, value3);
				}
			}
		}
		return dictionary;
	}

	public GamePassTier GetUnlockedTier(int gamePoints, GamePassTier curGamePassTier)
	{
		if (!gamePassRewardsActivated)
		{
			return curGamePassTier;
		}
		byte b = Enum.GetValues(typeof(GamePassTier)).Cast<byte>().Max();
		int num = 0;
		GamePassTier result = GamePassTier.Tier0;
		for (byte b2 = 0; b2 <= b; b2++)
		{
			num += progressionThresholds[(GamePassTier)b2].gamePointRequirement;
			if (gamePoints < num)
			{
				break;
			}
			result = (GamePassTier)b2;
		}
		return result;
	}

	public override string ToString()
	{
		string text = $"gamePassRewardsActivated {gamePassRewardsActivated}.";
		foreach (KeyValuePair<GamePassTier, ProgressionTierThresholds> progressionThreshold in progressionThresholds)
		{
			text += $"\n{progressionThreshold.Key}. {progressionThreshold.Value}.";
		}
		return text;
	}
}
