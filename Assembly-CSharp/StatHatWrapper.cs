using StatHat;

public static class StatHatWrapper
{
	private static string btFormat = "{0}.u.standalone.{1}";

	public static void Count(string key, int count)
	{
		string stat = $"{MVGameController.GameSessionData.region}.u.{key}";
		string stat2 = string.Format(btFormat, MVGameController.GameSessionData.region, key);
		Post.EzCounter(MVGameController.GameSessionData.ezKey, stat, count);
		Post.EzCounter(MVGameController.GameSessionData.ezKey, stat2, count);
	}

	public static void Value(string key, int value)
	{
		string stat = $"{MVGameController.GameSessionData.region}.u.{key}";
		string stat2 = string.Format(btFormat, MVGameController.GameSessionData.region, key);
		Post.EzValue(MVGameController.GameSessionData.ezKey, stat, value);
		Post.EzValue(MVGameController.GameSessionData.ezKey, stat2, value);
	}

	public static void Value(string key, float value)
	{
		string stat = $"{MVGameController.GameSessionData.region}.u.{key}";
		string stat2 = string.Format(btFormat, MVGameController.GameSessionData.region, key);
		Post.EzValue(MVGameController.GameSessionData.ezKey, stat, value);
		Post.EzValue(MVGameController.GameSessionData.ezKey, stat2, value);
	}
}
