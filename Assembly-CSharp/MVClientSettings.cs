using MV.Common;
using MV.WorldObject.Subscription;

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

	public static int PostGameInterstitialIntervalInSeconds { get; set; }

	public static int ReviveFlags { get; set; }

	public static bool TouristChatAllowed => (ClientSettingFlags & ClientSettingFlags.TouristChatAllowed) == ClientSettingFlags.TouristChatAllowed;

	public static bool EnableSentry => (ClientSettingFlags & ClientSettingFlags.EnableClientSentry) == ClientSettingFlags.EnableClientSentry;

	public static bool ShowTouristPromotion => (ClientSettingFlags & ClientSettingFlags.ShowTouristPromotion) == ClientSettingFlags.ShowTouristPromotion;

	public static bool SpinEnabled => (ClientSettingFlags & ClientSettingFlags.SpinEnabled) == ClientSettingFlags.SpinEnabled;

	public static bool PostGameInterstitialEnabled => IsFlagSet(ClientSettingFlags.PostGameInterstitialEnabled);

	public static bool SeekAdConsent => IsFlagSet(ClientSettingFlags.SeekAdConsent);

	public static bool JoinFlowAdsEnabled => IsFlagSet(ClientSettingFlags.JoinFlowAdsEnabled) && InterstitialsAdsEnabled;

	public static bool WebAdSDKsEnabled => IsFlagSet(ClientSettingFlags.WebAdSDKSEnabled);

	public static bool GameDistributionAdsEnabled => IsFlagSet(ClientSettingFlags.GameDistributionAdSDKEnabled);

	public static bool PokiAdsEnabled => IsFlagSet(ClientSettingFlags.PokiAdsEnabled);

	public static bool PlayButtonAdsRegisteredUsers => IsFlagSet(ClientSettingFlags.PlayButtonAdsRegisteredUsers);

	public static bool FirstPreviewTierFreeEnabled => IsFlagSet(ClientSettingFlags.FirstPreviewTierFreeEnabled);

	public static bool ReviveEnabled => IsReviveFlagEnabledForSessionType() && RewardedAdsEnabled && GameSetupOptions.IsReviveEnabled;

	public static bool PlayButtonAdsEnabled => false;

	public static bool BoostersEnabled => false;

	public static bool InterstitialsAdsEnabled => false;

	public static bool RewardedAdsEnabled => false;

	public static bool EnableStathat
	{
		get
		{
			bool flag = (ClientSettingFlags & ClientSettingFlags.StathatEnabled) == ClientSettingFlags.StathatEnabled;
			return !flagsSet || flag;
		}
	}

	public static bool IsFlagSet(ClientSettingFlags flag)
	{
		return (ClientSettingFlags & flag) == flag;
	}

	private static bool IsReviveFlagEnabledForSessionType()
	{
		if (MVGameControllerBase.IsTouristSession)
		{
		}
		return false;
	}

	public static bool IsSubscriber()
	{
		return MVGameControllerBase.Game.LocalPlayer.SubscriptionRules.SubscriptionType != SubscriptionType.None;
	}
}
