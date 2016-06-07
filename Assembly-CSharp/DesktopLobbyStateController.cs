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
	private float rewardButtonLerpSpeed = 5f;

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

	private readonly AccessoryMover accessoryMover = new AccessoryMover();

	private Vector3 rewardButtonTarget = Vector3.zero;

	private Vector3 rewardHiddenSize = Vector3.zero;

	private Vector3 rewardShownSize = Vector3.one;

	private void Awake()
	{
		rewardButtonTarget = rewardShownSize;
		rewardTransform.localScale = rewardShownSize;
		playReward.transform.localScale = rewardHiddenSize;
	}

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
