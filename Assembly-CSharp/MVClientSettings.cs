using MV.Common;

public static class MVClientSettings
{
	public static ClientSettingFlags ClientSettingFlags { get; set; }

	public static bool TouristChatAllowed => (ClientSettingFlags & ClientSettingFlags.TouristChatAllowed) == ClientSettingFlags.TouristChatAllowed;

	public static bool EnableSentry => (ClientSettingFlags & ClientSettingFlags.EnableClientSentry) == ClientSettingFlags.EnableClientSentry;

	public static bool ShowTouristPromotion => (ClientSettingFlags & ClientSettingFlags.ShowTouristPromotion) == ClientSettingFlags.ShowTouristPromotion;

	public static bool SpinEnabled => (ClientSettingFlags & ClientSettingFlags.SpinEnabled) == ClientSettingFlags.SpinEnabled;

	public static bool EnableStathat => (ClientSettingFlags & ClientSettingFlags.StathatEnabled) == ClientSettingFlags.StathatEnabled;
}
