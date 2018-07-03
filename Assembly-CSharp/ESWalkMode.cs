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
			MVGameControllerBase.WOCM.AvatarLocal.SetMode(AvatarRuntimeState.Playing);
			MVGameControllerDesktop.LockCursorManager.LockCursor = true;
		}
		else if (MVGameControllerBase.Game.NetworkGameStateListener.CurrentGameState == MVGameStateType.RoundEnded)
		{
			MVGameControllerBase.WOCM.AvatarLocal.SetMode(AvatarRuntimeState.Hidden);
			MVGameControllerDesktop.LockCursorManager.LockCursor = false;
		}
		MVGameControllerBase.WOCM.AvatarLocal.Visible = true;
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
		if (MVGameControllerBase.Game.GameType == MVGameType.Classic)
		{
			MVGameControllerBase.WOCM.AvatarLocal.SetMode(AvatarRuntimeState.Edit);
		}
		else if (MVGameControllerBase.Game.GameType == MVGameType.Platformer)
		{
			MVGameControllerBase.WOCM.AvatarLocal.SetMode(AvatarRuntimeState.Edit2D);
			((MVAvatarLocal.EditorAvatarMode2D)MVGameControllerBase.WOCM.AvatarLocal.CurrentMode).ResetToZPos();
			MVGameControllerBase.CameraController.StartTransitionCam(0.5f);
		}
		MVGameControllerBase.WOCM.MoveableController.ResetMoveables();
		MVGameControllerBase.WOCM.RootGroup.PlayModeInitialize();
		MVGameControllerDesktop.LockCursorManager.LockCursor = false;
		if (MVGameControllerBase.Game.GameCoinManager.BoostEnabled)
		{
			MVGameControllerBase.OperationRequests.SetGameCoinBoostState(gameCoinBoosterEnabled: false);
		}
		Cursor.visible = true;
	}
}
