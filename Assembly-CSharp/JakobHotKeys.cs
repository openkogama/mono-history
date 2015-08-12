using System.Collections.Generic;
using MV.Common;
using MV.WorldObject;
using UnityEngine;

public static class JakobHotKeys
{
	private static bool buttonPressed;

	public static void Handle()
	{
		Debug.Log("Button pressed");
		if (!buttonPressed)
		{
			Dictionary<object, object> dictionary = new Dictionary<object, object>();
			dictionary.Add("itemType", AvatarItemType.NinjaRun);
			MVGameController.Game.RegisterWorldObject(WorldObjectType.PickupItemSpawner, MVGameController.WOCM.RootGroup.Id, dictionary, Vector3.up * 25f, Quaternion.identity, Vector3.one, localOwner: true, transferOwnershipToServerOnLeave: true);
			Debug.Log("WorldObject registered");
			buttonPressed = true;
		}
	}
}
