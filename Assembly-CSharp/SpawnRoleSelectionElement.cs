using System;
using System.Collections.Generic;
using Assets.Scripts.AdIntegration;
using MV.Common;
using MV.WorldObject;
using MV.WorldObject.KogamaSettings.KogamaSettingsCore.KogamaSettingTypes;
using MV.WorldObject.KogamaSettings.SpecializedSettingsTypes.AttributeSettings;
using MV.WorldObject.KogamaSettings.SpecializedSettingsTypes.AttributeSettings.AttributeSettingTypes;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.UI;

public class SpawnRoleSelectionElement : DefaultSpawnRoleSelectionElement
{
	[SerializeField]
	private Text spawnRoleCostAmount;

	[SerializeField]
	private GameObject spawnRoleCostObject;

	[SerializeField]
	private NotificationFade spawnRoleCostFader;

	[SerializeField]
	private GameObject moreInfoButton;

	[SerializeField]
	private NotificationFade moreInfoButtonFader;

	[SerializeField]
	private NotificationFade playButtonFader;

	[SerializeField]
	private GameObject playButton;

	[SerializeField]
	private GameObject freeTryButton;

	[SerializeField]
	private GameObject lockedButton;

	[SerializeField]
	private ContinueButtonHandler continueButtonHandler;

	[SerializeField]
	private GamePassesTextBubble lockedTierBubble;

	[SerializeField]
	private SpawnRoleSelectionSkillMenu skillMenuPrefab;

	[SerializeField]
	private SpawnRoleUnlockedPopupController spawnRoleUnlockPopupPrefab;

	[SerializeField]
	private GameObject backgroundTier1;

	[SerializeField]
	private GameObject backgroundTier2;

	[SerializeField]
	private GameObject backgroundTier3;

	private GamePassTier tierRequirement;

	private MVTeam teamRequirement;

	private bool awaitingSpawn;

	private bool isWaitingForFreeTryTier;

	private bool haveShownFreeTryUnlock;

	public override GamePassTier Tier => tierRequirement;

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

	public override void Initialize(int spawnRoleIndex, int woId, GamePassTier tierRequirement, MVTeam team, UnityAction<int> onSelectedCallback, UnityAction<int> onActivatedCallback)
	{
		base.Initialize(spawnRoleIndex, woId, tierRequirement, team, onSelectedCallback, onActivatedCallback);
		this.tierRequirement = tierRequirement;
		teamRequirement = team;
		int skillCost = CalculateTotalSpawnRoleCost(woId);
		spawnRoleCostAmount.text = skillCost.ToString();
		spawnRoleCostAmount.color = SpawnRolesSkillDataManager.GetCostColor(skillCost);
		spawnRoleCostObject.SetActive(value: false);
		moreInfoButton.SetActive(value: false);
		spawnRoleCostFader.ShouldHideWhenDone = false;
		moreInfoButtonFader.ShouldHideWhenDone = false;
		ChangeBackground(tierRequirement);
		HandlePlayButtonVisibility();
		ContinueButtonHandler continueButtonHandler = this.continueButtonHandler;
		continueButtonHandler.OnClick = (Action)Delegate.Combine(continueButtonHandler.OnClick, new Action(OnPressPlay));
	}

	public void ShowSkillMenu()
	{
		OnShowSkillMenu();
	}

