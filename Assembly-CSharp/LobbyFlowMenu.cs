using System.Collections.Generic;
using MV.WorldObject;
using UnityEngine;
using UnityEngine.EventSystems;

public abstract class LobbyFlowMenu : MonoBehaviour
{
	protected enum LobbyFlowMenuType
	{
		LobbyState,
		Briefing,
		TeamSelect,
		SpawnRoleSelect,
		None
	}

	[SerializeField]
	protected MaskMode cameraMaskMode = MaskMode.SkyBoxOnly;

	[SerializeField]
	protected TeamMenu teamMenuPrefab;

	[SerializeField]
	protected WinningConditionBriefing winningConditionBriefingMenuPrefab;

	[SerializeField]
	protected SpawnRoleMenu spawnRoleMenuPrefab;

	private bool haveSetSelectedTeam;

	protected MVTeam selectedTeam;

	private List<LobbyFlowMenuType> menuOrder = new List<LobbyFlowMenuType>();

	protected abstract LobbyFlowMenuType MenuType { get; }

	protected MVTeam SelectedTeam
	{
		set
		{
			selectedTeam = value;
			haveSetSelectedTeam = true;
		}
	}

	public virtual void Start()
	{
		MVGameControllerBase.MainCameraManager.CamMaskMode = cameraMaskMode;
		if (!haveSetSelectedTeam)
		{
			selectedTeam = MVGameControllerBase.LocalPlayer.Team;
		}
		UpdateAvailableMenues();
	}

	protected virtual void OnDestroy()
	{
		if (MVGameControllerBase.IsAlive && MVGameControllerBase.PlayModeUI != null && !MVGameControllerBase.PlayModeUI.InLobbyState)
		{
			MVGameControllerBase.MainCameraManager.CamMaskMode = MaskMode.Default;
		}
	}

	protected void UpdateAvailableMenues()
	{
		menuOrder.Clear();
		menuOrder.Add(LobbyFlowMenuType.LobbyState);
		if (CanShowTeamSelect())
		{
			menuOrder.Add(LobbyFlowMenuType.TeamSelect);
		}
		if (CanShowBreifing())
		{
			menuOrder.Add(LobbyFlowMenuType.Briefing);
		}
		if (CanShowSpawnRoleSelect())
		{
			menuOrder.Add(LobbyFlowMenuType.SpawnRoleSelect);
		}
	}

	protected virtual bool CanShowTeamSelect()
	{
		return MVGameControllerBase.Game.TeamManager.TeamCount() > 1;
	}

	protected virtual bool CanShowBreifing()
	{
		WinningConditionType condition;
		return WinningConditionControl.TryGetPrioritizedWinCondition(out condition);
	}

	protected virtual bool CanShowSpawnRoleSelect()
	{
		return MVGameControllerBase.Game.TeamManager.TeamHasSpawnRoles(MVGameControllerBase.LocalPlayer.Team);
	}

	public void GoToNextMenu()
	{
		if (CanGoToNextMenu())
		{
			GoToMenu(GetNextMenuType());
		}
		else
		{
			StartPlaying();
		}
	}

	public void GoToPreviousMenu()
	{
		GoToMenu(GetPreviousMenuType());
	}

	protected bool CanGoToNextMenu()
	{
		bool flag = false;
		bool result = false;
		for (int i = 0; i < menuOrder.Count; i++)
		{
			if (flag)
			{
				result = true;
			}
			if (menuOrder[i] == MenuType)
			{
				flag = true;
			}
		}
		return result;
	}

	protected LobbyFlowMenuType GetNextMenuType()
	{
		bool flag = false;
		for (int i = 0; i < menuOrder.Count; i++)
		{
			if (flag)
			{
				return menuOrder[i];
			}
			if (menuOrder[i] == MenuType)
			{
				flag = true;
			}
		}
		return LobbyFlowMenuType.None;
	}

	protected LobbyFlowMenuType GetPreviousMenuType()
	{
		bool flag = false;
		for (int num = menuOrder.Count - 1; num >= 0; num--)
		{
			if (flag)
			{
				return menuOrder[num];
			}
			if (menuOrder[num] == MenuType)
			{
				flag = true;
			}
		}
		return LobbyFlowMenuType.None;
	}

	protected void GoToMenu(LobbyFlowMenuType newMenuType)
	{
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
		{
			x.Pop();
		});
		switch (newMenuType)
		{
		case LobbyFlowMenuType.LobbyState:
			break;
		case LobbyFlowMenuType.TeamSelect:
		{
			TeamMenu newTeamMenu = Object.Instantiate(teamMenuPrefab);
			ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
			{
				x.Push(newTeamMenu.gameObject, UIPushOption.HideAll | UIPushOption.InvisibleBlocker, null, UIGroupFlags.InventoryUI);
			});
			newTeamMenu.UpdateBackButtonVisibility();
			break;
		}
		case LobbyFlowMenuType.Briefing:
		{
			WinningConditionControl.TryGetPrioritizedWinCondition(out var condition);
			WinningConditionBriefing winConMenu = Object.Instantiate(winningConditionBriefingMenuPrefab);
			ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
			{
				x.Push(winConMenu.gameObject, UIPushOption.HideAll | UIPushOption.InvisibleBlocker, null, UIGroupFlags.InventoryUI);
			});
			winConMenu.Initialize(condition);
			winConMenu.SelectedTeam = selectedTeam;
			break;
		}
		case LobbyFlowMenuType.SpawnRoleSelect:
		{
			SpawnRoleMenu spawnRoleMenu = Object.Instantiate(spawnRoleMenuPrefab);
			spawnRoleMenu.Initialize(selectedTeam);
			ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
			{
				x.Push(spawnRoleMenu.gameObject, UIPushOption.HideAll | UIPushOption.InvisibleBlocker, null, UIGroupFlags.InventoryUI);
			});
			break;
		}
		}
	}

	protected virtual void StartPlaying()
	{
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
		{
			x.Pop();
		});
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
}
