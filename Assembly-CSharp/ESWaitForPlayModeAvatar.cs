using System.Collections.Generic;
using MV.Common;
using MV.WorldObject;

public class ESWaitForPlayModeAvatar : ESStateBase
{
	private enum EnterPlayFromEditSpawnState
	{
		DefaultSpawnAsSpawnRole,
		SpawnAsDefaultAvatar,
		SpawnAsSelectedSpawnRole
	}

	private bool defaultPlayModeSpawnRoleReady;

	private MVTeam selectedTeam = MVTeam.None;

	public override void Enter(EditorStateMachine esm)
	{
		defaultPlayModeSpawnRoleReady = false;
		MVGameControllerBase.LocalPlayer.SpawnRolesManager.OnSpawnRoleActivated += SpawnRolesManagerOnOnSpawnRoleActivated;
		EnterPlayFromEditSpawnState enterSpawnState = CalculateEnterPlayFromEditState();
		HandleEnterPlayModeFromEditSpawn(enterSpawnState);
	}

	private void SpawnRolesManagerOnOnSpawnRoleActivated(int spawnRoleId)
	{
		MVGameControllerBase.LocalPlayer.SpawnRolesManager.OnSpawnRoleActivated -= SpawnRolesManagerOnOnSpawnRoleActivated;
		defaultPlayModeSpawnRoleReady = true;
	}

	public override void Execute(EditorStateMachine e)
	{
		if (defaultPlayModeSpawnRoleReady)
		{
			e.Event = EditorEvent.ESWalkMode;
		}
	}

	public override void Exit(EditorStateMachine esm)
	{
	}

	private EnterPlayFromEditSpawnState CalculateEnterPlayFromEditState()
	{
		int selectedSpawnRoleCreator = ((MVLocalPlayerBuilder)MVGameControllerBase.LocalPlayer).EnterPlayStateData.selectedSpawnRoleCreator;
		List<MVWorldObjectClient> worldObjectsByType = MVGameControllerBase.Game.WorldObjectClientManager.GetWorldObjectsByType(WorldObjectType.AvatarSpawnRoleCreator);
		int numSpawnPoint = MVGameControllerBase.Game.TeamManager.NumSpawnPoint;
		bool flag = worldObjectsByType.Count > 0;
		bool flag2 = MVGameControllerBase.Game.TeamManager.TeamCount() > 1;
		bool isTeamValid = MVGameControllerBase.Game.TeamManager.HasTeam(selectedTeam) || !flag2;
		bool isSpawnRoleValid = MVGameControllerBase.Game.WorldObjectClientManager.GetWorldObject(selectedSpawnRoleCreator) != null || !flag;
		if (IsTeamWithoutSpawnRole(flag2, isTeamValid))
		{
			if (IsSelectedSpawnRoleDeleted(isSpawnRoleValid, selectedSpawnRoleCreator))
			{
				isSpawnRoleValid = false;
			}
			else
			{
				flag = false;
				isSpawnRoleValid = true;
			}
			selectedSpawnRoleCreator = -1;
		}
		if (ShouldEnterAsDefaultSpawnRole(isSpawnRoleValid, flag2, numSpawnPoint))
		{
			return EnterPlayFromEditSpawnState.DefaultSpawnAsSpawnRole;
		}
		if (ShouldEnterAsSelectedSpawnRole(isSpawnRoleValid, flag))
		{
			return EnterPlayFromEditSpawnState.SpawnAsSelectedSpawnRole;
		}
		return EnterPlayFromEditSpawnState.SpawnAsDefaultAvatar;
	}

	private bool IsTeamWithoutSpawnRole(bool teamsPresent, bool isTeamValid)
	{
		return teamsPresent && isTeamValid && !MVGameControllerBase.Game.TeamManager.TeamHasSpawnRoles(selectedTeam);
	}

	private bool IsSelectedSpawnRoleDeleted(bool isSpawnRoleValid, int selectedSpawnRoleCreatorId)
	{
		return !isSpawnRoleValid && selectedSpawnRoleCreatorId >= 0;
	}

	private bool ShouldEnterAsDefaultSpawnRole(bool isSpawnRoleValid, bool teamsPresent, int numberOfSpawnPoints)
	{
		return !isSpawnRoleValid && !teamsPresent && numberOfSpawnPoints == 1;
	}

