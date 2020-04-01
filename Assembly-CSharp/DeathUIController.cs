using System;
using System.Collections.Generic;
using MV.Common;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DeathUIController : MonoBehaviour
{
	[SerializeField]
	private Text deathReason;

	[SerializeField]
	private NotificationFade fader;

	[SerializeField]
	private TierUnlockedPopupController tierUnlockedPopupControllerPrefab;

	[SerializeField]
	private DeathUIBoostMenuController boostMenuPrefab;

	[SerializeField]
	private ReviveUIHandler reviveHandler;

	[SerializeField]
	private ReviveUIHandlerBoosts reviveHandlerBoosts;

	[SerializeField]
	private TierOnDeathProgress tierOnDeathProgress;

	[SerializeField]
	private GameObject claimGoldRewardPopupPrefab;

	[SerializeField]
	private TierBoostStateHandler tierHandler;

	[SerializeField]
	private GameObject invisibleBlocker;

	[SerializeField]
	private GameObject deathMessageBar;

	private float waitTime;

	private bool isDeathBriefActive;

	private void Awake()
	{
		gameObject.SetActive(value: false);
		Initialize();
	}

	private void Initialize()
	{
		MVGameControllerBase.SpawnRoleDataMediatorLocal.OnKilled += OnLocalPlayerKilled;
		MVGameControllerBase.SpawnRoleDataMediatorLocal.SpawnRoleModeTypeWrapper.OnChange += OnAvatarStateChanged;
		FlagDebriefingControl flagDebriefingControl = MVGameControllerBase.FlagDebriefingControl;
		flagDebriefingControl.OnFlagDebriefingEnd = (Action)Delegate.Combine(flagDebriefingControl.OnFlagDebriefingEnd, new Action(EndDeathBriefing));
		MVNetworkGame game = MVGameControllerBase.Game;
		game.OnWinningConditionFulfilled = (Action<IWinningCondition>)Delegate.Combine(game.OnWinningConditionFulfilled, new Action<IWinningCondition>(OnRoundEnd));
		MVGameControllerBase.SpawnRoleDataMediatorLocal.OnSuicide += OnLocalAvatarSuicide;
		NotificationFade notificationFade = fader;
		notificationFade.OnFinished = (Action)Delegate.Combine(notificationFade.OnFinished, new Action(OnFadeFinished));
	}

	private void OnAvatarStateChanged(SpawnRoleModeType mode)
	{
		if (isDeathBriefActive && mode != SpawnRoleModeType.Hidden && mode != SpawnRoleModeType.Dead)
		{
			EndDeathBriefing();
		}
	}

	private void EndDeathBriefing()
	{
		fader.Deactivate();
		fader.gameObject.SetActive(value: false);
		gameObject.SetActive(value: false);
		isDeathBriefActive = false;
	}

	private void Update()
	{
		if (!(waitTime < Time.time))
		{
			return;
		}
		if (!isDeathBriefActive && MVGameControllerBase.Game.NetworkGameStateListener.CurrentGameState != MVGameStateType.RoundEnded)
		{
			isDeathBriefActive = true;
			TierProgress();
			fader.gameObject.SetActive(value: true);
			fader.Activate();
		}
		if (tierOnDeathProgress.gameObject.activeInHierarchy)
		{
			if (tierOnDeathProgress.IsShowingTierProgress)
			{
				fader.PauseAt(0.99f);
			}
			else if (fader.IsPaused)
			{
				fader.Unpause();
			}
		}
	}

	private void TierProgress()
	{
		if (!GamePassesManager.GamePassesActive)
		{
			tierOnDeathProgress.gameObject.SetActive(value: false);
			return;
		}
		GamePassTier gamePassTier = GamePassesManager.PlayerPlanetData.gamePassTier;
		bool flag = (TierUnlockedPopupController.HighestTierRewardShown != gamePassTier || gamePassTier != GamePassTier.Tier3) && GamePassesManager.playerTierStateCalculator.gamePassRewardsActivated;
		tierOnDeathProgress.gameObject.SetActive(flag);
		if (flag)
		{
			tierOnDeathProgress.Initialize();
		}
	}

	private void OnDestroy()
	{
		if (MVGameControllerBase.IsAlive)
		{
			MVGameControllerBase.SpawnRoleDataMediatorLocal.OnSuicide -= OnLocalAvatarSuicide;
			MVGameControllerBase.SpawnRoleDataMediatorLocal.OnKilled -= OnLocalPlayerKilled;
			MVGameControllerBase.SpawnRoleDataMediatorLocal.SpawnRoleModeTypeWrapper.OnChange -= OnAvatarStateChanged;
			FlagDebriefingControl flagDebriefingControl = MVGameControllerBase.FlagDebriefingControl;
			flagDebriefingControl.OnFlagDebriefingEnd = (Action)Delegate.Remove(flagDebriefingControl.OnFlagDebriefingEnd, new Action(EndDeathBriefing));
			MVNetworkGame game = MVGameControllerBase.Game;
			game.OnWinningConditionFulfilled = (Action<IWinningCondition>)Delegate.Remove(game.OnWinningConditionFulfilled, new Action<IWinningCondition>(OnRoundEnd));
			NotificationFade notificationFade = fader;
			notificationFade.OnFinished = (Action)Delegate.Remove(notificationFade.OnFinished, new Action(OnFadeFinished));
		}
	}

	private void ShowReviveMenu(bool reboostOnly)
	{
		if (reboostOnly)
		{
			ReviveUIHandlerBoosts revivePopup = UnityEngine.Object.Instantiate(reviveHandlerBoosts);
			ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
			{
				x.Push(revivePopup.gameObject, UIPushOption.HideAll, null, UIGroupFlags.GameObjectUI);
			});
			revivePopup.Initialize(ReboostNotClicked);
			return;
		}
		ReviveUIHandler revivePopup2 = UnityEngine.Object.Instantiate(reviveHandler);
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
		{
			x.Push(revivePopup2.gameObject, UIPushOption.HideAll, () =>
			{
				StatHatWrapper.Count("Revive.Closed", 1);
			}, UIGroupFlags.GameObjectUI);
		});
		revivePopup2.Initialize(ReviveNotClicked);
	}

	private void ReboostNotClicked()
	{
		if (MVGameControllerBase.SpawnRoleDataMediatorLocal.SpawnRoleMode.Value == SpawnRoleModeType.Dead)
		{
			MVGameControllerBase.GameEventManager.AvatarCommandsPlayMode.SetToDeadMode();
		}
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
		{
			x.Pop();
		});
		MVGameControllerBase.Game.LocalPlayer.BoostController.RemoveAllBoosts();
		if (GamePassesManager.GamePassesActive && tierHandler.IsInTempTier())
		{
			tierHandler.StopPreviewTier(OnFinishPreviewTier);
		}
		else
		{
			ShowBoostMenu();
		}
	}

	private void ReviveNotClicked()
	{
		if (MVClientSettings.ReviveEnabled && MVGameControllerBase.SpawnRoleDataMediatorLocal.SpawnRoleMode.Value == SpawnRoleModeType.Dead)
		{
			MVGameControllerBase.GameEventManager.AvatarCommandsPlayMode.SetToDeadMode();
		}
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
		{
			x.Pop();
		});
		MVGameControllerBase.Game.LocalPlayer.BoostController.RemoveAllBoosts();
		tierHandler.StopPreviewTier(OnFinishPreviewTier);
	}

	private void NotReviving()
	{
		MVGameControllerBase.Game.LocalPlayer.BoostController.RemoveAllBoosts();
		tierHandler.StopPreviewTier(OnFinishPreviewTier);
	}

	private void OnFinishPreviewTier(bool openBoostMenu)
	{
		if (openBoostMenu)
		{
			ShowBoostMenu();
		}
	}

	private void OnDisable()
	{
		Debug.Log("Disabled");
		fader.PauseAt(0f);
		fader.Deactivate();
		fader.gameObject.SetActive(value: false);
		isDeathBriefActive = false;
	}

	private void ShowBoostMenu()
	{
		DeathUIBoostMenuController boostMenu = UnityEngine.Object.Instantiate(boostMenuPrefab);
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
		{
			x.Push(boostMenu.gameObject, UIPushOption.HideAll, null, UIGroupFlags.GameObjectUI);
		});
		boostMenu.Initialize();
	}

	private void OnFadeFinished()
	{
		if (fader.gameObject.activeSelf)
		{
			fader.gameObject.SetActive(value: false);
			isDeathBriefActive = false;
			gameObject.SetActive(value: false);
			Debug.Log("Fade finished");
			if (!MVGameControllerBase.PlayModeUI.InLobbyState && GamePassesManager.GamePassesActive && ShouldShowTierReward(GamePassesManager.PlayerPlanetData.gamePassTier))
			{
				ShowTierUnlockedPopup(wasPurchased: false, wasTempUnlocked: false);
			}
			else if (!ShowingClaimGold())
			{
				ShowDeadmodeUI();
			}
		}
	}

	private void ShowTierUnlockedPopup(bool wasPurchased, bool wasTempUnlocked)
	{
		TierUnlockedPopupController tierUnlockedPopupController = UnityEngine.Object.Instantiate(tierUnlockedPopupControllerPrefab);
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
		{
			x.Push(tierUnlockedPopupController.gameObject, UIPushOption.InvisibleBlocker, OnTierUnlockedPop, UIGroupFlags.InventoryUI);
		});
		tierUnlockedPopupController.Initialize(GamePassesManager.PlayerPlanetData.gamePassTier, wasPurchased, wasTempUnlocked);
	}

	private void OnTierUnlockedPop()
	{
		if (!ShowingClaimGold())
		{
			ShowDeadmodeUI();
		}
	}

	private bool ShowingClaimGold()
	{
		if (MVGameControllerBase.GoldRewardManager.CanGetGoldReward() && MVGameControllerBase.GoldRewardManager.GetGoldRewardTimeLeft() <= 0f)
		{
			GameObject claimGoldRewardPopup = UnityEngine.Object.Instantiate(claimGoldRewardPopupPrefab);
			ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
			{
				x.Push(claimGoldRewardPopup.gameObject, UIPushOption.InvisibleBlocker, ShowDeadmodeUI, UIGroupFlags.InventoryUI);
			});
			return true;
		}
		return false;
	}

	private void ShowDeadmodeUI()
	{
		Debug.Log("ShowDeadmodeUI");
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
		{
			x.Pop();
		});
		bool canSafelySpawn = MVGameControllerBase.SpawnRoleDataMediatorLocal.ReviveState.Value.CanSafelySpawn;
		bool flag = ((GamePassesManager.GamePassesActive && tierHandler.IsInTempTier()) || MVGameControllerBase.LocalPlayer.BoostController.GetActiveBoosts().Count > 0) && !canSafelySpawn;
		if (MVClientSettings.RewardedAdsEnabled && ((MVClientSettings.ReviveEnabled && canSafelySpawn) || flag))
		{
			ShowReviveMenu(flag);
		}
		else
		{
			NotReviving();
		}
	}

	private bool ShouldShowTierReward(GamePassTier tierToShow)
	{
		if (MVGameControllerBase.GameSessionData.gameMode == MVGameMode.Edit)
		{
			return false;
		}
		return (int)TierUnlockedPopupController.HighestTierRewardShown < (int)GamePassesManager.PlayerPlanetData.gamePassTier && (int)TierUnlockedPopupController.HighestTierRewardShown < (int)tierToShow && (int)tierToShow <= (int)GamePassesManager.PlayerPlanetData.gamePassTier;
	}

	private void OnLocalAvatarSuicide()
	{
		if (!MVGameControllerBase.FlagDebriefingControl.IsInFlagDebriefing)
		{
			Debug.Log("LocalSuicide");
			gameObject.SetActive(value: true);
			string deathText = TM._("Respawning..");
			deathMessageBar.SetActive(value: false);
			StartDeathBriefing(deathText);
		}
	}

	private void OnLocalPlayerKilled(int localPlayerActorNr, int dmgDealerActorNr, PlayerKilledByType damageType)
	{
		bool shotSelf = localPlayerActorNr == dmgDealerActorNr;
		Color color;
		Color color2;
		if (MVGameControllerBase.Game.TeamManager.GetTeamList().Count > 1)
		{
			color = Styles.GetTeamColor(MVGameControllerBase.Game.MVPlayerContainer[localPlayerActorNr].Team);
			color2 = Styles.GetTeamColor(MVGameControllerBase.Game.MVPlayerContainer[dmgDealerActorNr].Team);
		}
		else
		{
			color = Styles.GetColor(ColorStyle.Gray);
			color2 = Styles.GetColor(ColorStyle.Gray);
		}
		string userName = MVGameControllerBase.Game.MVPlayerContainer[localPlayerActorNr].UserProfileData.UserName;
		string userName2 = MVGameControllerBase.Game.MVPlayerContainer[dmgDealerActorNr].UserProfileData.UserName;
		string deathText = string.Format(KillNotification.GetKillText(damageType, shotSelf), Styles.ColorToHex(color), userName, Styles.ColorToHex(color2), userName2);
		if (!MVGameControllerBase.FlagDebriefingControl.IsInFlagDebriefing)
		{
			deathMessageBar.SetActive(value: true);
			StartDeathBriefing(deathText);
		}
	}

	private void SendCurrentProgressNotification()
	{
		WinningConditionControl.TryGetPrioritizedStat(out var statType);
		if (statType != GameStatCounterType.None)
		{
			Dictionary<object, object> data = new Dictionary<object, object>();
			NotificationController.PushNotification(NotificationType.CurrentProgress, data);
		}
	}

	private void StartDeathBriefing(string deathText)
	{
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
		{
			x.Push(UnityEngine.Object.Instantiate(invisibleBlocker), UIPushOption.InvisibleBlocker | UIPushOption.SuppressInput, null, UIGroupFlags.Popup);
		});
		Debug.Log("StartDeathBriefing");
		if (!MVGameControllerBase.PlayModeUI.InLobbyState)
		{
			waitTime = Time.time;
			gameObject.SetActive(value: true);
			deathReason.text = deathText;
		}
		else
		{
			ShowDeadmodeUI();
		}
	}

	private void OnRoundEnd(IWinningCondition winningCondition)
	{
		EndDeathBriefing();
	}

	public void OnPressPlay()
	{
		MVGameControllerDesktop.LockCursorManager.CursorLock = true;
		MVGameControllerBase.GameEventManager.AvatarCommandsPlayMode.SetRespawnWhenPossible();
	}
}
