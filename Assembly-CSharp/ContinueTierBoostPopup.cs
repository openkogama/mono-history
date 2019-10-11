using System;
using Assets.Scripts.AdIntegration;
using MV.Common;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ContinueTierBoostPopup : MonoBehaviour
{
	[SerializeField]
	private Text tierNumber;

	[SerializeField]
	private Text continueRewardDescriptionText;

	[SerializeField]
	private Image countdownFillImage;

	[SerializeField]
	private Text countdownText;

	[SerializeField]
	private TierUnlockedPopupController TierUnlockedPopupControllerPrefab;

	[SerializeField]
	private SpawnRoleMenu spawnRoleSelectionMenuPrefab;

	[SerializeField]
	private float countDownDuration;

	private float timeLeft = 100f;

	private const string rewardDescription = "Do you want to continue as Tier {0}?";

	private GamePassTier previousPreviewTier;

	private bool isWaitingForFreeTryTier;

	private bool shouldUpdate = true;

	public void OnWatchAd()
	{
		ShowAd();
	}

	public void OnDeclinePressed()
	{
		StopPreviewTier();
	}

	public void Initialize(int tier)
	{
		tierNumber.text = tier.ToString();
		continueRewardDescriptionText.text = string.Format(TM._("Do you want to continue as Tier {0}?"), tier);
	}

	private void Start()
	{
		timeLeft = countDownDuration;
		previousPreviewTier = GamePassesManager.PlayerPlanetData.previewGamePassTier;
	}

	private void Update()
	{
		bool isBlocked = false;
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
		{
			isBlocked = x.IsUIElementBlocked(gameObject);
		});
		if (shouldUpdate && !isBlocked)
		{
			timeLeft -= Time.deltaTime;
			float fillAmount = timeLeft / countDownDuration;
			countdownFillImage.fillAmount = fillAmount;
			if (timeLeft < 0f)
			{
				StopPreviewTier();
				timeLeft = 0f;
			}
			countdownText.text = Mathf.FloorToInt(timeLeft).ToString();
		}
	}

	private void ShowTierUnlock(bool wasPurchased, bool wasTempUnlocked)
	{
		TierUnlockedPopupController tierUnlockedPopupController = UnityEngine.Object.Instantiate(TierUnlockedPopupControllerPrefab);
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
		{
			x.Push(tierUnlockedPopupController.gameObject, UIPushOption.InvisibleBlocker, null, UIGroupFlags.InventoryUI);
		});
		tierUnlockedPopupController.Initialize(previousPreviewTier, wasPurchased, wasTempUnlocked);
	}

	private void ShowSpawnRoleSelectionMenu()
	{
		SpawnRoleMenu spawnRoleSelectionMenu = UnityEngine.Object.Instantiate(spawnRoleSelectionMenuPrefab);
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
		{
			x.Push(spawnRoleSelectionMenu.gameObject, UIPushOption.HideAll, null, UIGroupFlags.InventoryUI);
		});
		spawnRoleSelectionMenu.Initialize(MVGameControllerBase.LocalPlayer.Team);
		spawnRoleSelectionMenu.HideBackButton();
	}

	private void ShowAd()
	{
		shouldUpdate = false;
		MVGameControllerBase.AdManager.RequestRewardedAd(RewardedAdCallback, AdContext.PreviewTier);
	}

	private void RewardedAdCallback(RewardedAdResult result)
	{
		switch (result)
		{
		case RewardedAdResult.RewardUnlocked:
			ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
			{
				x.Pop();
			});
			break;
		case RewardedAdResult.RewardNotUnlocked:
			ExecuteEvents.ExecuteHierarchy(gameObject, null, (IModalPopupCreator x, BaseEventData y) =>
			{
				x.Create(TM._("The video was canceled. Your Free Try have not been activated."), TM._("Video canceled"));
			});
			shouldUpdate = true;
			break;
		case RewardedAdResult.ErrorClient:
		case RewardedAdResult.ErrorInternal:
			ExecuteEvents.ExecuteHierarchy(gameObject, null, (IModalPopupCreator x, BaseEventData y) =>
			{
				x.Create(TM._("Free try cannot be activated at this moment."), TM._("An error occurred"));
			});
			shouldUpdate = true;
			break;
		case RewardedAdResult.ErrorTimeout:
			break;
		}
	}

	public void StopPreviewTier()
	{
		MVGameControllerBase.OperationRequests.TogglePreviewTier();
		GamePassesManager.OnPlayerPlanetDataUpdated = (Action)Delegate.Combine(GamePassesManager.OnPlayerPlanetDataUpdated, new Action(OnPlayerPlanetDataUpdated));
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IModalPopupCreator x, BaseEventData y) =>
		{
			x.Create();
		});
		isWaitingForFreeTryTier = true;
	}

	private void OnPlayerPlanetDataUpdated()
	{
		GamePassTier previewGamePassTier = GamePassesManager.PlayerPlanetData.previewGamePassTier;
		if ((int)previewGamePassTier >= (int)previousPreviewTier)
		{
			return;
		}
		if (isWaitingForFreeTryTier)
		{
			ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
			{
				x.Pop();
			});
			isWaitingForFreeTryTier = false;
		}
		GamePassesManager.OnPlayerPlanetDataUpdated = (Action)Delegate.Remove(GamePassesManager.OnPlayerPlanetDataUpdated, new Action(OnPlayerPlanetDataUpdated));
		ExitContinuePopup();
	}

	private void ExitContinuePopup()
	{
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
		{
			x.Pop();
		});
		GamePassTier gamePassTier = MVGameControllerBase.LocalPlayer.SpawnRoleDataMediator.TierRequirement;
		GamePassTier gamePassTier2 = GamePassesManager.PlayerPlanetData.gamePassTier;
		if ((int)gamePassTier > (int)gamePassTier2)
		{
			ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
			{
				x.Pop();
			});
			ShowSpawnRoleSelectionMenu();
		}
	}
}
