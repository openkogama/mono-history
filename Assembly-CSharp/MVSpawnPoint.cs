using System.Collections;
using System.Collections.Generic;
using Localize;
using MV.WorldObject;
using UnityEngine;

public abstract class MVSpawnPoint : MVLogicObject
{
	public MVSpawnPoint(Hashtable data, string prefabPath, Dictionary<int, MVWorldObjectClient> worldObjects)
		: base(data, prefabPath, worldObjects)
	{
		interactionFlags = InteractionFlags.Selectable | InteractionFlags.CanRotateY;
	}

	public override Vector3 GetClosestGridPoint(float gridSize, Vector3 position)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		Vector3 one = Vector3.one;
		one.y = 2f;
		return SharedCubeFunctions.GetClosestGridPoint(position, gameObject.transform.rotation, gridSize, one);
	}

	public override bool Delete(MVWorldObjectClientManager worldObjectClientManager, ref TextSlotIndex errorTextIndex)
	{
		int num = MVGameController.Instance.WOCM.GetWorldObjectsByType(WorldObjectType.SpawnPointBlue).Count + MVGameController.Instance.WOCM.GetWorldObjectsByType(WorldObjectType.SpawnPointRed).Count + MVGameController.Instance.WOCM.GetWorldObjectsByType(WorldObjectType.SpawnPointGreen).Count + MVGameController.Instance.WOCM.GetWorldObjectsByType(WorldObjectType.SpawnPointYellow).Count;
		if (num <= 1)
		{
			errorTextIndex = TextSlotIndex.DeleteSpawnPointWarning;
			return false;
		}
		return base.Delete(worldObjectClientManager, ref errorTextIndex);
	}
}
