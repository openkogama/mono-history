using System.Collections.Generic;
using MV.WorldObject;
using UnityEngine;

public abstract class MVSpawnPoint : MVLogicObject
{
	private bool isInWorld;

	public MVSpawnPoint(Dictionary<object, object> data, ObjectPrefab prefabObject, Dictionary<int, MVWorldObjectClient> worldObjects)
		: base(data, prefabObject, worldObjects)
	{
		interactionFlags = InteractionFlags.Selectable | InteractionFlags.CanRotateY;
	}

	public override void Initialize()
	{
		base.Initialize();
		isInWorld = true;
		MVGameControllerBase.Game.TeamManager.OnAddSpawnPoint(Id, WOTypeToTeamIndex(WorldObjectType));
	}

	public override void Destroy()
	{
		base.Destroy();
		if (isInWorld && !MVGameControllerBase.Quitting)
		{
			MVGameControllerBase.Game.TeamManager.OnRemoveSpawnPoint(Id, WOTypeToTeamIndex(WorldObjectType));
		}
	}

	private static MVTeam WOTypeToTeamIndex(WorldObjectType type)
	{
		return type switch
		{
			WorldObjectType.SpawnPointBlue => MVTeam.Blue, 
			WorldObjectType.SpawnPointRed => MVTeam.Red, 
			WorldObjectType.SpawnPointGreen => MVTeam.Green, 
			WorldObjectType.SpawnPointYellow => MVTeam.Yellow, 
			_ => MVTeam.None, 
		};
	}

	public override Vector3 GetClosestGridPoint(float gridSize, Vector3 position)
	{
		Vector3 one = Vector3.one;
		one.y = 2f;
		return SharedCubeFunctions.GetClosestGridPoint(position, gameObject.transform.rotation, gridSize, one);
	}

	public override bool Delete(MVWorldObjectClientManager worldObjectClientManager, ref string errorText)
	{
		if (MVGameControllerBase.Game.TeamManager.NumSpawnPoint <= 1)
		{
			errorText = "You cannot delete the last spawn-point.\nAll Projects must have at least one";
			return false;
		}
		return base.Delete(worldObjectClientManager, ref errorText);
	}
}
