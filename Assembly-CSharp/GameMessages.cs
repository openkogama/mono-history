using System.Collections.Generic;
using MV.Common;

public class GameMessages
{
	public struct PlayerJoinMessage
	{
		public int playerId;
	}

	public struct PlayerLeftMessage
	{
		public int playerId;

		public string userName;
	}

	public struct CollectibleMessage
	{
		public int playerId;
	}

	public struct AchievementGetMessage
	{
		public int playerId;

		public AchievementType achievementType;
	}

	public struct CheckpointMessage
	{
		public int playerID;
	}

	public static Dictionary<object, object> MakePlayerKilledMessage(int avatarId, int killerId, PlayerKilledByType weaponType)
	{
		Dictionary<object, object> dictionary = new Dictionary<object, object>();
		dictionary.Add((byte)0, avatarId);
		dictionary.Add((byte)1, killerId);
		dictionary.Add((byte)2, (byte)weaponType);
		return dictionary;
	}

	public static PlayerJoinMessage ParsePlayerJoinMessage(Dictionary<object, object> package)
	{
		return new PlayerJoinMessage
		{
			playerId = (int)package[(byte)0]
		};
	}

	public static PlayerLeftMessage ParsePlayerLeftMessage(Dictionary<object, object> package)
	{
		return new PlayerLeftMessage
		{
			playerId = (int)package[(byte)0],
			userName = (string)package[(byte)3]
		};
	}

	public static CollectibleMessage ParseCollectibleMessage(Dictionary<object, object> package)
	{
		return new CollectibleMessage
		{
			playerId = (int)package[(byte)0]
		};
	}

	public static AchievementGetMessage ParseAchievementGetMessage(Dictionary<object, object> package)
	{
		return new AchievementGetMessage
		{
			playerId = (int)package[(byte)0],
			achievementType = (AchievementType)(int)package[(byte)4]
		};
	}

	public static CheckpointMessage ParseCheckpointMessage(Dictionary<object, object> package)
	{
		return new CheckpointMessage
		{
			playerID = (int)package[(byte)0]
		};
	}
}
