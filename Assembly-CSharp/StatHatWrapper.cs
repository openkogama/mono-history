using StatHat;

public static class StatHatWrapper
{
	private static bool doDetailedStatsForSession;

	private static string allBtFormat = "{0}.u.{1}";

	private static string allBtFormatDetailedStats = "{0}.u.detailedstat.{1}";

	private static string btFormat = "{0}.u.standalone.{1}";

	private static string btFormatDetailedStats = "{0}.u.detailedstat.standalone.{1}";

	public static void DoDetailedStatsForSession()
	{
		doDetailedStatsForSession = true;
	}

	public static void Count(string key, int count)
	{
		string allBtKey = string.Format(allBtFormat, MVGameControllerBase.GameSessionData.region, key);
		string btKey = string.Format(btFormat, MVGameControllerBase.GameSessionData.region, key);
		Count(allBtKey, btKey, count);
		if (doDetailedStatsForSession)
		{
			string allBtKey2 = string.Format(allBtFormatDetailedStats, MVGameControllerBase.GameSessionData.region, key);
			string btKey2 = string.Format(btFormatDetailedStats, MVGameControllerBase.GameSessionData.region, key);
			Count(allBtKey2, btKey2, count);
		}
	}

	public static void Value(string key, int value)
	{
		string allBtKey = string.Format(allBtFormat, MVGameControllerBase.GameSessionData.region, key);
		string btKey = string.Format(btFormat, MVGameControllerBase.GameSessionData.region, key);
		Value(allBtKey, btKey, value);
		if (doDetailedStatsForSession)
		{
			string allBtKey2 = string.Format(allBtFormatDetailedStats, MVGameControllerBase.GameSessionData.region, key);
			string btKey2 = string.Format(btFormatDetailedStats, MVGameControllerBase.GameSessionData.region, key);
			Value(allBtKey2, btKey2, value);
		}
	}

	public static void Value(string key, float value)
	{
		string allBtKey = string.Format(allBtFormat, MVGameControllerBase.GameSessionData.region, key);
		string btKey = string.Format(btFormat, MVGameControllerBase.GameSessionData.region, key);
		Value(allBtKey, btKey, value);
		if (doDetailedStatsForSession)
		{
			string allBtKey2 = string.Format(allBtFormatDetailedStats, MVGameControllerBase.GameSessionData.region, key);
			string btKey2 = string.Format(btFormatDetailedStats, MVGameControllerBase.GameSessionData.region, key);
			Value(allBtKey2, btKey2, value);
		}
	}

	private static void Count(string allBtKey, string btKey, int count)
	{
		Post.EzCounter(MVGameControllerBase.GameSessionData.ezKey, allBtKey, count);
		Post.EzCounter(MVGameControllerBase.GameSessionData.ezKey, btKey, count);
	}

	private static void Value(string allBtKey, string btKey, int value)
	{
		Post.EzValue(MVGameControllerBase.GameSessionData.ezKey, allBtKey, value);
		Post.EzValue(MVGameControllerBase.GameSessionData.ezKey, btKey, value);
	}

	private static void Value(string allBtKey, string btKey, float value)
	{
		Post.EzValue(MVGameControllerBase.GameSessionData.ezKey, allBtKey, value);
		Post.EzValue(MVGameControllerBase.GameSessionData.ezKey, btKey, value);
	}
}
