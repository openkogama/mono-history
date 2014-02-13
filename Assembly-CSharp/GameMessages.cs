using System.Collections;
using MV.Common;

public class GameMessages
{
	public struct PlayerKilledMessage
	{
		public int playerId;

		public int killerId;

		public PlayerKilledByType weaponType;
	}

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

	public static string PlayerKilledByTypeToPrettyString(PlayerKilledByType type)
	{
		return type switch
		{
			PlayerKilledByType.CenterGun => "Center Gun", 
			PlayerKilledByType.BazookaGun => "Bazooka", 
			PlayerKilledByType.RailGun => "Rail Gun", 
			_ => type.ToString(), 
		};
	}

	public static PlayerKilledMessage ParsePlayerKilledMessage(Hashtable package)
	{
		return new PlayerKilledMessage
		{
			playerId = (int)package[(byte)0],
			killerId = (int)package[(byte)1],
			weaponType = (PlayerKilledByType)(byte)package[(byte)2]
		};
	}

	public static Hashtable MakePlayerKilledMessage(int avatarId, int killerId, PlayerKilledByType weaponType)
	{
		Hashtable hashtable = new Hashtable();
		hashtable.Add((byte)0, avatarId);
		hashtable.Add((byte)1, killerId);
		hashtable.Add((byte)2, (byte)weaponType);
		return hashtable;
	}

	public static PlayerJoinMessage ParsePlayerJoinMessage(Hashtable package)
	{
		return new PlayerJoinMessage
		{
			playerId = (int)package[(byte)0]
		};
	}

	public static PlayerLeftMessage ParsePlayerLeftMessage(Hashtable package)
	{
		return new PlayerLeftMessage
		{
			playerId = (int)package[(byte)0],
			userName = (string)package[(byte)3]
		};
	}

	public static CollectibleMessage ParseCollectibleMessage(Hashtable package)
	{
		return new CollectibleMessage
		{
			playerId = (int)package[(byte)0]
		};
	}

	public static AchievementGetMessage ParseAchievementGetMessage(Hashtable package)
	{
		return new AchievementGetMessage
		{
			playerId = (int)package[(byte)0],
			achievementType = (AchievementType)(int)package[(byte)4]
		};
	}

	public static CheckpointMessage ParseCheckpointMessage(Hashtable package)
	{
		return new CheckpointMessage
		{
			playerID = (int)package[(byte)0]
		};
	}
}
