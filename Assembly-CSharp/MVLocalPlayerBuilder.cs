using MV.WorldObject;
using MV.WorldObject.MetaData;
using MV.WorldObject.SpawnRoles;

public class MVLocalPlayerBuilder : MVLocalPlayerRegistered
{
	public struct EnterPlayStateDataStruct
	{
		public MVTeam selectedTeam;

		public int selectedSpawnRoleCreator;

		public int previousSpawnRoleId;
	}

	private EnterPlayStateDataStruct enterPlayStateData = default;

	public EnterPlayStateDataStruct EnterPlayStateData
	{
		get
		{
			return enterPlayStateData;
		}
		set
		{
			enterPlayStateData = value;
		}
	}

	public int BuildModeSpawnRoleId => spawnRolesMetaData.spawnRolesDefaultTypeWoIDMap[DefaultSpawnRoleType.BuildModeSpawnRole];

	public MVLocalPlayerBuilder(int actorNumber, int profileID, string regionCode, int planetOwnershipTypeID, UserProfileData userProfileData)
		: base(actorNumber, profileID, regionCode, planetOwnershipTypeID, userProfileData)
	{
		enterPlayStateData.selectedTeam = MVTeam.None;
		enterPlayStateData.selectedSpawnRoleCreator = -1;
		enterPlayStateData.previousSpawnRoleId = -1;
	}

	public void SetToDefaultPlayModeSpawnRole()
	{
		MVGameControllerBase.LocalPlayer.SetActiveSpawnRole(spawnRolesMetaData.spawnRolesDefaultTypeWoIDMap[DefaultSpawnRoleType.DefaultPlayModeSpawnRole]);
	}

	public void SetToBuildModeSpawnRole()
	{
		MVGameControllerBase.LocalPlayer.SetActiveSpawnRole(spawnRolesMetaData.spawnRolesDefaultTypeWoIDMap[DefaultSpawnRoleType.BuildModeSpawnRole]);
	}
}
