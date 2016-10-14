using System;
using MV.Common;
using UnityEngine;
using UnityEngine.Events;

public class DesktopLobbyStateController : MonoBehaviour
{
	[SerializeField]
	private GameObject teamButton;

	[SerializeField]
	private RectTransform rewardTransform;

	[SerializeField]
	private GameObject respawnButton;

	[SerializeField]
	private GameObject gameCoinBoosterButton;

	[SerializeField]
	private GameObject avatarAccessoriesButton;

	[SerializeField]
	private GameObject touristRegisterButton;

	[SerializeField]
	private GameObject touristRewardPreview;

	[SerializeField]
	private TimedPlayReward playReward;

	[SerializeField]
	private AdOfferGold adOfferGold;

	[SerializeField]
	private RewardGenerator rewardGenerator;

	private readonly AccessoryMover accessoryMover = new AccessoryMover();

	private void Start()
	{
		bool isTouristSession = MVGameControllerBase.IsTouristSession;
		bool flag = MVGameControllerBase.IEditModeUI == null && !isTouristSession && MVGameControllerBase.GameMode == MVGameMode.Play;
		playReward.gameObject.SetActive(value: false);
		bool active = MVGameControllerBase.IsTouristSession && MVClientSettings.ShowTouristPromotion;
		touristRegisterButton.SetActive(active);
		rewardTransform.gameObject.SetActive(flag);
		gameCoinBoosterButton.SetActive(!isTouristSession);
		avatarAccessoriesButton.SetActive(!isTouristSession);
		touristRewardPreview.SetActive(isTouristSession);
		if (flag)
		{
			rewardGenerator.Initialize();
			playReward.Initialize();
			RewardManager.NumberOfPendingRewardsChanged = (UnityAction)Delegate.Combine(RewardManager.NumberOfPendingRewardsChanged, new UnityAction(RewardChanged));
			RewardManager.TimerUpdated = (UnityAction)Delegate.Combine(RewardManager.TimerUpdated, new UnityAction(RewardChanged));
			if (RewardManager.TimerInitiated)
			{
				RewardChanged();
			}
		}
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
		MVInputWrapper.IsInGameInputSuppressed = true;
		accessoryMover.MoveAccessory();
	}

	private void OnDisable()
	{
		accessoryMover.Destroy();
	}

	private void OnEnable()
	{
		accessoryMover.Activate();
		if (MVGameControllerBase.Game.TeamManager.TeamCount() > 1)
		{
			teamButton.SetActive(value: true);
		}
		else
		{
			teamButton.SetActive(value: false);
		}
	}
}
