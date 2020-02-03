using System;
using System.Collections.Generic;
using Assets.Scripts.AdIntegration;
using MV.Common;
using MV.WorldObject;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LobbyStateButton : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler, IEventSystemHandler
{
	[SerializeField]
	private TeamMenu teamMenuPrefab;

	[SerializeField]
	private WinningConditionBriefing winningConditionBriefingMenu;

	[SerializeField]
	private SpawnRoleMenu spawnRoleMenuPrefab;

	[SerializeField]
	private Image countdownFill;

	[SerializeField]
	private Button lobbyStateButton;

	[SerializeField]
	private LevelRewardsLobbyState levelRewards;

	[SerializeField]
	private ContinueButtonLockCursor continueScreenPrefab;

	[SerializeField]
	private Image playButtonImage;

	[SerializeField]
	private Sprite watchAdPlayButtonImageSprite;

	[SerializeField]
	private GamePassesTextBubble signupToRemoveAds;

	private bool shouldUpdateFillImage;

	private bool isMoveOverButton;

	private bool shouldPop;

	public bool ShouldPop
	{
		set
		{
			shouldPop = value;
		}
	}

	private bool IsRoundEnded => MVGameControllerBase.Game.NetworkGameStateListener.CurrentGameState == MVGameStateType.RoundEnded;

	private void Start()
	{
		if (!WinningConditionControl.TryGetPrioritizedWinCondition(out var _))
		{
			countdownFill.enabled = true;
			shouldUpdateFillImage = true;
		}
		else
		{
			countdownFill.enabled = false;
		}
		if (MVClientSettings.PlayButtonAdsEnabled && MVGameControllerBase.EditModeUI == null)
		{
			Debug.Log("Lobby state button interstitial.");
			countdownFill.enabled = false;
			shouldUpdateFillImage = false;
			playButtonImage.sprite = watchAdPlayButtonImageSprite;
		}
	}

	private void Update()
	{
		bool flag = MVGameControllerBase.Game.NetworkGameStateListener.CurrentGameState == MVGameStateType.RoundEnded;
		if (flag && shouldUpdateFillImage)
		{
			countdownFill.fillAmount = MVGameControllerBase.Game.NetworkGameStateListener.CountdownInPercentage;
			if (!countdownFill.gameObject.activeSelf)
			{
				countdownFill.gameObject.SetActive(value: true);
			}
		}
		else if (countdownFill.gameObject.activeSelf)
		{
			OnCountDownEnd();
			countdownFill.gameObject.SetActive(value: false);
		}
		if (!flag)
		{
			OnCountDownEnd();
		}
	}

	public void OnPointerDown(PointerEventData eventData)
	{
		if (eventData.button == PointerEventData.InputButton.Left && isMoveOverButton)
		{
			OnPressPlay();
		}
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		isMoveOverButton = true;
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		isMoveOverButton = false;
	}

	public void OnPressPlay()
	{
		levelRewards.ShowLevelNotification();
		bool hasGameWinningCondition = WinningConditionControl.TryGetPrioritizedWinCondition(out var winCon);
		List<MVWorldObjectClient> worldObjectsByType = MVGameControllerBase.Game.WorldObjectClientManager.GetWorldObjectsByType(WorldObjectType.AvatarSpawnRoleCreator);
		bool flag = worldObjectsByType.Count > 0;
		if (MVGameControllerBase.Game.TeamManager.TeamCount() > 1)
		{
			Action<InterstitialAdResult> callback = (InterstitialAdResult result) =>
			{
				if (IsRoundEnded && !hasGameWinningCondition)
				{
					lobbyStateButton.interactable = false;
				}
				else if (IsRoundEnded)
				{
					CreateTeamMenu();
				}
				else
				{
					CreateTeamMenu();
				}
			};
			RequestAdWithCallback(callback);
		}
		else if (hasGameWinningCondition)
		{
			Action<InterstitialAdResult> callback2 = (InterstitialAdResult result) =>
			{
				CreateBriefing(winCon);
			};
			RequestAdWithCallback(callback2);
		}
		else if (flag)
		{
			RequestAdWithCallback(OnShowAdFinishedSpawnRolesPresent);
		}
		else
		{
			RequestAdWithCallback(OnShowAdFinishedEnterPlaymode);
		}
	}

	private void RequestAdWithCallback(Action<InterstitialAdResult> callback)
	{
		if (MVClientSettings.PlayButtonAdsEnabled)
		{
			MVGameControllerBase.AdManager.RequestInterstitial(callback, AdContext.PlayButtonAd);
		}
		else
		{
			callback(InterstitialAdResult.Done);
		}
	}

	private void OnShowAdFinishedSpawnRolesPresent(InterstitialAdResult result)
	{
		if (IsRoundEnded)
		{
			lobbyStateButton.interactable = false;
		}
		else
		{
			CreateSpawnRoleSelectionMenu();
		}
	}

	private void OnShowAdFinishedEnterPlaymode(InterstitialAdResult result)
	{
		DoLockCursor();
	}

	private void PopThenLockCursor()
	{
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
		{
			x.Pop();
		});
		DoLockCursor();
	}

	private void DoLockCursor()
	{
		if (IsRoundEnded)
		{
			lobbyStateButton.interactable = false;
			LockCursor();
		}
		else
		{
			StartPlaying();
		}
	}

	public void CancelEnterPlay()
	{
		lobbyStateButton.interactable = true;
	}

	private void OnCountDownEnd()
	{
		if (!lobbyStateButton.interactable)
		{
			lobbyStateButton.interactable = true;
			List<MVWorldObjectClient> worldObjectsByType = MVGameControllerBase.Game.WorldObjectClientManager.GetWorldObjectsByType(WorldObjectType.AvatarSpawnRoleCreator);
			bool flag = worldObjectsByType.Count > 0;
			if (MVGameControllerBase.Game.TeamManager.TeamCount() > 1)
			{
				CreateTeamMenu();
			}
			else if (flag)
			{
				CreateSpawnRoleSelectionMenu();
			}
			else
			{
				StartPlaying();
			}
		}
	}

	private void StartPlaying()
	{
		if (!FirstTimePressPlayController.HaveBeenPressed)
		{
			FirstTimePressPlayController.OnFirstTimePlayIsPressed();
		}
		if (!MVGameControllerDesktop.LockCursorManager.CursorLock)
		{
			MVGameControllerDesktop.LockCursorManager.CursorLock = true;
		}
		else
		{
			MVGameControllerBase.PlayModeUI.InLobbyState = false;
		}
	}

	private void LockCursor()
	{
		MVGameControllerDesktop.LockCursorManager.CursorLockWithoutCallback = true;
	}

	private void OnEnable()
	{
		lobbyStateButton.interactable = true;
	}

	private void CreateTeamMenu()
	{
		if (shouldPop)
		{
			ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
			{
				x.Pop();
			});
		}
		TeamMenu newTeamMenu = UnityEngine.Object.Instantiate(teamMenuPrefab);
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
		{
			x.Push(newTeamMenu.gameObject, UIPushOption.HideAll | UIPushOption.InvisibleBlocker, null, UIGroupFlags.InventoryUI);
		});
	}

	private void CreateSpawnRoleSelectionMenu()
	{
		if (shouldPop)
		{
			ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
			{
				x.Pop();
			});
		}
		SpawnRoleMenu spawnRoleMenu = UnityEngine.Object.Instantiate(spawnRoleMenuPrefab);
		spawnRoleMenu.Initialize(MVGameControllerBase.LocalPlayer.Team);
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
		{
			x.Push(spawnRoleMenu.gameObject, UIPushOption.HideAll | UIPushOption.InvisibleBlocker, null, UIGroupFlags.InventoryUI);
		});
	}

	private void CreateBriefing(WinningConditionType winCon)
	{
		if (shouldPop)
		{
			ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
			{
				x.Pop();
			});
		}
		WinningConditionBriefing winConMenu = UnityEngine.Object.Instantiate(winningConditionBriefingMenu);
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
		{
			x.Push(winConMenu.gameObject, UIPushOption.HideAll | UIPushOption.InvisibleBlocker, null, UIGroupFlags.InventoryUI);
		});
		winConMenu.Initialize(winCon);
	}
}
