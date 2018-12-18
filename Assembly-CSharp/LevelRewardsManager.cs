using System;
using System.Collections.Generic;
using UnityEngine;

public class LevelRewardsManager
{
	private Dictionary<int, int> unseenLevelRewards = new Dictionary<int, int>();

	public Action OnRewardsReturned;

	public Dictionary<int, int> RewardsToShow => unseenLevelRewards;

	public KeyValuePair<int, int> NextReward { get; private set; }

	public void ClearRewards()
	{
		unseenLevelRewards.Clear();
	}

	public void SetNextLevelReward(int level, int gold)
	{
		Debug.Log("Next level reward: Level " + level.ToString() + ", Gold " + gold);
		NextReward = new KeyValuePair<int, int>(level, gold);
	}

	public void AddClaimedLevelRewards(Dictionary<int, int> levelRewards)
	{
		foreach (KeyValuePair<int, int> levelReward in levelRewards)
		{
			if (!unseenLevelRewards.ContainsKey(levelReward.Key))
			{
				unseenLevelRewards.Add(levelReward.Key, levelReward.Value);
			}
		}
		if (OnRewardsReturned != null)
		{
			OnRewardsReturned();
		}
	}
}
