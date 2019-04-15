using System.Collections.Generic;
using MV.WorldObject;
using UnityEngine;

public abstract class MVSpawnPoint : MVLogicObject
{
	public MVSpawnPoint(Dictionary<object, object> data, ObjectPrefab prefabObject, Dictionary<int, MVWorldObjectClient> worldObjects)
		: base(data, prefabObject, worldObjects)
	{
		interactionFlags = InteractionFlags.Selectable | InteractionFlags.CanRotateY;
	}

	public override Vector3 GetClosestGridPoint(float gridSize, Vector3 position)
	{
		Vector3 one = Vector3.one;
		one.y = 2f;
		return SharedCubeFunctions.GetClosestGridPoint(position, gameObject.transform.rotation, gridSize, one);
	}

	public override bool Delete(MVWorldObjectClientManager worldObjectClientManager, ref string errorText)
	{
		int num = MVGameControllerBase.WOCM.GetWorldObjectsByType(WorldObjectType.SpawnPointBlue).Count + MVGameControllerBase.WOCM.GetWorldObjectsByType(WorldObjectType.SpawnPointRed).Count + MVGameControllerBase.WOCM.GetWorldObjectsByType(WorldObjectType.SpawnPointGreen).Count + MVGameControllerBase.WOCM.GetWorldObjectsByType(WorldObjectType.SpawnPointYellow).Count;
		if (num <= 1)
		{
			errorText = "You cannot delete the last spawn-point.\nAll Projects must have at least one";
			return false;
		}
		return base.Delete(worldObjectClientManager, ref errorText);
	}

	public override void OnDataUpdate()
	{
		base.OnDataUpdate();
	}
}
