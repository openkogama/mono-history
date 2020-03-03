using System;
using System.Collections.Generic;
using Assets.Scripts.AdIntegration;
using MV.Common;
using MV.WorldObject;
using MV.WorldObject.KogamaSettings.KogamaSettingsCore.KogamaSettingTypes;
using MV.WorldObject.KogamaSettings.SpecializedSettingsTypes.AttributeSettings.AttributeSettingTypes;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GamePassesSpawnRoleRewardInfo : MonoBehaviour, IGamePassShopContent
{
	[SerializeField]
	private Image spawnRoleTeamImage;

	[SerializeField]
	private RawImage spawnRoleImage;

	[SerializeField]
	private Text spawnRoleCostAmount;

	[SerializeField]
	private GameObject spawnRoleEditButton;

	[SerializeField]
	private GameObject lockedUI;

	[SerializeField]
	private GameObject unlockedUI;

	[SerializeField]
	private GameObject freeTryUI;

	[SerializeField]
	private ContinueButtonHandler continueButtonHandler;

	[SerializeField]
	private GamePassesTextBubble lockedTipTextBubble;

	[SerializeField]
	private SpawnRoleEditorMenu spawnRoleEditorMenuPrefab;

	[SerializeField]
	private SpawnRolePreviewer spawnRolePreviewerPrefab;

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

	[SerializeField]
	private int previewWidth;

	[SerializeField]
	private int previewHeight;

	[SerializeField]
	private Image buttonAdImage;

	private SpawnRolePreviewer spawnRolePreviewer;

	private int spawnRoleIndex;

	private MVAvatarSpawnRoleCreator spawnRole;

	private int woid;

	private GamePassTier tierRequirment;

	private MVTeam team;

	private GameObject spawnRolePreviewObject;

	private bool awaitingSpawn;

	private bool enterPlayWhenPlayerCanSpawn;

	private bool isWaitingForFreeTryTier;

	private bool haveShownFreeTryUnlock = true;

	private void OnDestroy()
	{
		MVAvatarSpawnRoleCreator mVAvatarSpawnRoleCreator = spawnRole;
		mVAvatarSpawnRoleCreator.OnBodyUpdate = (Action)Delegate.Remove(mVAvatarSpawnRoleCreator.OnBodyUpdate, new Action(OnSpawnRoleBodyUpdate));
		GamePassesManager.OnPlayerPlanetDataUpdated = (Action)Delegate.Remove(GamePassesManager.OnPlayerPlanetDataUpdated, new Action(OnPlayerPlanetDataUpdated));
		ContinueButtonHandler continueButtonHandler = this.continueButtonHandler;
		continueButtonHandler.OnClick = (Action)Delegate.Remove(continueButtonHandler.OnClick, new Action(OnPlayPressed));
		if (awaitingSpawn)
		{
			MVGameControllerBase.LocalPlayer.SpawnRolesManager.OnSpawnRoleActivated -= Close;
		}
		if (spawnRolePreviewer != null)
		{
			UnityEngine.Object.Destroy(spawnRolePreviewer);
		}
	}

	public void Initialize(int spawnRoleIndex, GameObject spawnRolePreviewObject, MVAvatarSpawnRoleCreator spawnRole, GamePassTier tierRequirment)
	{
		this.spawnRoleIndex = spawnRoleIndex;
		this.spawnRole = spawnRole;
		this.tierRequirment = tierRequirment;
		team = spawnRole.Team;
		this.spawnRolePreviewObject = spawnRolePreviewObject;
		woid = spawnRole.Id;
		spawnRoleTeamImage.color = GetTeamRequirementColor(spawnRole.Team);
		ChangeBackground(tierRequirment);
		SetupPreviewImage(spawnRolePreviewObject);
		if (MVGameControllerBase.GameSessionData.gameMode != MVGameMode.Edit || MVGameControllerBase.EditModeUI.IsInPlayInEditMode)
		{
			spawnRoleEditButton.SetActive(value: false);
		}
		int skillCost = CalculateSpawnRoleCost();
		spawnRoleCostAmount.text = skillCost.ToString();
		spawnRoleCostAmount.color = SpawnRolesSkillDataManager.GetCostColor(skillCost);
		HandleLockedUIVisibility((int)tierRequirment <= (int)GamePassesManager.PlayerPlanetData.gamePassTier || (int)tierRequirment <= (int)GamePassesManager.PlayerPlanetData.previewGamePassTier, tierRequirment == GamePassesManager.PlayerPlanetData.gamePassTier + 1);
		spawnRole.OnBodyUpdate = (Action)Delegate.Combine(spawnRole.OnBodyUpdate, new Action(OnSpawnRoleBodyUpdate));
		GamePassesManager.OnPlayerPlanetDataUpdated = (Action)Delegate.Combine(GamePassesManager.OnPlayerPlanetDataUpdated, new Action(OnPlayerPlanetDataUpdated));
		ContinueButtonHandler continueButtonHandler = this.continueButtonHandler;
		continueButtonHandler.OnClick = (Action)Delegate.Combine(continueButtonHandler.OnClick, new Action(OnPlayPressed));
		buttonAdImage.enabled = !GamePassesManager.TogglePreviewState.FreeTryWithoutAdAvailable;
	}

	public void OnPressed()
	{
		SpawnRoleSelectionSkillMenu skillMenu = UnityEngine.Object.Instantiate(skillMenuPrefab);
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
		{
			x.Push(skillMenu.gameObject, UIPushOption.InvisibleBlocker, null, UIGroupFlags.InventoryUI);
		});
		skillMenu.Initialize(woid, tierRequirment, spawnRolePreviewObject);
		Debug.Log("LOOK WOID " + woid);
	}

	public void OnEditPressed()
	{
		if (MVGameControllerBase.GameSessionData.gameMode == MVGameMode.Edit && !MVGameControllerBase.EditModeUI.IsInPlayInEditMode)
		{
			SpawnRoleEditorMenu spawnRoleMenu = UnityEngine.Object.Instantiate(spawnRoleEditorMenuPrefab);
			spawnRoleMenu.Initialize(spawnRole.Id);
			ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
			{
				x.Push(spawnRoleMenu.gameObject, UIPushOption.HideAll | UIPushOption.InvisibleBlocker, null, UIGroupFlags.InventoryUI);
			});
		}
	}

	public void OnLockedPressed()
	{
		GamePassTier gamePassTier = GamePassesManager.PlayerPlanetData.gamePassTier;
		GamePassTier previewGamePassTier = GamePassesManager.PlayerPlanetData.previewGamePassTier;
		if ((int)(gamePassTier + 1) < (int)tierRequirment && (int)(previewGamePassTier + 1) < (int)tierRequirment)
		{
			string key = "You need to unlock Tier {0} before you can unlock this Tier.";
			string format = TM._(key);
			string textBubbleText = string.Format(format, (int)(tierRequirment - 1));
			lockedTipTextBubble.Activate(textBubbleText);
		}
		else
		{
			string key2 = "You need to unlock Tier {0} before you can play as this Class.";
			string format2 = TM._(key2);
			string textBubbleText2 = string.Format(format2, (int)tierRequirment);
			lockedTipTextBubble.Activate(textBubbleText2);
		}
	}

	public void OnPressFreePlay()
	{
		ShowAd();
	}

	public void OnPlayPressed()
	{
		if (MVGameControllerBase.Game.TeamManager.TeamCount() > 1)
		{
			HandleTeamSwitching();
		}
		if (CanSpawn())
		{
			int id = spawnRole.Id;
			bool flag = id == MVGameControllerBase.LocalPlayer.SpawnRoleDataMediator.WoId;
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
				MVGameControllerBase.Game.LocalPlayer.CreateSpawnRole(id);
				MVGameControllerDesktop.LockCursorManager.CursorLockWithoutCallback = true;
			}
		}
		else
		{
			ExecuteEvents.ExecuteHierarchy(gameObject, null, (IModalPopupCreator x, BaseEventData y) =>
			{
				x.Create();
			});
			enterPlayWhenPlayerCanSpawn = true;
		}
	}

	private void AwaitSpawnThenClose()
	{
		awaitingSpawn = true;
		MVGameControllerBase.LocalPlayer.SpawnRolesManager.OnSpawnRoleActivated += Close;
	}

	private void Close(int spawnRoleID = 0)
	{
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack handler, BaseEventData data) =>
		{
			handler.PopGroups(UIGroupFlags.InventoryUI);
		});
		MVGameControllerBase.MainCameraManager.CamMaskMode = MaskMode.Default;
	}

	private void HandleTeamSwitching()
	{
		if (team != MVGameControllerBase.Game.LocalPlayer.Team)
		{
			MVGameControllerBase.OperationRequests.SetTeam(team);
			MVGameControllerBase.Game.GameStatCounterManager.RemoveTeamScoreOnActorLeave(MVGameControllerBase.Game.LocalPlayer.ActorNr, MVGameControllerBase.Game.LocalPlayer.Team);
			MVGameControllerBase.Game.LocalPlayer.ResetCheckpoint();
			MVGameControllerBase.Game.LocalPlayer.Team = team;
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

	public void Activate()
	{
		spawnRolePreviewer.ActivatePreview();
	}

	public void Deactivate()
	{
		spawnRolePreviewer.DeactivatePreview();
	}

	private void Update()
	{
		if (enterPlayWhenPlayerCanSpawn && CanSpawn())
		{
			ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
			{
				x.Pop();
			});
			enterPlayWhenPlayerCanSpawn = false;
			OnPlayPressed();
		}
	}

	private Color GetTeamRequirementColor(MVTeam team)
	{
		return team switch
		{
			MVTeam.Blue => Styles.GetColor(ColorStyle.TeamBlue), 
			MVTeam.Red => Styles.GetColor(ColorStyle.TeamRed), 
			MVTeam.Green => Styles.GetColor(ColorStyle.TeamGreen), 
			MVTeam.Yellow => Styles.GetColor(ColorStyle.TeamYellow), 
			_ => Styles.GetColor(ColorStyle.TeamNone), 
		};
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

	private void SetupPreviewImage(GameObject spawnRolePreviewObject)
	{
		if (spawnRolePreviewer != null)
		{
			UnityEngine.Object.Destroy(spawnRolePreviewer);
		}
		spawnRolePreviewer = UnityEngine.Object.Instantiate(spawnRolePreviewerPrefab);
		GameObject gameObject = UnityEngine.Object.Instantiate(spawnRolePreviewObject);
		gameObject.transform.localRotation = Quaternion.identity;
		Transform previewSpawnRoleRoot = new GameObject("Preview Root - TierShopItem").transform;
		Vector3 previewPosition = new Vector3(500f, 500f, 10f * (float)spawnRoleIndex);
		Vector3 cameraOffset = new Vector3(0f, 1.5f, -6f);
		spawnRolePreviewer.Initialize(previewWidth, previewHeight, CameraClearFlags.Color, LayerFlags.Default | LayerFlags.CamRotateTarget, cameraOffset, previewSpawnRoleRoot, previewPosition, "SpawnRole", spawnRoleIndex, gameObject);
		spawnRoleImage.texture = spawnRolePreviewer.PreviewTexture;
	}

	private void OnSpawnRoleBodyUpdate()
	{
		SetupPreviewImage(spawnRole.GetSpawnRolePreviewObject());
	}

	private int CalculateSpawnRoleCost()
	{
		int num = 0;
		KogamaSettingsCollectionBase kogamaSettingsCollectionBase = (KogamaSettingsCollectionBase)spawnRole.AttributeSettingsManagerAvatar.Settings;
		if (kogamaSettingsCollectionBase == null)
		{
			return num;
		}
		foreach (KeyValuePair<string, KogamaSettingWrapperBase> child in kogamaSettingsCollectionBase.Children)
		{
			num += ((IAttributeSetting)child.Value).AttributeValue;
		}
		return num;
	}

	private void HandleLockedUIVisibility(bool playerHasUnlockedTier, bool isTierUnlockable)
	{
		bool flag = MVGameControllerBase.GameSessionData.gameMode == MVGameMode.Edit && !MVGameControllerBase.EditModeUI.IsInPlayInEditMode;
		lockedUI.SetActive((!playerHasUnlockedTier && (!isTierUnlockable || !CanShowFreeTry())) || flag);
		freeTryUI.SetActive(!playerHasUnlockedTier && isTierUnlockable && !flag && CanShowFreeTry());
		unlockedUI.SetActive(playerHasUnlockedTier && !flag);
	}

	private void ShowAd()
	{
		if (GamePassesManager.TogglePreviewState.CanToggle)
		{
			if (GamePassesManager.TogglePreviewState.FreeTryWithoutAdAvailable)
			{
				PreviewTier();
				GamePassesManager.TogglePreviewState.FreeTryWithoutAdAvailable = false;
			}
			else
			{
				MVGameControllerBase.AdManager.RequestRewardedAd(RewardedAdCallback, AdContext.PreviewTier);
			}
		}
		else
		{
			ExecuteEvents.ExecuteHierarchy(gameObject, null, (IModalPopupCreator x, BaseEventData y) =>
			{
				x.Create(TM._("Free try cannot be activated at this moment."), TM._("An error occurred"));
			});
		}
	}

	private void RewardedAdCallback(RewardedAdResult result)
	{
		if (MVGameControllerBase.EditModeUI != null)
		{
			result = RewardedAdResult.RewardUnlocked;
		}
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
			haveShownFreeTryUnlock = false;
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
		HandleLockedUIVisibility((int)tierRequirment <= (int)GamePassesManager.PlayerPlanetData.gamePassTier || (int)tierRequirment <= (int)GamePassesManager.PlayerPlanetData.previewGamePassTier, tierRequirment == GamePassesManager.PlayerPlanetData.gamePassTier + 1);
		if (isWaitingForFreeTryTier)
		{
			ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
			{
				x.Pop();
			});
			isWaitingForFreeTryTier = false;
		}
		GamePassTier previewGamePassTier = GamePassesManager.PlayerPlanetData.previewGamePassTier;
		if ((int)previewGamePassTier >= (int)tierRequirment && !haveShownFreeTryUnlock)
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
			spawnRoleUnlockPopup.Initialize(tierRequirment, wasPurchased: false, wasTempUnlocked: true, woid);
			Debug.Log("PLAY WOID " + woid);
			haveShownFreeTryUnlock = true;
		}
	}

	private bool CanSpawn()
	{
		bool flag = MVGameControllerBase.Game.NetworkGameStateListener.CurrentGameState == MVGameStateType.RoundEnded;
		bool flag2 = Time.time < MVGameControllerBase.LocalPlayer.RespawnTime;
		return !flag && !flag2;
	}

	private bool CanShowFreeTry()
	{
		bool flag = MVGameControllerBase.GameMode == MVGameMode.Edit;
		bool rewardedAdsEnabled = MVClientSettings.RewardedAdsEnabled;
		return !flag && rewardedAdsEnabled;
	}
}