	private bool ShouldEnterAsSelectedSpawnRole(bool isSpawnRoleValid, bool spawnRolesPresent)
	{
		return isSpawnRoleValid && spawnRolesPresent;
	}

	private void HandleEnterPlayModeFromEditSpawn(EnterPlayFromEditSpawnState enterSpawnState)
	{
		switch (enterSpawnState)
		{
		case EnterPlayFromEditSpawnState.DefaultSpawnAsSpawnRole:
			HandleDefaultSpawnAsSpawnRole();
			break;
		case EnterPlayFromEditSpawnState.SpawnAsDefaultAvatar:
			HandleSpawnAsDefaultPlayModeSpawnRole();
			break;
		case EnterPlayFromEditSpawnState.SpawnAsSelectedSpawnRole:
			HanldeSpawnAsSelectedSpawnRole();
			break;
		}
	}

	private void HandleDefaultSpawnAsSpawnRole()
	{
		List<MVWorldObjectClient> worldObjectsByType = MVGameControllerBase.Game.WorldObjectClientManager.GetWorldObjectsByType(WorldObjectType.AvatarSpawnRoleCreator);
		int id = worldObjectsByType[0].Id;
		SpawnAsSelectedSpawnRole(id);
	}

	private void HandleSpawnAsDefaultPlayModeSpawnRole()
	{
		SpawnAsDefaultPlayModeSpawnRole();
	}

	private void HanldeSpawnAsSelectedSpawnRole()
	{
		int selectedSpawnRoleCreator = ((MVLocalPlayerBuilder)MVGameControllerBase.LocalPlayer).EnterPlayStateData.selectedSpawnRoleCreator;
		SpawnAsSelectedSpawnRole(selectedSpawnRoleCreator);
	}

	private void SpawnAsSelectedSpawnRole(int spawnRoleId)
	{
		if (CanSpawnAsSelectedSpawnRole(spawnRoleId))
		{
			MVGameControllerBase.Game.LocalPlayer.CreateSpawnRole(spawnRoleId);
			MVLocalPlayerBuilder.EnterPlayStateDataStruct enterPlayStateData = ((MVLocalPlayerBuilder)MVGameControllerBase.LocalPlayer).EnterPlayStateData;
			enterPlayStateData.selectedSpawnRoleCreator = spawnRoleId;
			((MVLocalPlayerBuilder)MVGameControllerBase.LocalPlayer).EnterPlayStateData = enterPlayStateData;
		}
		else
		{
			SpawnAsDefaultPlayModeSpawnRole();
			MVLocalPlayerBuilder.EnterPlayStateDataStruct enterPlayStateData2 = ((MVLocalPlayerBuilder)MVGameControllerBase.LocalPlayer).EnterPlayStateData;
			enterPlayStateData2.selectedSpawnRoleCreator = MVGameControllerBase.LocalPlayer.DefaultSpawnRoleId;
			((MVLocalPlayerBuilder)MVGameControllerBase.LocalPlayer).EnterPlayStateData = enterPlayStateData2;
		}
	}

	private void SpawnAsDefaultPlayModeSpawnRole()
	{
		((MVLocalPlayerBuilder)MVGameControllerBase.LocalPlayer).SetToDefaultPlayModeSpawnRole();
	}

	private bool CanSpawnAsSelectedSpawnRole(int spawnRoleId)
	{
		if (!(MVGameControllerBase.Game.WorldObjectClientManager.GetWorldObject(spawnRoleId) is MVAvatarSpawnRoleCreator))
		{
			return false;
		}
		MVAvatarSpawnRoleCreator mVAvatarSpawnRoleCreator = (MVAvatarSpawnRoleCreator)MVGameControllerBase.Game.WorldObjectClientManager.GetWorldObject(spawnRoleId);
		if (mVAvatarSpawnRoleCreator == null)
		{
			return false;
		}
		GamePassTier gamePassTier = GamePassesManager.PlayerPlanetData.gamePassTier;
		GamePassTier tier = mVAvatarSpawnRoleCreator.Tier;
		if ((int)tier > (int)gamePassTier)
		{
			return false;
		}
		return true;
	}
}
