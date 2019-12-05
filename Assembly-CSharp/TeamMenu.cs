using System.Collections.Generic;
using System.Linq;
using MV.Common;
using MV.WorldObject;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TeamMenu : LobbyFlowMenu
{
	[SerializeField]
	private VerticalLayoutGroup teamLayoutGroup;

	[SerializeField]
	private TeamSelectButton teamPrefab;

	[SerializeField]
	private GameObject backButton;

	protected override LobbyFlowMenuType MenuType => LobbyFlowMenuType.TeamSelect;

	public override void Start()
	{
		base.Start();
		List<TeamData> teamDatas = MVGameControllerBase.Game.TeamManager.GetTeamDatas(GameStatCounterType.Kill);
		List<TeamData> list = teamDatas.OrderBy((TeamData teamData) => teamData.playersCount).ToList();
		for (int num = 0; num < list.Count; num++)
		{
			TeamSelectButton teamSelectButton = Object.Instantiate(teamPrefab);
			teamSelectButton.Initialize(list[num], OnTeamSelected);
			teamSelectButton.transform.SetParent(teamLayoutGroup.transform, worldPositionStays: false);
		}
	}

	public void UpdateBackButtonVisibility()
	{
		backButton.SetActive(CanSpawnAsSelectedClass());
	}

	private void Update()
	{
		if (MVGameControllerBase.MainCameraManager.BlueModeEnabled)
		{
			MVGameControllerBase.MainCameraManager.CamMaskMode = MaskMode.SkyBoxOnly;
		}
	}

	private void OnTeamSelected(MVTeam selectedTeam)
	{
		base.selectedTeam = selectedTeam;
		if (MVGameControllerBase.LocalPlayer.SpawnRoleDataMediator.SpawnRoleMode.Value == SpawnRoleModeType.Hidden)
		{
			MVGameControllerBase.GameEventManager.AvatarCommandsPlayMode.SetToSpawnPoint();
		}
		UpdateAvailableMenues();
		GoToNextMenu();
	}

	protected override bool CanShowSpawnRoleSelect()
	{
		return MVGameControllerBase.Game.TeamManager.TeamHasSpawnRoles(selectedTeam);
	}

	private bool CanSpawnAsSelectedClass()
	{
		if (GamePassesManager.PlayerPlanetData == null)
		{
			return true;
		}
		GamePassTier gamePassTier = MVGameControllerBase.LocalPlayer.SpawnRoleDataMediator.TierRequirement;
		GamePassTier gamePassTier2 = GamePassesManager.PlayerPlanetData.gamePassTier;
		GamePassTier previewGamePassTier = GamePassesManager.PlayerPlanetData.previewGamePassTier;
		return (int)gamePassTier <= (int)gamePassTier2 || (int)gamePassTier <= (int)previewGamePassTier;
	}

	protected override void StartPlaying()
	{
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
		{
			x.Pop();
		});
		if (!FirstTimePressPlayController.HaveBeenPressed)
		{
			FirstTimePressPlayController.OnFirstTimePlayIsPressed();
		}
		MVGameControllerDesktop.LockCursorManager.CursorLock = true;
		if (MVGameControllerBase.LocalPlayer.SpawnRoleDataMediator.WoId != MVGameControllerBase.LocalPlayer.DefaultSpawnRoleId)
		{
			MVGameControllerBase.LocalPlayer.SetActiveSpawnRole(MVGameControllerBase.LocalPlayer.DefaultSpawnRoleId);
		}
	}
}
