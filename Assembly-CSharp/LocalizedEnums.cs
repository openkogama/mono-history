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

		public static string Get(MVEventCodes enumVal)
		{
			return enumLocalizeBookkeeping.GetLocalizedString((int)enumVal);
		}

		private static void Init(Dictionary<int, string> map)
		{
			map.Add(255, TM._("Joining"));
			map.Add(63, TM._("Synchronizing Game Time"));
			map.Add(64, TM._("Fetching Materials"));
			map.Add(66, TM._("Fetching Item Types"));
			map.Add(65, TM._("Fetching Ownership Types"));
			map.Add(73, TM._("Fetching Inventory"));
			map.Add(75, TM._("Fetching BuiltIn Items"));
			map.Add(74, TM._("Fetching Shop Inventory"));
			map.Add(76, TM._("Fetching Avatar Shop Inventory"));
			map.Add(69, TM._("Fetching Game Snapshot"));
			map.Add(68, TM._("Creating Game Snapshot"));
			map.Add(72, TM._("Fetching Friends"));
			map.Add(67, TM._("Fetching Streaming Assets"));
			map.Add(71, TM._("Fetching Streaming Asset Inventory"));
			map.Add(70, TM._("Setting Actor Ready"));
			map.Add(78, TM._("Fetching Active Avatar"));
			map.Add(77, TM._("Initialize Avatar Edit"));
			map.Add(95, TM._("Getting Profile Meta Data"));
		}
	}

	private static class XPRewardTypeLS
	{
		private static EnumLocalizeBookkeeping enumLocalizeBookkeeping = new EnumLocalizeBookkeeping(Init);

		public static string Get(XPRewardType enumVal)
		{
			return enumLocalizeBookkeeping.GetLocalizedString((int)enumVal);
		}

		private static void Init(Dictionary<int, string> map)
		{
			map.Add(1, TM._("Play mode reward!"));
			map.Add(3, TM._("Avatar mode reward!"));
			map.Add(2, TM._("Build mode reward!"));
			map.Add(4, TM._("Cool!"));
			map.Add(5, TM._("Nice!"));
			map.Add(6, TM._("Awesome!"));
		}
	}

	public static string _(MVConnState enumVal)
	{
		return MVConnStateLS.Get(enumVal);
	}

	public static string _(MVEventCodes enumVal)
	{
		return MVJoinStateLS.Get(enumVal);
	}

	public static string _(XPRewardType enumVal)
	{
		return XPRewardTypeLS.Get(enumVal);
	}
}
