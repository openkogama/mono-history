using StatHat;

public static class StatHatWrapper
{
	private static string btFormat = "{0}.u.standalone.{1}";

	public static void Count(string key, int count)
	{
		string stat = $"{MVGameControllerBase.GameSessionData.region}.u.{key}";
		string stat2 = string.Format(btFormat, MVGameControllerBase.GameSessionData.region, key);
		Post.EzCounter(MVGameControllerBase.GameSessionData.ezKey, stat, count);
		Post.EzCounter(MVGameControllerBase.GameSessionData.ezKey, stat2, count);
	}

	public static void Value(string key, int value)
	{
		string stat = $"{MVGameControllerBase.GameSessionData.region}.u.{key}";
		string stat2 = string.Format(btFormat, MVGameControllerBase.GameSessionData.region, key);
		Post.EzValue(MVGameControllerBase.GameSessionData.ezKey, stat, value);
		Post.EzValue(MVGameControllerBase.GameSessionData.ezKey, stat2, value);
	}

	public static void Value(string key, float value)
	{
		string stat = $"{MVGameControllerBase.GameSessionData.region}.u.{key}";
		string stat2 = string.Format(btFormat, MVGameControllerBase.GameSessionData.region, key);
		Post.EzValue(MVGameControllerBase.GameSessionData.ezKey, stat, value);
		Post.EzValue(MVGameControllerBase.GameSessionData.ezKey, stat2, value);
	}
}
