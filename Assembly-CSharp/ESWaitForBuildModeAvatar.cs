using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class ESWaitForBuildModeAvatar : ESStateBase
{
	private bool defaultBuildModeSpawnRoleReady;

	public override void Enter(EditorStateMachine esm)
	{
		MVLocalPlayerBuilder.EnterPlayStateDataStruct enterPlayStateData = ((MVLocalPlayerBuilder)MVGameControllerBase.LocalPlayer).EnterPlayStateData;
		enterPlayStateData.previousSpawnRoleId = MVGameControllerBase.LocalPlayer.SpawnRolesManager.SpawnRoleId;
		enterPlayStateData.selectedTeam = MVGameControllerBase.LocalPlayer.Team;
		((MVLocalPlayerBuilder)MVGameControllerBase.LocalPlayer).EnterPlayStateData = enterPlayStateData;
		defaultBuildModeSpawnRoleReady = false;
		MVGameControllerBase.LocalPlayer.SpawnRolesManager.OnSpawnRoleActivated += SpawnRolesManagerOnOnSpawnRoleActivated;
		((MVLocalPlayerBuilder)MVGameControllerBase.LocalPlayer).SetToBuildModeSpawnRole();
	}

	private void SpawnRolesManagerOnOnSpawnRoleActivated(int spawnRoleId)
	{
		MVGameControllerBase.LocalPlayer.SpawnRolesManager.OnSpawnRoleActivated -= SpawnRolesManagerOnOnSpawnRoleActivated;
		if (((MVLocalPlayerBuilder)MVGameControllerBase.LocalPlayer).BuildModeSpawnRoleId == spawnRoleId)
		{
			defaultBuildModeSpawnRoleReady = true;
			return;
		}
		throw new Exception("Unexpected spawnrole");
	}

	public override void Execute(EditorStateMachine e)
	{
		if (defaultBuildModeSpawnRoleReady)
		{
			e.Event = EditorEvent.ESTerrainEdit;
		}
	}

	public override void Exit(EditorStateMachine esm)
	{
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
		ExecuteEvents.ExecuteHierarchy(esm.GameObject, null, (IEditModeController x, BaseEventData y) =>
		{
			x.EnterBuildMode();
		});
	}
}
