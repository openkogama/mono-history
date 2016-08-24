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
	private GameObject touristRegisterButton;

	[SerializeField]
	private GameObject accessoryShop;

	[SerializeField]
	private RewardGenerator rewardGenerator;

	[SerializeField]
	private GameObject touristRewardPreview;

	[SerializeField]
	private TimedPlayReward playReward;

	private void Start()
	{
		bool isTouristSession = MVGameControllerBase.IsTouristSession;
		touristRewardPreview.SetActive(isTouristSession);
		touristRegisterButton.SetActive(isTouristSession);
		if (!isTouristSession)
		{
			accessoryShop.SetActive(value: true);
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
		rewardTransform.gameObject.SetActive(value: true);
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
	}
}
