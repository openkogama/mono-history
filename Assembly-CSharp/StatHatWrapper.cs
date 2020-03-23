using System.Collections.Generic;
using StatHat;

public static class StatHatWrapper
{
	private static class RegionDatas
	{
		private struct RegionData(string regionKey, bool isEnabled)
		{
			public string regionKey = regionKey;

			public bool isEnabled = isEnabled;
		}

		private static Dictionary<string, RegionData> regionTagToRegionKeyMap = new Dictionary<string, RegionData>
		{
			{
				"local",
				new RegionData("local", isEnabled: false)
			},
			{
				"dev",
				new RegionData("dev", isEnabled: false)
			},
			{
				"test",
				new RegionData("test", isEnabled: false)
			},
			{
				"friends",
				new RegionData("na", isEnabled: true)
			},
			{
				"br",
				new RegionData("br", isEnabled: true)
			},
			{
				"www",
				new RegionData("eu", isEnabled: true)
			}
		};

		public static string Key => regionTagToRegionKeyMap[MVGameControllerBase.KoGaMaSettings.RegionTag].regionKey;

		public static bool IsEnabled => regionTagToRegionKeyMap[MVGameControllerBase.KoGaMaSettings.RegionTag].isEnabled;
	}

	private static bool isFirstTimeSession;

	private const string allBtFormat = "{0}.u.{1}";

	private const string allBtFormatFtsStats = "{0}.u.fts.{1}";

	private const string ezKey = "h5g9REtmi1LT7JY5";

	private const string btFormat = "{0}.u.standalone.{1}";

	private const string btFormatFtsStats = "{0}.u.fts.standalone.{1}";

	public static void Initialize(bool isFirstTimeSession)
	{
		StatHatWrapper.isFirstTimeSession = isFirstTimeSession;
	}

	public static void Count(string key, int count)
	{
		if (RegionDatas.IsEnabled)
		{
			string allBtKey = $"{RegionDatas.Key}.u.{key}";
			string btKey = $"{RegionDatas.Key}.u.standalone.{key}";
			Count(allBtKey, btKey, count);
			if (isFirstTimeSession)
			{
				string allBtKey2 = $"{RegionDatas.Key}.u.fts.{key}";
				string btKey2 = $"{RegionDatas.Key}.u.fts.standalone.{key}";
				Count(allBtKey2, btKey2, count);
			}
		}
	}

	public static void Value(string key, int value)
	{
		if (RegionDatas.IsEnabled)
		{
			string allBtKey = $"{RegionDatas.Key}.u.{key}";
			string btKey = $"{RegionDatas.Key}.u.standalone.{key}";
			Value(allBtKey, btKey, value);
			if (isFirstTimeSession)
			{
				string allBtKey2 = $"{RegionDatas.Key}.u.fts.{key}";
				string btKey2 = $"{RegionDatas.Key}.u.fts.standalone.{key}";
				Value(allBtKey2, btKey2, value);
			}
		}
	}

	public static void Value(string key, float value)
	{
		if (RegionDatas.IsEnabled)
		{
			string allBtKey = $"{RegionDatas.Key}.u.{key}";
			string btKey = $"{RegionDatas.Key}.u.standalone.{key}";
			Value(allBtKey, btKey, value);
			if (isFirstTimeSession)
			{
				string allBtKey2 = $"{RegionDatas.Key}.u.fts.{key}";
				string btKey2 = $"{RegionDatas.Key}.u.fts.standalone.{key}";
				Value(allBtKey2, btKey2, value);
			}
		}
	}

	private static void Count(string allBtKey, string btKey, int count)
	{
		Post.EzCounter("h5g9REtmi1LT7JY5", allBtKey, count);
		Post.EzCounter("h5g9REtmi1LT7JY5", btKey, count);
	}

	private static void Value(string allBtKey, string btKey, int value)
	{
		Post.EzValue("h5g9REtmi1LT7JY5", allBtKey, value);
		Post.EzValue("h5g9REtmi1LT7JY5", btKey, value);
	}

	private static void Value(string allBtKey, string btKey, float value)
	{
		Post.EzValue("h5g9REtmi1LT7JY5", allBtKey, value);
		Post.EzValue("h5g9REtmi1LT7JY5", btKey, value);
	}
}
