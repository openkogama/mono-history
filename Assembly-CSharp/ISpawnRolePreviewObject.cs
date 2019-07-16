using MV.Common;
using MV.WorldObject;
using UnityEngine;

public interface ISpawnRolePreviewObject
{
	GameObject GetSpawnRolePreviewObject();

	MVTeam GetTeamRequirement();

	GamePassTier GetTierRequirement();
}
