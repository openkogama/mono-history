using MV.Common;

public static class MVClientSettings
{
	private static ClientSettingFlags flags;

	private static bool flagsSet;

	public static ClientSettingFlags ClientSettingFlags
	{
		get
		{
			return flags;
		}
		set
		{
			flags = value;
			flagsSet = true;
		}
	}

	public static bool TouristChatAllowed => (ClientSettingFlags & ClientSettingFlags.TouristChatAllowed) == ClientSettingFlags.TouristChatAllowed;

	public static bool EnableSentry => (ClientSettingFlags & ClientSettingFlags.EnableClientSentry) == ClientSettingFlags.EnableClientSentry;

	public static bool ShowTouristPromotion => (ClientSettingFlags & ClientSettingFlags.ShowTouristPromotion) == ClientSettingFlags.ShowTouristPromotion;

	public static bool SpinEnabled => (ClientSettingFlags & ClientSettingFlags.SpinEnabled) == ClientSettingFlags.SpinEnabled;

	public static bool EnableStathat
	{
		get
		{
			if (!flagsSet)
			{
				return true;
			}
			return (ClientSettingFlags & ClientSettingFlags.StathatEnabled) == ClientSettingFlags.StathatEnabled;
		}
	}
}
