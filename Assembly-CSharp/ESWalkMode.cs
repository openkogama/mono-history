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
		MVGameControllerBase.WOCM.AvatarLocal.SetMode(AvatarRuntimeState.Playing);
		MVGameControllerBase.WOCM.MoveableController.ResetMoveables();
		MVGameControllerBase.PostGameMsg(MVGameMsgType.AdminMsg, TM._("Leveling is disabled in edit play mode"));
		MVTeam team = MVGameControllerBase.Game.LocalPlayer.Team;
		MVTeamManager teamManager = MVGameControllerBase.Game.TeamManager;
		if (!teamManager.IsTeamActive(team))
		{
			List<MVTeam> teamList = teamManager.GetTeamList();
			MVGameControllerBase.Game.SetTeam(teamList[0]);
		}
		MVGameControllerDesktop.LockCursorManager.LockCursor = true;
		MVGameControllerBase.WOCM.AvatarLocal.Body.AccessoryMoveOverride = true;
		DrawPlane.HideDrawPlane();
		Debug.LogWarning("ShowBriefing()");
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
		MVGameControllerBase.WOCM.AvatarLocal.Body.AccessoryMoveOverride = false;
		if (MVGameControllerBase.Game.GameCoinManager.BoostEnabled)
		{
			MVGameControllerBase.Game.SetGameCoinBoostState(gameCoinBoosterEnabled: false);
		}
		Cursor.visible = true;
	}
}
