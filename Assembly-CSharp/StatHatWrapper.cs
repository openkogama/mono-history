using StatHat;

public static class StatHatWrapper
{
	private static bool doDetailedStatsForSession;

	private const string allBtFormat = "{0}.u.{1}";

	private const string allBtFormatDetailedStats = "{0}.u.detailedstat.{1}";

	private const string btFormat = "{0}.u.standalone.{1}";

	private const string btFormatDetailedStats = "{0}.u.detailedstat.standalone.{1}";

	private static bool StathatReportingEnabled => MVClientSettings.EnableStathat;

	public static void Initialize(bool doDetailedStatsForSession)
	{
		StatHatWrapper.doDetailedStatsForSession = doDetailedStatsForSession;
	}

	public static void Count(string key, int count)
	{
		if (StathatReportingEnabled)
		{
			string allBtKey = $"{MVGameControllerBase.GameSessionData.region}.u.{key}";
			string btKey = $"{MVGameControllerBase.GameSessionData.region}.u.standalone.{key}";
			Count(allBtKey, btKey, count);
			if (doDetailedStatsForSession)
			{
				string allBtKey2 = $"{MVGameControllerBase.GameSessionData.region}.u.detailedstat.{key}";
				string btKey2 = $"{MVGameControllerBase.GameSessionData.region}.u.detailedstat.standalone.{key}";
				Count(allBtKey2, btKey2, count);
			}
		}
	}

	public static void Value(string key, int value)
	{
		if (StathatReportingEnabled)
		{
			string allBtKey = $"{MVGameControllerBase.GameSessionData.region}.u.{key}";
			string btKey = $"{MVGameControllerBase.GameSessionData.region}.u.standalone.{key}";
			Value(allBtKey, btKey, value);
			if (doDetailedStatsForSession)
			{
				string allBtKey2 = $"{MVGameControllerBase.GameSessionData.region}.u.detailedstat.{key}";
				string btKey2 = $"{MVGameControllerBase.GameSessionData.region}.u.detailedstat.standalone.{key}";
				Value(allBtKey2, btKey2, value);
			}
		}
	}

	public static void Value(string key, float value)
	{
		if (StathatReportingEnabled)
		{
			string allBtKey = $"{MVGameControllerBase.GameSessionData.region}.u.{key}";
			string btKey = $"{MVGameControllerBase.GameSessionData.region}.u.standalone.{key}";
			Value(allBtKey, btKey, value);
			if (doDetailedStatsForSession)
			{
				string allBtKey2 = $"{MVGameControllerBase.GameSessionData.region}.u.detailedstat.{key}";
				string btKey2 = $"{MVGameControllerBase.GameSessionData.region}.u.detailedstat.standalone.{key}";
				Value(allBtKey2, btKey2, value);
			}
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
