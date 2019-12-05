using MV.Common;
using UnityEngine;

public class GoldRewardManager
{
	private bool isGoldRewardGame;

	private bool isCountingDownGoldReward;

	private bool isGoldRewardDone;

	private float startTime;

	public bool IsGoldRewardGame => isGoldRewardGame;

	public bool IsCountingDownGoldReward => isCountingDownGoldReward;

	public bool IsGoldRewardDone => isGoldRewardDone;

	public void Initialize(bool isGoldRewardGame)
	{
		this.isGoldRewardGame = isGoldRewardGame;
	}

	public void StartGoldRewardCountdownWhenReady()
	{
		MVGameControllerBase.SpawnRoleDataMediatorLocal.SpawnRoleMode.OnChange += OnAvatarChangeState;
	}

	public float GetGoldRewardTimeLeft()
	{
		float num = 120f - (Time.time - startTime);
		if (num < 0f)
		{
			num = 0f;
		}
		return num;
	}

	public float GetGoldRewardCountdownProgressPercentage()
	{
		return (Time.time - startTime) / 120f;
	}

	public void OnClaimGoldReward()
	{
		isGoldRewardDone = true;
		isCountingDownGoldReward = false;
	}

	public bool CanGetGoldReward()
	{
		bool rewardedAdsEnabled = MVClientSettings.RewardedAdsEnabled;
		bool flag = MVGameControllerBase.GameMode == MVGameMode.Play;
		bool isTouristSession = MVGameControllerBase.IsTouristSession;
		return rewardedAdsEnabled && isGoldRewardGame && !isGoldRewardDone && flag && !isTouristSession;
	}

	private void OnAvatarChangeState(SpawnRoleModeType avatarMode)
	{
		if (avatarMode == SpawnRoleModeType.Playing)
		{
			MVGameControllerBase.SpawnRoleDataMediatorLocal.SpawnRoleMode.OnChange -= OnAvatarChangeState;
			StartGoldRewardCountdown();
		}
	}

	private void StartGoldRewardCountdown()
	{
		startTime = Time.time;
		isCountingDownGoldReward = true;
	}
}
