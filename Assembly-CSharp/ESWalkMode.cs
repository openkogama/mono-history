using System.Collections.Generic;
using MV.Common;
using MV.WorldObject;
using UnityEngine;

internal class ESWalkMode : ESStateBase
{
	public override void Enter(EditorStateMachine esm)
	{
		esm.DeSelectAll();
		esm.ExitGroupToRoot();
		MVGameControllerBase.WOCM.RootGroup.PlayModeInitialize();
		Debug.Log(MVGameControllerBase.Game.NetworkGameStateListener.CurrentGameState);
		if (MVGameControllerBase.Game.NetworkGameStateListener.CurrentGameState == MVGameStateType.Round)
		{
			MVGameControllerBase.GameEventManager.AvatarCommandsPlayMode.Spawn();
			MVGameControllerDesktop.LockCursorManager.CursorLock = true;
		}
		else if (MVGameControllerBase.Game.NetworkGameStateListener.CurrentGameState == MVGameStateType.RoundEnded)
		{
			MVGameControllerBase.GameEventManager.AvatarCommandsPlayMode.RemoveFromGame();
			MVGameControllerDesktop.LockCursorManager.CursorLock = false;
		}
		MVGameControllerBase.WOCM.MoveableController.ResetMoveables();
		MVTeam team = MVGameControllerBase.Game.LocalPlayer.Team;
		MVTeamManager teamManager = MVGameControllerBase.Game.TeamManager;
		if (team != MVTeam.None && !teamManager.IsTeamActive(team))
		{
			List<MVTeam> teamList = teamManager.GetTeamList();
			MVGameControllerBase.OperationRequests.SetTeam(teamList[0]);
		}
		DrawPlane.HideDrawPlane();
		((MVLocalPlayerBuilder)MVGameControllerBase.LocalPlayer).EnterPlayMode();
	}

	public override void Execute(EditorStateMachine e)
	{
	}

	public override void Exit(EditorStateMachine esm)
	{
		MVGameControllerBase.GameEventManager.AvatarCommandsBuildMode.SetToEditMode();
		MVGameControllerBase.WOCM.MoveableController.ResetMoveables();
		MVGameControllerBase.WOCM.RootGroup.PlayModeInitialize();
		MVGameControllerDesktop.LockCursorManager.CursorLock = false;
		if (MVGameControllerBase.Game.GameCoinManager.BoostEnabled)
		{
			Debug.LogWarning("Game coints. Probably do this directly. ");
		}
		Cursor.visible = true;
		((MVLocalPlayerBuilder)MVGameControllerBase.LocalPlayer).EnterBuildMode();
	}
}
