using System;
using System.Collections.Generic;
using MV.Common;
using MV.WorldObject;
using MV.WorldObject.AntiCheat;
using MV.WorldObject.GamePassSystem.GamePassProgressionDataObject;
using UnityEngine;

public static class GamePassProgressionController
{
	private static MVGamePassProgressionDataObject progressionDataObject;

	private static bool isInitialized;

	public static Action OnGamePassesProgressionUpdate;

	public static bool IsProgressionEnabled
	{
		get
		{
			if (!isInitialized)
			{
				Initialize();
			}
			if (progressionDataObject == null)
			{
				return false;
			}
			return progressionDataObject.EnableProgression;
		}
	}

	public static void Initialize()
	{
		List<MVWorldObjectClient> worldObjectsByType = MVGameControllerBase.WOCM.GetWorldObjectsByType(WorldObjectType.GamePassProgressionDataObject);
		if (worldObjectsByType.Count > 0)
		{
			progressionDataObject = (MVGamePassProgressionDataObject)worldObjectsByType[0];
		}
		isInitialized = true;
	}

	public static int GetXPReward(GamePassTier tier)
	{
		if (!isInitialized)
		{
			Initialize();
		}
		if (progressionDataObject == null)
		{
			return 0;
		}
		return progressionDataObject.GamePassProgressionDataObjectShared.xpTierRewards.xpTierRewards[tier];
	}

	public static void SetXPReward(GamePassTier tier, int xpReward)
	{
		if (!isInitialized)
		{
			Initialize();
		}
		RangeValidator<int> xPRewardRangeValidator = GetXPRewardRangeValidator(tier);
		xpReward = Mathf.Clamp(xpReward, xPRewardRangeValidator.min, xPRewardRangeValidator.max);
		GamePassProgressionDataObjectShared gamePassProgressionDataObjectShared = progressionDataObject.GamePassProgressionDataObjectShared;
		gamePassProgressionDataObjectShared.xpTierRewards.xpTierRewards[tier] = xpReward;
		progressionDataObject.GamePassProgressionDataObjectShared = gamePassProgressionDataObjectShared;
	}

	public static RangeValidator<int> GetXPRewardRangeValidator(GamePassTier tier)
	{
		if (!isInitialized)
		{
			Initialize();
		}
		return progressionDataObject.GamePassProgressionDataObjectSharedValidator.XpTiersRewardsValidator.xpTierRewardsValidators[tier].rangeValidator;
	}
}
