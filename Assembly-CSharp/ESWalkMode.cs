using System;
using System.Collections.Generic;
using MV.Common;
using MV.WorldObject;
using UnityEngine.EventSystems;

internal class ESWalkMode : ESStateBase
{
	private enum EnterPlayFromEditState
	{
		EnterPlayMode,
		SelectTeamOrSpawnRole,
		WaitForRoundToStart
	}

	public override void Enter(EditorStateMachine esm)
	{
		ExecuteEvents.ExecuteHierarchy(esm.GameObject, null, (IEditModeController x, BaseEventData y) =>
		{
			x.DisableEditMode();
		});
		esm.DeSelectAll();
		esm.ExitGroupToRoot();
		MVGameControllerBase.WOCM.RootGroup.PlayModeInitialize();
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
		SpawnRoleMenu.OnNewSpawnRoleSelected = (Action<int>)Delegate.Combine(SpawnRoleMenu.OnNewSpawnRoleSelected, new Action<int>(OnNewSpawnRoleSelected));
	}

	public override void Execute(EditorStateMachine e)
	{
	}

	public override void Exit(EditorStateMachine esm)
	{
		SpawnRoleMenu.OnNewSpawnRoleSelected = (Action<int>)Delegate.Remove(SpawnRoleMenu.OnNewSpawnRoleSelected, new Action<int>(OnNewSpawnRoleSelected));
	}

	private void HandleEnterPlayInEditMode()
	{
		EnterPlayFromEditState enterState = CalculateEnterPlayFromEditState();
		HandleEnterState(enterState);
	}

	private EnterPlayFromEditState CalculateEnterPlayFromEditState()
	{
		MVTeam selectedTeam = ((MVLocalPlayerBuilder)MVGameControllerBase.LocalPlayer).EnterPlayStateData.selectedTeam;
		int selectedSpawnRoleCreator = ((MVLocalPlayerBuilder)MVGameControllerBase.LocalPlayer).EnterPlayStateData.selectedSpawnRoleCreator;
		List<MVWorldObjectClient> worldObjectsByType = MVGameControllerBase.Game.WorldObjectClientManager.GetWorldObjectsByType(WorldObjectType.AvatarSpawnRoleCreator);
		bool flag = worldObjectsByType.Count > 0;
		bool flag2 = MVGameControllerBase.Game.TeamManager.TeamCount() > 1;
		bool isTeamValid = MVGameControllerBase.Game.TeamManager.HasTeam(selectedTeam) || !flag2;
		bool isSpawnRoleValid = MVGameControllerBase.Game.WorldObjectClientManager.GetWorldObject(selectedSpawnRoleCreator) != null || !flag;
		bool flag3 = MVGameControllerBase.Game.NetworkGameStateListener.CurrentGameState == MVGameStateType.RoundEnded;
		if (ShouldSelectTeamOrSpawnRole(isSpawnRoleValid, isTeamValid))
		{
			return EnterPlayFromEditState.SelectTeamOrSpawnRole;
		}
		if (flag3)
		{
			return EnterPlayFromEditState.WaitForRoundToStart;
		}
		return EnterPlayFromEditState.EnterPlayMode;
	}

	private bool ShouldSelectTeamOrSpawnRole(bool isSpawnRoleValid, bool isTeamValid)
	{
		return (!isSpawnRoleValid && !WasPlayingAsDefaultAvatar()) || !isTeamValid;
	}

	private void HandleEnterState(EnterPlayFromEditState enterState)
	{
		switch (enterState)
		{
		case EnterPlayFromEditState.EnterPlayMode:
			HandleEnterPlayMode();
			break;
		case EnterPlayFromEditState.SelectTeamOrSpawnRole:
			HandleSelectTeamOrSpawnRole();
			break;
		case EnterPlayFromEditState.WaitForRoundToStart:
			HanldeWaitForRoundToStart();
			break;
		}
	}

	private void HandleEnterPlayMode()
	{
		TryLockCursor();
	}

	private void HandleSelectTeamOrSpawnRole()
	{
		MVGameControllerBase.GameEventManager.GameState.OnEnableLobbyState();
		MVGameControllerBase.GameEventManager.AvatarCommandsPlayMode.RemoveFromGame();
		MVGameControllerDesktop.LockCursorManager.CursorLock = false;
	}

	private void HanldeWaitForRoundToStart()
	{
		SetToHiddenMode();
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

	private void SetToHiddenMode()
	{
		MVGameControllerBase.GameEventManager.AvatarCommandsPlayMode.RemoveFromGame();
		MVGameControllerDesktop.LockCursorManager.CursorLock = false;
	}

	private bool WasPlayingAsDefaultAvatar()
	{
		return ((MVLocalPlayerBuilder)MVGameControllerBase.LocalPlayer).EnterPlayStateData.previousSpawnRoleId == MVGameControllerBase.LocalPlayer.DefaultSpawnRoleId;
	}

	private void OnNewSpawnRoleSelected(int newSpawnRoleId)
	{
		MVLocalPlayerBuilder.EnterPlayStateDataStruct enterPlayStateData = ((MVLocalPlayerBuilder)MVGameControllerBase.LocalPlayer).EnterPlayStateData;
		enterPlayStateData.selectedSpawnRoleCreator = newSpawnRoleId;
		((MVLocalPlayerBuilder)MVGameControllerBase.LocalPlayer).EnterPlayStateData = enterPlayStateData;
	}
}
