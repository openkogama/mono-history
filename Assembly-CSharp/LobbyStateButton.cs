using System.Collections.Generic;
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
		bool flag = MVGameControllerBase.Game.NetworkGameStateListener.CurrentGameState == MVGameStateType.RoundEnded;
		bool flag2 = WinningConditionControl.TryGetPrioritizedWinCondition(out var condition);
		List<MVWorldObjectClient> worldObjectsByType = MVGameControllerBase.Game.WorldObjectClientManager.GetWorldObjectsByType(WorldObjectType.AvatarSpawnRoleCreator);
		bool flag3 = worldObjectsByType.Count > 0;
		if (MVGameControllerBase.Game.TeamManager.TeamCount() > 1)
		{
			if (flag && !flag2)
			{
				lobbyStateButton.interactable = false;
			}
			else if (flag && flag2)
			{
				CreateTeamMenu();
			}
			else
			{
				CreateTeamMenu();
			}
		}
		else if (flag2)
		{
			CreateBriefing(condition);
		}
		else if (flag3)
		{
			if (flag)
			{
				lobbyStateButton.interactable = false;
			}
			else
			{
				CreateSpawnRoleSelectionMenu();
			}
		}
		else if (flag)
		{
			lobbyStateButton.interactable = false;
			LockCursor();
		}
		else
		{
			StartPlaying();
		}
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
		TeamMenu newTeamMenu = Object.Instantiate(teamMenuPrefab);
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
		SpawnRoleMenu spawnRoleMenu = Object.Instantiate(spawnRoleMenuPrefab);
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
		WinningConditionBriefing winConMenu = Object.Instantiate(winningConditionBriefingMenu);
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
		{
			x.Push(winConMenu.gameObject, UIPushOption.HideAll | UIPushOption.InvisibleBlocker, null, UIGroupFlags.InventoryUI);
		});
		winConMenu.Initialize(winCon);
	}
}
