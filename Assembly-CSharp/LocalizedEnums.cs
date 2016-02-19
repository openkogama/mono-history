using System.Collections.Generic;
using Localize;
using MV.Common;

public static class LocalizedEnums
{
	private static class MVConnStateLS
	{
		private static EnumLocalizeBookkeeping enumLocalizeBookkeeping = new EnumLocalizeBookkeeping(Init);

		public static string Get(MVConnState enumVal)
		{
			return enumLocalizeBookkeeping.GetLocalizedString((int)enumVal);
		}

		private static void Init(Dictionary<int, string> map)
		{
			map.Add(0, TM._("Disconnected"));
			map.Add(1, TM._("DisconnectedByUser"));
			map.Add(2, TM._("Connecting"));
			map.Add(3, TM._("Joining"));
			map.Add(4, TM._("Joined"));
			map.Add(5, TM._("Disconnecting"));
			map.Add(6, TM._("Exception"));
			map.Add(7, TM._("TimeoutDisconnect"));
			map.Add(8, TM._("SendError"));
			map.Add(9, TM._("HandlingException"));
		}
	}

	private static class MVJoinStateLS
	{
		private static EnumLocalizeBookkeeping enumLocalizeBookkeeping = new EnumLocalizeBookkeeping(Init);

		public static string Get(MVOperationCodes enumVal)
		{
			return enumLocalizeBookkeeping.GetLocalizedString((int)enumVal);
		}

		private static void Init(Dictionary<int, string> map)
		{
			map.Add(255, TM._("Joining"));
			map.Add(61, TM._("Synchronizing Game Time"));
			map.Add(59, TM._("Fetching Credit Status"));
			map.Add(43, TM._("Fetching Materials"));
			map.Add(84, TM._("Fetching Item Types"));
			map.Add(85, TM._("Fetching Ownership Types"));
			map.Add(87, TM._("Fetching Inventory"));
			map.Add(60, TM._("Fetching BuiltIn Items"));
			map.Add(88, TM._("Fetching Shop Inventory"));
			map.Add(82, TM._("Fetching Avatar Shop Inventory"));
			map.Add(86, TM._("Fetching Game Snapshot"));
			map.Add(66, TM._("Creating Game Snapshot"));
			map.Add(17, TM._("Fetching Friends"));
			map.Add(49, TM._("Fetching Streaming Assets"));
			map.Add(50, TM._("Fetching Streaming Asset Inventory"));
			map.Add(0, TM._("Setting Actor Ready"));
			map.Add(56, TM._("Fetching Active Avatar"));
			map.Add(74, TM._("Initialize Avatar Edit"));
		}
	}

	private static class PlayerKilledByTypeLS
	{
		private static EnumLocalizeBookkeeping enumLocalizeBookkeeping = new EnumLocalizeBookkeeping(Init);

		public static string Get(PlayerKilledByType enumVal)
		{
			return enumLocalizeBookkeeping.GetLocalizedString((int)enumVal);
		}

		private static void Init(Dictionary<int, string> map)
		{
			map.Add(0, TM._("None"));
			map.Add(1, TM._("a Center Gun"));
			map.Add(2, TM._("a Bazooka"));
			map.Add(3, TM._("a RailGun"));
			map.Add(4, TM._("{0} comitted suicide"));
			map.Add(5, TM._("{0} hit the ground too hard"));
			map.Add(6, TM._("{0}  was killed by the environment"));
			map.Add(7, TM._("a Sword"));
			map.Add(8, TM._("{0} was blown to bits"));
			map.Add(9, TM._("{0} burned to death"));
			map.Add(10, TM._("{0} fell off the world"));
			map.Add(11, TM._("Mutant"));
			map.Add(12, TM._("a Shotgun"));
			map.Add(13, TM._("a FlameThrower"));
			map.Add(14, TM._("{0} was crushed by cubes"));
			map.Add(15, TM._("{0} was killed by a ghost"));
			map.Add(16, TM._("{0} was eliminated by an Oculus"));
			map.Add(17, TM._("Colt 45"));
			map.Add(19, TM._("Throwing star"));
			map.Add(20, TM._("Multi throwing star"));
		}
	}

	public static string _(MVConnState enumVal)
	{
		return MVConnStateLS.Get(enumVal);
	}

	public static string _(MVOperationCodes enumVal)
	{
		return MVJoinStateLS.Get(enumVal);
	}

	public static string _(PlayerKilledByType enumVal)
	{
		return PlayerKilledByTypeLS.Get(enumVal);
	}
}
