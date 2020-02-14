using System;
using System.Collections.Generic;
using MV.Common;
using MV.WorldObject;
using UnityEngine;
using UnityEngine.EventSystems;

public class TierBoostStateHandler : MonoBehaviour
{
	[SerializeField]
	private TeamMenu teamMenuPrefab;

	[SerializeField]
	private SpawnRoleMenu spawnRoleSelectionMenuPrefab;

	private Action<bool> onFinished;

	public void StopPreviewTier(Action<bool> onFinishPreviewTier)
	{
		onFinished = onFinishPreviewTier;
		if (GamePassesManager.GamePassesActive && (IsInTempClass() || IsInTempTier()))
		{
			GamePassesManager.OnPlayerPlanetDataUpdated = (Action)Delegate.Combine(GamePassesManager.OnPlayerPlanetDataUpdated, new Action(OnPlayerPlanetDataUpdated));
			ExecuteEvents.ExecuteHierarchy(gameObject, null, (IModalPopupCreator x, BaseEventData y) =>
			{
				x.Create();
			});
			MVGameControllerBase.OperationRequests.TogglePreviewTier();
		}
		else
		{
			onFinishPreviewTier(obj: true);
		}
	}

	private void OnPlayerPlanetDataUpdated()
	{
		GamePassesManager.OnPlayerPlanetDataUpdated = (Action)Delegate.Remove(GamePassesManager.OnPlayerPlanetDataUpdated, new Action(OnPlayerPlanetDataUpdated));
		ExitContinuePopup();
	}

	private void ExitContinuePopup()
	{
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
		{
			x.Pop();
		});
		bool flag = IsInTempClass();
		if (flag)
		{
			if (CanSpawnInTeam(MVGameControllerBase.LocalPlayer.Team))
			{
				ShowSpawnRoleSelectionMenu();
			}
			else
			{
				ShowTeamSelectionMenu();
			}
		}
		if (onFinished != null)
		{
			onFinished(!flag);
		}
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

	private bool IsInTempClass()
	{
		GamePassTier gamePassTier = MVGameControllerBase.LocalPlayer.SpawnRoleDataMediator.TierRequirement;
		GamePassTier gamePassTier2 = GamePassesManager.PlayerPlanetData.gamePassTier;
		return (int)gamePassTier > (int)gamePassTier2;
	}

	public bool IsInTempTier()
	{
		GamePassTier previewGamePassTier = GamePassesManager.PlayerPlanetData.previewGamePassTier;
		GamePassTier gamePassTier = GamePassesManager.PlayerPlanetData.gamePassTier;
		return (int)previewGamePassTier > (int)gamePassTier;
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
}
