using System;
using System.Collections.Generic;
using MV.Common;
using UnityEngine;

public static class GameMessagesRepository
{
	private struct PlayerKilledMessage
	{
		public int playerId;

		public int killerId;

		public PlayerKilledByType weaponType;
	}

	public static bool ShowKilledMessage(Dictionary<object, object> package)
	{
		PlayerKilledMessage playerKilledMessage = ParsePlayerKilledMessage(package);
		try
		{
			MVPlayer mVPlayer = MVGameControllerBase.Game.Players[playerKilledMessage.killerId];
			MVPlayer mVPlayer2 = MVGameControllerBase.Game.Players[playerKilledMessage.playerId];
			if (mVPlayer.IsAnonymous && mVPlayer2.IsAnonymous)
			{
				return false;
			}
			if (mVPlayer.ActorNr == mVPlayer2.ActorNr)
			{
				return false;
			}
		}
		catch (Exception message)
		{
			Debug.LogError(message);
			return false;
		}
		return true;
	}

	public static string CreateKilledMessage(Dictionary<object, object> package)
	{
		PlayerKilledMessage data = ParsePlayerKilledMessage(package);
		return BuildLine(data);
	}

	private static PlayerKilledMessage ParsePlayerKilledMessage(Dictionary<object, object> package)
	{
		return new PlayerKilledMessage
		{
			playerId = (int)package[(byte)0],
			killerId = (int)package[(byte)1],
			weaponType = (PlayerKilledByType)(byte)package[(byte)2]
		};
	}

	private static string BuildLine(PlayerKilledMessage data)
	{
		string empty = string.Empty;
		MVPlayer mVPlayer = MVGameControllerBase.Game.Players[data.killerId];
		if (data.killerId != data.playerId)
		{
			MVPlayer mVPlayer2 = MVGameControllerBase.Game.Players[data.playerId];
			string format = TM._("{0} killed {1} with {2}");
			return string.Format(format, mVPlayer.Username, mVPlayer2.Username, LocalizedEnums._(data.weaponType));
		}
		string text = LocalizedEnums._(data.weaponType);
		if (data.weaponType == PlayerKilledByType.BazookaGun)
		{
			text = TM._("{0} failed to rocketjump");
		}
		Debug.Log(text);
		if (text.Contains("0"))
		{
			return string.Format(text, mVPlayer.Username);
		}
		return text;
	}
}
