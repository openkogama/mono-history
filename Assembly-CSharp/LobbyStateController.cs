using System;
using MV.Common;
using UnityEngine;
using UnityEngine.Events;

public class LobbyStateController : MonoBehaviour
{
	[SerializeField]
	private RespawnButton respawnButton;

	[SerializeField]
	private RectTransform rewardTransform;

	[SerializeField]
	private float rewardButtonLerpSpeed = 5f;

	[SerializeField]
	private RewardGenerator rewardGenerator;

	[SerializeField]
	private GameObject touristRewardPreview;

	[SerializeField]
	private TimedPlayReward playReward;

	private Vector3 rewardButtonTarget = Vector3.zero;

	private Vector3 rewardHiddenSize = Vector3.zero;

	private Vector3 rewardShownSize = Vector3.one;

	private void Start()
	{
		rewardButtonTarget = rewardShownSize;
		rewardTransform.localScale = rewardButtonTarget;
		playReward.transform.localScale = rewardHiddenSize;
		bool isTouristSession = MVGameControllerBase.IsTouristSession;
		touristRewardPreview.SetActive(isTouristSession);
		if (!isTouristSession)
		{
			playReward.Initialize();
			RewardManager.NumberOfPendingRewardsChanged = (UnityAction)Delegate.Combine(RewardManager.NumberOfPendingRewardsChanged, new UnityAction(RewardChanged));
			RewardManager.TimerUpdated = (UnityAction)Delegate.Combine(RewardManager.TimerUpdated, new UnityAction(RewardChanged));
			if (RewardManager.TimerInitiated)
			{
				RewardChanged();
			}
		}
		playReward.gameObject.SetActive(value: false);
	}

	private void RewardChanged()
	{
		bool flag = RewardManager.CountDownTimeInMS > 0 || RewardManager.NumberOfPendingRewards > 0;
		rewardTransform.gameObject.SetActive(value: true);
		rewardButtonTarget = rewardHiddenSize;
		if (flag)
		{
			rewardButtonTarget = rewardShownSize;
		}
	}

	private void Update()
	{
		if (MVGameControllerBase.WOCM.AvatarLocal.AvatarRuntimeState != AvatarRuntimeState.Playing && respawnButton.gameObject.activeSelf)
		{
			respawnButton.gameObject.SetActive(value: false);
		}
		else if (MVGameControllerBase.WOCM.AvatarLocal.AvatarRuntimeState == AvatarRuntimeState.Playing && !respawnButton.gameObject.activeSelf)
		{
			respawnButton.gameObject.SetActive(value: true);
		}
		if (rewardTransform.localScale != rewardButtonTarget)
		{
			rewardTransform.localScale = Vector3.Lerp(rewardTransform.localScale, rewardButtonTarget, rewardButtonLerpSpeed * Time.deltaTime);
		}
		if (playReward.rewardAvailable)
		{
			playReward.transform.localScale = Vector3.Lerp(playReward.transform.localScale, Vector3.one, rewardButtonLerpSpeed * Time.deltaTime);
		}
	}
}
