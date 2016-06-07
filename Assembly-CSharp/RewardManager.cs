using System;
using System.Collections.Generic;
using MV.Common;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Events;

public static class RewardManager
{
	private static int rewardIndex = -1;

	private static bool isInitialized;

	private static bool timerInitiated;

	private static List<IActorRewardClient> actorRewards;

	private static UnityAction OnRewardsInitialized;

	private static UnityAction OnGetRewardCallback;

	private static UnityAction OnClaimRewardCallback;

	private static int numberOfPendingRewards;

	private static int countDownStartTick;

	private static int countDownTimeInMs;

	private static int spinPrice;

	public static UnityAction NumberOfPendingRewardsChanged { get; set; }

	public static UnityAction TimerUpdated { get; set; }

	public static int CountDownTimeInMS => countDownTimeInMs - WaitForTicks.Diff(countDownStartTick);

	public static bool TimerInitiated => timerInitiated;

	public static int NumberOfPendingRewards => numberOfPendingRewards;

	public static bool IsInitialized => isInitialized;

	public static int SpinPrice => spinPrice;

	public static IActorRewardClient CurrentReward => actorRewards[rewardIndex];

	public static void RequestInitialization(UnityAction rewardInitializeCallback)
	{
		if (isInitialized)
		{
			Debug.LogError("Is already initialized");
			return;
		}
		if (OnRewardsInitialized != null)
		{
			Debug.LogError("Already waiting for callback");
			return;
		}
		OnRewardsInitialized = rewardInitializeCallback;
		MVGameControllerBase.OperationRequests.GetRewardList();
	}

	public static void GetReward(UnityAction getRewardCallback)
	{
		if (OnGetRewardCallback != null)
		{
			Debug.LogError("Reward request already pending");
			return;
		}
		OnGetRewardCallback = getRewardCallback;
		MVGameControllerBase.OperationRequests.GetRewardIndex();
	}

	public static void UnsubscribeFromGetReward()
	{
		OnGetRewardCallback = null;
	}

	public static void ClaimReward(UnityAction claimRewardCallback)
	{
		OnClaimRewardCallback = claimRewardCallback;
		MVGameControllerBase.OperationRequests.ClaimReward();
	}

	public static void OnClaimReward()
	{
		if (OnClaimRewardCallback != null)
		{
			OnClaimRewardCallback();
			OnClaimRewardCallback = null;
		}
	}

	public static void SetRewardIndex(int rewardIndex)
	{
		RewardManager.rewardIndex = rewardIndex;
		if (OnGetRewardCallback != null)
		{
			OnGetRewardCallback();
			OnGetRewardCallback = null;
		}
	}

	public static void SetCountDownValues(int countDownTimeInMs, int countDownStartTick)
	{
		RewardManager.countDownTimeInMs = countDownTimeInMs;
		RewardManager.countDownStartTick = countDownStartTick;
		if (TimerUpdated != null)
		{
			TimerUpdated();
		}
		timerInitiated = true;
	}

	public static void SetNumberOfPendingRewards(int numberOfPendingRewards)
	{
		RewardManager.numberOfPendingRewards = numberOfPendingRewards;
		if (NumberOfPendingRewardsChanged != null)
		{
			NumberOfPendingRewardsChanged();
		}
	}

	public static void Initialize(List<KeyValuePair<int, string>> rewardData, int spinPrice)
	{
		RewardManager.spinPrice = spinPrice;
		actorRewards = GetActorRewards(rewardData);
		isInitialized = true;
		if (OnRewardsInitialized != null)
		{
			OnRewardsInitialized();
			OnRewardsInitialized = null;
		}
	}

	public static List<IActorRewardClient> GetPossibleRewards()
	{
		return actorRewards;
	}

	private static List<IActorRewardClient> GetActorRewards(List<KeyValuePair<int, string>> rewardData)
	{
		List<IActorRewardClient> list = new List<IActorRewardClient>();
		for (int i = 0; i < rewardData.Count; i++)
		{
			KeyValuePair<int, string> keyValuePair = rewardData[i];
			IActorRewardClient item = Create((RewardType)keyValuePair.Key, keyValuePair.Value);
			list.Add(item);
		}
		return list;
	}

	private static IActorRewardClient Create(RewardType rewardType, string jsonData)
	{
		return rewardType switch
		{
			RewardType.XPReward => (IActorRewardClient)JsonConvert.DeserializeObject<ActorXPRewardClient>(jsonData), 
			RewardType.GoldReward => JsonConvert.DeserializeObject<ActorGoldRewardClient>(jsonData), 
			RewardType.TestReward => JsonConvert.DeserializeObject<ActorTestRewardClient>(jsonData), 
			_ => throw new Exception("Unknown reward type"), 
		};
	}
}
