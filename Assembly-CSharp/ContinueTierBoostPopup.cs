using System;
using System.Collections.Generic;
using Assets.Scripts.AdIntegration;
using MV.Common;
using MV.WorldObject;
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
	private Text tempClassContinueRewardDescriptionText;

	[SerializeField]
	private Image countdownFillImage;

	[SerializeField]
	private Text countdownText;

	[SerializeField]
	private GameObject defaultUI;

	[SerializeField]
	private GameObject tempClassUI;

	[SerializeField]
	private CurrentSpawnRolePreviewer spawnRolePreviewer;

	[SerializeField]
	private GameObject backgroundTier1;

	[SerializeField]
	private GameObject backgroundTier2;

	[SerializeField]
	private GameObject backgroundTier3;

	[SerializeField]
	private TierUnlockedPopupController TierUnlockedPopupControllerPrefab;

	[SerializeField]
	private SpawnRoleMenu spawnRoleSelectionMenuPrefab;

	[SerializeField]
	private TeamMenu teamMenuPrefab;

	[SerializeField]
	private float countDownDuration;

	[SerializeField]
	private Image buttonAdImage;

	private float timeLeft = 100f;

	private const string rewardDescription = "Keep playing as Tier {0}?";

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
		continueRewardDescriptionText.text = string.Format(TM._("Keep playing as Tier {0}?"), tier);
		tempClassContinueRewardDescriptionText.text = string.Format(TM._("Keep playing as Tier {0}?"), tier);
		bool flag = IsInTempClass();
		defaultUI.SetActive(!flag);
		tempClassUI.SetActive(flag);
		if (flag)
		{
			spawnRolePreviewer.SetupPreviewer(307, 614);
			ChangeBackground((GamePassTier)tier);
		}
		buttonAdImage.enabled = !GamePassesManager.TogglePreviewState.FreeTryWithoutAdAvailable;
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

	private void ShowTeamSelectionMenu()
	{
		TeamMenu newTeamMenu = UnityEngine.Object.Instantiate(teamMenuPrefab);
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
		{
			x.Push(newTeamMenu.gameObject, UIPushOption.HideAll | UIPushOption.InvisibleBlocker);
		});
		newTeamMenu.UpdateBackButtonVisibility();
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
				x.Create(TM._("The video was canceled. Your Free Try has not been activated."), TM._("Video canceled"));
			});
			shouldUpdate = true;
			break;
		case RewardedAdResult.ErrorClient:
		case RewardedAdResult.ErrorInternal:
		case RewardedAdResult.ErrorTimeout:
			ExecuteEvents.ExecuteHierarchy(gameObject, null, (IModalPopupCreator x, BaseEventData y) =>
			{
				x.Create(MVGameControllerBase.AdManager.RewardedAdNotAvailableText, TM._("No Ad Available"));
			});
			shouldUpdate = true;
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
		if (IsInTempClass())
		{
			ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
			{
				x.Pop();
			});
			if (CanSpawnInTeam(MVGameControllerBase.LocalPlayer.Team))
			{
				ShowSpawnRoleSelectionMenu();
			}
			else
			{
				ShowTeamSelectionMenu();
			}
		}
	}

	private bool IsInTempClass()
	{
		GamePassTier gamePassTier = MVGameControllerBase.LocalPlayer.SpawnRoleDataMediator.TierRequirement;
		GamePassTier gamePassTier2 = GamePassesManager.PlayerPlanetData.gamePassTier;
		return (int)gamePassTier > (int)gamePassTier2;
	}

	private bool CanSpawnInTeam(MVTeam team)
	{
		if (MVGameControllerBase.Game.TeamManager.GetTeamList().Count <= 1 || MVGameControllerBase.Game.TeamManager.TeamHasSpawnPoints(team))
		{
			return true;
		}
		List<MVWorldObjectClient> spawnPointsForTeam = MVGameControllerBase.Game.TeamManager.GetSpawnPointsForTeam(team);
		for (int i = 0; i < spawnPointsForTeam.Count; i++)
		{
			if (spawnPointsForTeam[i] is MVAvatarSpawnRoleCreator && (int)((MVAvatarSpawnRoleCreator)spawnPointsForTeam[i]).Tier < (int)(GamePassTier)MVGameControllerBase.LocalPlayer.SpawnRoleDataMediator.TierRequirement)
			{
				return true;
			}
		}
		return false;
	}

	private void ChangeBackground(GamePassTier tier)
	{
		bool flag = tier == GamePassTier.Tier1;
		bool flag2 = tier == GamePassTier.Tier2;
		bool flag3 = tier == GamePassTier.Tier3;
		if (backgroundTier1.activeSelf != flag)
		{
			backgroundTier1.SetActive(flag);
		}
		if (backgroundTier2.activeSelf != flag2)
		{
			backgroundTier2.SetActive(flag2);
		}
		if (backgroundTier3.activeSelf != flag3)
		{
			backgroundTier3.SetActive(flag3);
		}
	}
}
