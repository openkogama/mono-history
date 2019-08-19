using System;
using System.Collections.Generic;
using MV.Common;
using MV.WorldObject;
using UnityEngine;

internal class ESWalkMode : ESStateBase
{
	private int selectedSpawnRoleCreatorId = -1;

	private MVTeam selectedTeam = MVTeam.None;

	public override void Enter(EditorStateMachine esm)
	{
		SpawnRoleMenu.OnNewSpawnRoleSelected = (Action<int>)Delegate.Combine(SpawnRoleMenu.OnNewSpawnRoleSelected, new Action<int>(OnNewSpawnRoleSelected));
		esm.DeSelectAll();
		esm.ExitGroupToRoot();
		MVGameControllerBase.WOCM.RootGroup.PlayModeInitialize();
		Debug.Log(MVGameControllerBase.Game.NetworkGameStateListener.CurrentGameState);
		HandleEnterPlayInEditMode();
		MVGameControllerBase.WOCM.MoveableController.ResetMoveables();
		MVTeam team = MVGameControllerBase.Game.LocalPlayer.Team;
		MVTeamManager teamManager = MVGameControllerBase.Game.TeamManager;
		if (team != MVTeam.None && !teamManager.IsTeamActive(team))
		{
			List<MVTeam> teamList = teamManager.GetTeamList();
			MVGameControllerBase.OperationRequests.SetTeam(teamList[0]);
		}
		DrawPlane.HideDrawPlane();
	}

	public override void Execute(EditorStateMachine e)
	{
	}

	public override void Exit(EditorStateMachine esm)
	{
		SpawnRoleMenu.OnNewSpawnRoleSelected = (Action<int>)Delegate.Remove(SpawnRoleMenu.OnNewSpawnRoleSelected, new Action<int>(OnNewSpawnRoleSelected));
		MVGameControllerBase.GameEventManager.AvatarCommandsBuildMode.SetToEditMode();
		MVGameControllerBase.WOCM.MoveableController.ResetMoveables();
		MVGameControllerBase.WOCM.RootGroup.PlayModeInitialize();
		MVGameControllerDesktop.LockCursorManager.CursorLock = false;
		if (MVGameControllerBase.MainCameraManager.CamMaskMode != MaskMode.Default)
		{
			MVGameControllerBase.MainCameraManager.CamMaskMode = MaskMode.Default;
		}
		if (MVGameControllerBase.Game.GameCoinManager.BoostEnabled)
		{
			Debug.LogWarning("Game coints. Probably do this directly. ");
		}
		Cursor.visible = true;
		((MVLocalPlayerBuilder)MVGameControllerBase.LocalPlayer).EnterBuildMode();
		selectedTeam = MVGameControllerBase.LocalPlayer.Team;
	}

	private void HandleEnterPlayInEditMode()
	{
		bool flag = MVGameControllerBase.Game.NetworkGameStateListener.CurrentGameState == MVGameStateType.RoundEnded;
		List<MVWorldObjectClient> worldObjectsByType = MVGameControllerBase.Game.WorldObjectClientManager.GetWorldObjectsByType(WorldObjectType.AvatarSpawnRoleCreator);
		int numSpawnPoint = MVGameControllerBase.Game.TeamManager.NumSpawnPoint;
		bool flag2 = worldObjectsByType.Count > 0;
		bool flag3 = MVGameControllerBase.Game.TeamManager.TeamCount() > 1;
		bool flag4 = MVGameControllerBase.Game.TeamManager.HasTeam(selectedTeam) || !flag3;
		bool flag5 = MVGameControllerBase.Game.WorldObjectClientManager.GetWorldObject(selectedSpawnRoleCreatorId) != null || !flag2;
		if (flag3 && flag4 && !MVGameControllerBase.Game.TeamManager.TeamHasSpawnRoles(selectedTeam))
		{
			if (!flag5 && selectedSpawnRoleCreatorId >= 0)
			{
				flag5 = false;
			}
			else
			{
				flag2 = false;
				flag5 = true;
			}
			selectedSpawnRoleCreatorId = -1;
		}
		if (!flag5 && !flag3 && numSpawnPoint == 1)
		{
			selectedSpawnRoleCreatorId = worldObjectsByType[0].Id;
			MVGameControllerBase.Game.OperationRequestSender.CreateSpawnRole(worldObjectsByType[0].Id);
			if (flag)
			{
				MVGameControllerBase.GameEventManager.AvatarCommandsPlayMode.RemoveFromGame();
				MVGameControllerDesktop.LockCursorManager.CursorLock = false;
			}
			else
			{
				TryLockCursor();
			}
		}
		else if (!flag5 || !flag4)
		{
			MVGameControllerBase.GameEventManager.GameState.OnEnableLobbyState();
			MVGameControllerDesktop.LockCursorManager.CursorLock = false;
		}
		else if (flag2)
		{
			MVGameControllerBase.Game.OperationRequestSender.CreateSpawnRole(selectedSpawnRoleCreatorId);
			if (flag)
			{
				MVGameControllerBase.GameEventManager.AvatarCommandsPlayMode.RemoveFromGame();
				MVGameControllerDesktop.LockCursorManager.CursorLock = false;
			}
			else
			{
				TryLockCursor();
			}
		}
		else
		{
			if (flag)
			{
				MVGameControllerBase.GameEventManager.AvatarCommandsPlayMode.RemoveFromGame();
				MVGameControllerDesktop.LockCursorManager.CursorLock = false;
			}
			else
			{
				MVGameControllerBase.GameEventManager.AvatarCommandsPlayMode.Spawn();
				TryLockCursor();
			}
			((MVLocalPlayerBuilder)MVGameControllerBase.LocalPlayer).EnterPlayMode();
		}
	}

	private void TryLockCursor()
	{
		bool flag = true;
		if (flag)
		{
			MVGameControllerDesktop.LockCursorManager.CursorLock = true;
		}
		else if (!flag && !MVGameControllerDesktop.LockCursorManager.CursorLock)
		{
			MVGameControllerDesktop.LockCursorManager.CursorLock = false;
		}
	}

	private void OnNewSpawnRoleSelected(int newSpawnRoleId)
	{
		selectedSpawnRoleCreatorId = newSpawnRoleId;
	}
}
