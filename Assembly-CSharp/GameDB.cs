using System;
using System.Collections.Generic;
using MV.Common;

public static class GameDB
{
	public static MVGameType GameType => MVGameController.Game.GameType;

	public static bool IsPlatformerGame => MVGameController.Game.GameType == MVGameType.Platformer;

	public static bool IsClassicGame => MVGameController.Game.GameType == MVGameType.Classic;

	public static bool IsPlaying => MVGameController.Game.IsPlaying;

	public static bool HasGravityCube => MVGameController.WOCM.GetSingletonWorldObject<MVGravityCube>() != null;

	public static MVAvatarLocal LocalAvatar => MVGameController.WOCM.AvatarLocal;

	public static MVWorldObjectClient GetWorldObject(int woId)
	{
		return MVGameController.WOCM.GetWorldObjectClient(woId);
	}

	public static MVWorldObjectClient GetWorldObjectWhere(Func<MVWorldObjectClient, bool> predicate)
	{
		return MVGameController.WOCM.GetWorldObjectClientWhere(predicate);
	}

	public static IEnumerable<MVWorldObjectClient> GetWorldObjectsWhere(Func<MVWorldObjectClient, bool> predicate)
	{
		return MVGameController.WOCM.GetWorldObjectClientsWhere(predicate);
	}

	public static MVCameraSettings GetCameraSettingsObject()
	{
		return MVGameController.WOCM.GetSingletonWorldObject<MVCameraSettings>();
	}

	public static MVGravityCube GetGravityCube()
	{
		return MVGameController.WOCM.GetSingletonWorldObject<MVGravityCube>();
	}

	public static PlaymodeCamera GetPlaymodeCamera()
	{
		PlaymodeCamera result = null;
		if (GameType == MVGameType.Classic)
		{
			result = MVGameController.Game.CameraController.GetCamera<ThirdPersonCamera>();
		}
		else if (GameType == MVGameType.Platformer)
		{
			result = MVGameController.Game.CameraController.GetCamera<PlatformerCamera>();
		}
		return result;
	}
}
