using MV.WorldObject.MetaData;
using MV.WorldObject.SpawnRoles;

public class MVLocalPlayerBuilder : MVLocalPlayerRegistered
{
	private SpawnRolesMetaData spawnRolesMetaData;

	public MVLocalPlayerBuilder(int actorNumber, int profileID, string regionCode, int planetOwnershipTypeID, UserProfileData userProfileData)
		: base(actorNumber, profileID, regionCode, planetOwnershipTypeID, userProfileData)
	{
	}

	public void SetSpawnRoleMetaData(SpawnRolesMetaData spawnRolesMetaData)
	{
		this.spawnRolesMetaData = spawnRolesMetaData;
	}

	public void EnterPlayMode()
	{
		MVGameControllerBase.OperationRequests.SetActiveSpawnRole(spawnRolesMetaData.spawnRolesDefaultTypeWoIDMap[DefaultSpawnRoleType.DefaultPlayModeSpawnRole]);
	}

	public void EnterBuildMode()
	{
		MVGameControllerBase.OperationRequests.SetActiveSpawnRole(spawnRolesMetaData.spawnRolesDefaultTypeWoIDMap[DefaultSpawnRoleType.BuildModeSpawnRole]);
	}
}
