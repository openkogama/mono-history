using StatHat;

public static class StatHatWrapper
{
	private static bool useStatHatPost = true;

	public static void Count(string key, int count)
	{
		if (useStatHatPost)
		{
			Post.EzCounter(MVGameController.GameSessionData.ezKey, $"{MVGameController.GameSessionData.region}.u.{key}", count);
			return;
		}
		BrowserComm.ToJavaScript.ExternalCall("sendStatHatCount", key, count);
	}

	public static void Value(string key, int value)
	{
		if (useStatHatPost)
		{
			Post.EzValue(MVGameController.GameSessionData.ezKey, $"{MVGameController.GameSessionData.region}.u.{key}", value);
			return;
		}
		BrowserComm.ToJavaScript.ExternalCall("sendStatHatValue", key, value);
	}

	public static void Value(string key, float value)
	{
		if (useStatHatPost)
		{
			Post.EzValue(MVGameController.GameSessionData.ezKey, $"{MVGameController.GameSessionData.region}.u.{key}", value);
			return;
		}
		BrowserComm.ToJavaScript.ExternalCall("sendStatHatValue", key, value);
	}
}