	public void OnShowSkillMenu()
	{
		SpawnRoleSelectionSkillMenu skillMenu = UnityEngine.Object.Instantiate(skillMenuPrefab);
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
		{
			x.Push(skillMenu.gameObject, UIPushOption.InvisibleBlocker, null, UIGroupFlags.InventoryUI);
		});
		skillMenu.Initialize(WOID, tierRequirement, spawnRolePreviewObject);
	}

	public override void UpdateButtonUI()
	{
		base.UpdateButtonUI();
		HandlePlayButtonVisibility();
	}

	public void OnPressPlay()
	{
		if (MVGameControllerBase.Game.TeamManager.TeamCount() > 1)
		{
			HandleTeamSwitching();
		}
		int wOID = WOID;
		bool flag = wOID == MVGameControllerBase.LocalPlayer.SpawnRoleDataMediator.WoId;
		if (!FirstTimePressPlayController.HaveBeenPressed)
		{
			FirstTimePressPlayController.OnFirstTimePlayIsPressed();
		}
		if (flag)
		{
			Close();
			StartPlaying();
		}
		else
		{
			AwaitSpawnThenClose();
			MVGameControllerBase.Game.LocalPlayer.CreateSpawnRole(wOID);
			MVGameControllerDesktop.LockCursorManager.CursorLockWithoutCallback = true;
		}
	}

	public void OnPressFreePlay()
	{
		ShowAd();
	}

	public void OnPressLockedPlay()
	{
		GamePassTier gamePassTier = GamePassesManager.PlayerPlanetData.gamePassTier;
		GamePassTier previewGamePassTier = GamePassesManager.PlayerPlanetData.previewGamePassTier;
		if ((int)(gamePassTier + 1) < (int)tierRequirement && (int)(previewGamePassTier + 1) < (int)tierRequirement)
		{
			string key = "You need to unlock Tier {0} before you can unlock this Tier.";
			string format = TM._(key);
			string textBubbleText = string.Format(format, (int)(Tier - 1));
			lockedTierBubble.Activate(textBubbleText);
		}
		else
		{
			string key2 = "You need to unlock Tier {0} before you can play as this Class.";
			string format2 = TM._(key2);
			string textBubbleText2 = string.Format(format2, (int)Tier);
			lockedTierBubble.Activate(textBubbleText2);
		}
	}

	private int CalculateTotalSpawnRoleCost(int spawnRoleId)
	{
		MVAvatarSpawnRoleCreator mVAvatarSpawnRoleCreator = (MVAvatarSpawnRoleCreator)MVGameControllerBase.WOCM.GetWorldObject(spawnRoleId);
		AttributeSettingsManager attributeSettingsManagerAvatar = mVAvatarSpawnRoleCreator.AttributeSettingsManagerAvatar;
		KogamaSettingsCollectionBase kogamaSettingsCollectionBase = (KogamaSettingsCollectionBase)attributeSettingsManagerAvatar.Settings;
		if (kogamaSettingsCollectionBase == null)
		{
			return 0;
		}
		int num = 0;
		foreach (KeyValuePair<string, KogamaSettingWrapperBase> child in kogamaSettingsCollectionBase.Children)
		{
			num += ((IAttributeSetting)child.Value).AttributeValue;
		}
		return num;
	}

	private void HandlePlayButtonVisibility()
	{
		if (GamePassesManager.PlayerPlanetData == null)
		{
			playButton.SetActive(value: true);
			freeTryButton.SetActive(value: false);
			lockedButton.SetActive(value: false);
			return;
		}
		GamePassTier gamePassTier = GamePassesManager.PlayerPlanetData.gamePassTier;
		GamePassTier previewGamePassTier = GamePassesManager.PlayerPlanetData.previewGamePassTier;
		if ((int)tierRequirement <= (int)gamePassTier || (int)tierRequirement <= (int)previewGamePassTier)
		{
			playButton.SetActive(value: true);
			freeTryButton.SetActive(value: false);
			lockedButton.SetActive(value: false);
		}
		else if (tierRequirement == gamePassTier + 1)
		{
			playButton.SetActive(value: false);
			freeTryButton.SetActive(CanShowFreeTry());
			lockedButton.SetActive(!CanShowFreeTry());
		}
		else
		{
			playButton.SetActive(value: false);
			freeTryButton.SetActive(value: false);
			lockedButton.SetActive(value: true);
		}
	}

	private bool CanShowFreeTry()
	{
		bool flag = MVGameControllerBase.GameMode == MVGameMode.Edit;
		bool rewardedAdsEnabled = MVClientSettings.RewardedAdsEnabled;
		return !flag && rewardedAdsEnabled;
	}

	private void HandleTeamSwitching()
	{
		if (teamRequirement != MVGameControllerBase.Game.LocalPlayer.Team)
		{
			MVGameControllerBase.OperationRequests.SetTeam(teamRequirement);
			MVGameControllerBase.Game.GameStatCounterManager.RemoveTeamScoreOnActorLeave(MVGameControllerBase.Game.LocalPlayer.ActorNr, MVGameControllerBase.Game.LocalPlayer.Team);
			MVGameControllerBase.Game.LocalPlayer.ResetCheckpoint();
			MVGameControllerBase.Game.LocalPlayer.Team = teamRequirement;
		}
	}

	private void StartPlaying()
	{
		MVGameControllerDesktop.LockCursorManager.CursorLock = true;
		if (MVGameControllerBase.GameSessionData.gameMode != MVGameMode.Edit || (MVGameControllerBase.GameSessionData.gameMode == MVGameMode.Edit && MVGameControllerBase.EditModeUI.IsInPlayInEditMode))
		{
			MVGameControllerBase.GameEventManager.AvatarCommandsPlayMode.SetToSpawnPoint();
		}
	}

	private void Close(int spawnRoleID = 0)
	{
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack handler, BaseEventData data) =>
		{
			handler.Pop();
		});
		MVGameControllerBase.MainCameraManager.CamMaskMode = MaskMode.Default;
	}

	private void AwaitSpawnThenClose()
	{
		awaitingSpawn = true;
		MVGameControllerBase.LocalPlayer.SpawnRolesManager.OnSpawnRoleActivated += Close;
	}

	private void ShowAd()
	{
		if (GamePassesManager.TogglePreviewState.CanToggle)
		{
			MVGameControllerBase.AdManager.RequestRewardedAd(RewardedAdCallback, AdContext.PreviewTier);
			return;
		}
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IModalPopupCreator x, BaseEventData y) =>
		{
			x.Create(TM._("Free try cannot be activated at this moment."), TM._("An error occurred"));
		});
	}

	private void RewardedAdCallback(RewardedAdResult result)
	{
		switch (result)
		{
		case RewardedAdResult.RewardUnlocked:
			PreviewTier();
			break;
		case RewardedAdResult.RewardNotUnlocked:
			ExecuteEvents.ExecuteHierarchy(gameObject, null, (IModalPopupCreator x, BaseEventData y) =>
			{
				x.Create(TM._("The video was canceled. Your Free Try has not been activated."), TM._("Video canceled"));
			});
			break;
		case RewardedAdResult.ErrorClient:
		case RewardedAdResult.ErrorInternal:
			ExecuteEvents.ExecuteHierarchy(gameObject, null, (IModalPopupCreator x, BaseEventData y) =>
			{
				x.Create(MVGameControllerBase.AdManager.RewardedAdNotAvailableText, TM._("No Ad Available"));
			});
			break;
		case RewardedAdResult.ErrorTimeout:
			break;
		}
	}

	private void PreviewTier()
	{
		if (GamePassesManager.TogglePreviewState.CanToggle)
		{
			MVGameControllerBase.OperationRequests.TogglePreviewTier();
			GamePassesManager.OnPlayerPlanetDataUpdated = (Action)Delegate.Combine(GamePassesManager.OnPlayerPlanetDataUpdated, new Action(OnPlayerPlanetDataUpdated));
			ExecuteEvents.ExecuteHierarchy(gameObject, null, (IModalPopupCreator x, BaseEventData y) =>
			{
				x.Create();
			});
			isWaitingForFreeTryTier = true;
		}
		else
		{
			ExecuteEvents.ExecuteHierarchy(gameObject, null, (IModalPopupCreator x, BaseEventData y) =>
			{
				x.Create(TM._("Free try cannot be activated at this moment."), TM._("An error occurred"));
			});
		}
	}

	private void OnPlayerPlanetDataUpdated()
	{
		if (isWaitingForFreeTryTier)
		{
			ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
			{
				x.Pop();
			});
			isWaitingForFreeTryTier = false;
		}
		GamePassTier previewGamePassTier = GamePassesManager.PlayerPlanetData.previewGamePassTier;
		if ((int)previewGamePassTier >= (int)tierRequirement && !haveShownFreeTryUnlock)
		{
			GamePassesManager.OnPlayerPlanetDataUpdated = (Action)Delegate.Remove(GamePassesManager.OnPlayerPlanetDataUpdated, new Action(OnPlayerPlanetDataUpdated));
			ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
			{
				x.Pop();
			});
			SpawnRoleUnlockedPopupController spawnRoleUnlockPopup = UnityEngine.Object.Instantiate(spawnRoleUnlockPopupPrefab);
			ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
			{
				x.Push(spawnRoleUnlockPopup.gameObject, UIPushOption.InvisibleBlocker, null, UIGroupFlags.InventoryUI);
			});
			spawnRoleUnlockPopup.Initialize(tierRequirement, wasPurchased: false, wasTempUnlocked: true, WOID);
			haveShownFreeTryUnlock = true;
		}
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		if (awaitingSpawn)
		{
			MVGameControllerBase.LocalPlayer.SpawnRolesManager.OnSpawnRoleActivated -= Close;
		}
		ContinueButtonHandler continueButtonHandler = this.continueButtonHandler;
		continueButtonHandler.OnClick = (Action)Delegate.Remove(continueButtonHandler.OnClick, new Action(OnPressPlay));
	}

	public override void OnSelctionHighlight()
	{
		base.OnSelctionHighlight();
	}

	public override void OnSelected()
	{
		base.OnSelected();
		spawnRoleCostObject.SetActive(value: true);
		moreInfoButton.SetActive(value: true);
		playButtonFader.gameObject.SetActive(value: true);
		spawnRoleCostFader.Activate();
		moreInfoButtonFader.Activate();
		playButtonFader.Activate();
	}

	public override void OnUnSelected()
	{
		base.OnUnSelected();
		spawnRoleCostObject.SetActive(value: false);
		moreInfoButton.SetActive(value: false);
		playButtonFader.gameObject.SetActive(value: false);
	}
}
