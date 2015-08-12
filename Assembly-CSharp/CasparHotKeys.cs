using System.Collections.Generic;
using MV.WorldObject;
using UnityEngine;

public static class CasparHotKeys
{
	public static void Handle()
	{
		if (MVInputWrapper.DebugGetKeyUp(KeyCode.T))
		{
			Debug.Log("Trying tor create");
			Dictionary<object, object> dictionary = new Dictionary<object, object>();
			dictionary["height"] = 3f;
			dictionary["distanceToAvatar"] = 12f;
			dictionary["smoothness"] = 0.54f;
			dictionary["speedDistanceModifier"] = 12f;
			dictionary["tiltAdjust"] = 1.8f;
			dictionary["avatarWorldCollision"] = false;
			MVGameController.Game.RegisterWorldObject(WorldObjectType.CameraSettings, MVGameController.WOCM.RootGroup.Id, dictionary, Vector3.up * 10f, Quaternion.identity, Vector3.one, localOwner: true, transferOwnershipToServerOnLeave: true);
		}
	}
}
