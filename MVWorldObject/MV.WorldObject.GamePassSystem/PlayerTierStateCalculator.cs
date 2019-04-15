using System;
using System.Collections.Generic;
using System.Linq;
using MV.Common;

namespace MV.WorldObject.GamePassSystem;

public class PlayerTierStateCalculator
{
	public bool gamePassRewardsActivated;

	public bool gamePointVelocityIsZero;

	public int welcomeReward;

	public Dictionary<GamePassTier, PlayerTierThresholds> progressionThresholds = new Dictionary<GamePassTier, PlayerTierThresholds>();

	public PlayerTierStateCalculator()
	{
	}

	public PlayerTierStateCalculator(bool gamePassRewardsActivated, bool gamePointVelocityIsZero, int welcomeReward, Dictionary<GamePassTier, PlayerTierThresholds> progressionThresholds)
	{
		this.gamePassRewardsActivated = gamePassRewardsActivated;
		this.progressionThresholds = progressionThresholds;
		this.gamePointVelocityIsZero = gamePointVelocityIsZero;
		this.welcomeReward = welcomeReward;
	}

	public Dictionary<GamePassTier, PlayerTierState> GetTierPricingState(int playerGamePoints, GamePassTier playerGamePassTier)
	{
		if (!gamePointVelocityIsZero)
		{
			return GetTierPricingStateBasedOnUserGamePointAmount(playerGamePoints, playerGamePassTier);
		}
		return GetTierPricingStateBasedOnUserTier(playerGamePassTier);
	}

	private Dictionary<GamePassTier, PlayerTierState> GetTierPricingStateBasedOnUserTier(GamePassTier playerGamePassTier)
	{
		byte b = Enum.GetValues(typeof(GamePassTier)).Cast<byte>().Max();
		Dictionary<GamePassTier, PlayerTierState> dictionary = new Dictionary<GamePassTier, PlayerTierState>();
		for (byte b2 = 0; b2 <= b; b2++)
		{
			GamePassTier gamePassTier = (GamePassTier)b2;
			if ((int)playerGamePassTier >= (int)gamePassTier)
			{
				PlayerTierState value = new PlayerTierState(TierLockState.Unlocked, 0, progressionThresholds[gamePassTier].gamePointRequirement, 0, progressionThresholds[gamePassTier].goldPriceRequirement);
				dictionary.Add(gamePassTier, value);
			}
			else
			{
				GamePassTier key = (GamePassTier)(b2 - 1);
				if (dictionary[key].tierLockState == TierLockState.Unlocked)
				{
					PlayerTierState value2 = new PlayerTierState(TierLockState.PurchaseUnlock, progressionThresholds[gamePassTier].gamePointRequirement, progressionThresholds[gamePassTier].gamePointRequirement, progressionThresholds[gamePassTier].goldPriceRequirement, progressionThresholds[gamePassTier].goldPriceRequirement);
					dictionary.Add(gamePassTier, value2);
				}
				else
				{
					PlayerTierState value3 = new PlayerTierState(TierLockState.Locked, progressionThresholds[gamePassTier].gamePointRequirement, progressionThresholds[gamePassTier].gamePointRequirement, progressionThresholds[gamePassTier].goldPriceRequirement, progressionThresholds[gamePassTier].goldPriceRequirement);
					dictionary.Add(gamePassTier, value3);
				}
			}
		}
		return dictionary;
	}

	private Dictionary<GamePassTier, PlayerTierState> GetTierPricingStateBasedOnUserGamePointAmount(int playerGamePoints, GamePassTier playerGamePassTier)
	{
		byte b = Enum.GetValues(typeof(GamePassTier)).Cast<byte>().Max();
		int num = 0;
		Dictionary<GamePassTier, PlayerTierState> dictionary = new Dictionary<GamePassTier, PlayerTierState>();
		for (byte b2 = 0; b2 <= b; b2++)
		{
			GamePassTier gamePassTier = (GamePassTier)b2;
			num += progressionThresholds[gamePassTier].gamePointRequirement;
			if (playerGamePoints >= num || (int)playerGamePassTier >= (int)gamePassTier)
			{
				PlayerTierState value = new PlayerTierState(TierLockState.Unlocked, 0, progressionThresholds[gamePassTier].gamePointRequirement, 0, progressionThresholds[gamePassTier].goldPriceRequirement);
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
					PlayerTierState value2 = new PlayerTierState(TierLockState.PurchaseUnlock, remainingGamePointRequired, gamePointRequirement, remainingGoldPriceRequired, goldPriceRequirement);
					dictionary.Add(gamePassTier, value2);
				}
				else
				{
					PlayerTierState value3 = new PlayerTierState(TierLockState.Locked, progressionThresholds[gamePassTier].gamePointRequirement, progressionThresholds[gamePassTier].gamePointRequirement, progressionThresholds[gamePassTier].goldPriceRequirement, progressionThresholds[gamePassTier].goldPriceRequirement);
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
		GamePassTier gamePassTier = GamePassTier.Tier0;
		for (byte b2 = 0; b2 <= b; b2++)
		{
			num += progressionThresholds[(GamePassTier)b2].gamePointRequirement;
			if (gamePoints < num)
			{
				break;
			}
			gamePassTier = (GamePassTier)b2;
		}
		if ((int)curGamePassTier > (int)gamePassTier)
		{
			return curGamePassTier;
		}
		return gamePassTier;
	}

	public override string ToString()
	{
		string text = $"gamePassRewardsActivated {gamePassRewardsActivated}. welcomeReward {welcomeReward}.";
		foreach (KeyValuePair<GamePassTier, PlayerTierThresholds> progressionThreshold in progressionThresholds)
		{
			text += $"\n{progressionThreshold.Key}. {progressionThreshold.Value}.";
		}
		return text;
	}
}
