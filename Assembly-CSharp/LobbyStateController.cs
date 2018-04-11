using System;
using UnityEngine;
using UnityEngine.Events;

public class LobbyStateController : MonoBehaviour
{
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

	[SerializeField]
	private GameObject gameCoinBoosterButton;

	private void Start()
	{
		bool isTouristSession = MVGameControllerBase.IsTouristSession;
		touristRewardPreview.SetActive(isTouristSession && MVClientSettings.SpinEnabled);
		touristRegisterButton.SetActive(isTouristSession && !MVGameControllerBase.GameSessionData.IsPlayedFromPoki);
		if (!isTouristSession)
		{
			accessoryShop.SetActive(value: true);
			playReward.Initialize();
			gameCoinBoosterButton.SetActive(value: true);
			if (MVClientSettings.SpinEnabled)
			{
				rewardGenerator.Initialize();
				RewardManager.NumberOfPendingRewardsChanged = (UnityAction)Delegate.Combine(RewardManager.NumberOfPendingRewardsChanged, new UnityAction(RewardChanged));
				RewardManager.TimerUpdated = (UnityAction)Delegate.Combine(RewardManager.TimerUpdated, new UnityAction(RewardChanged));
				if (RewardManager.TimerInitiated)
				{
					RewardChanged();
				}
			}
		}
		playReward.gameObject.SetActive(value: false);
	}

	private void RewardChanged()
	{
		rewardTransform.gameObject.SetActive(value: true);
	}

	private void OnDisable()
	{
		if (MVGameControllerBase.Game != null)
		{
			MVGameControllerBase.WOCM.AvatarLocal.Body.AccessoryMoveOverride = false;
		}
	}

	private void OnEnable()
	{
		MVGameControllerBase.WOCM.AvatarLocal.Body.AccessoryMoveOverride = true;
	}
}
