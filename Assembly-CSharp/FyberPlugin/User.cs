namespace FyberPlugin;

public class User
{
	protected static void NativePut(string json)
	{
		Utils.printWarningMessage();
	}

	protected static string GetJsonMessage(string key)
	{
		Utils.printWarningMessage();
		return "{\"success\":false,\"error\":\"Unsupported platform\":\"key\":" + key + "}";
	}

	protected static void NativeClearGdprConsentData()
	{
		Utils.printWarningMessage();
	}
}
