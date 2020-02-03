using System;

namespace MV.Common;

[Flags]
public enum ClientSettingFlags
{
	None = 0,
	TouristChatAllowed = 1,
	EnableClientSentry = 2,
	ShowTouristPromotion = 4,
	ShowTouristAd = 8,
	SpinEnabled = 0x10,
	StathatEnabled = 0x20,
	InterstitalAdsEnabledWebGL = 0x40,
	RewardedAdsEnabledWebGL = 0x80,
	GamePassSilentReleaseEnabled = 0x100,
	PostGameInterstitialEnabled = 0x200,
	BoostersEnabledWebGL = 0x400,
	BoostersEnabledAndroid = 0x800,
	SeekAdConsent = 0x1000,
	JoinFlowAdsEnabled = 0x2000,
	InterstitalAdsEnabledAndroid = 0x4000,
	RewardedAdsEnabledAndroid = 0x8000,
	WebAdSDKSEnabled = 0x10000,
	PlayButtonAdsRegisteredUsers = 0x20000,
	GameDistributionAdSDKEnabled = 0x40000,
	PokiAdsEnabled = 0x80000,
	PlayButtonAdsEnabled = 0x100000,
	PlayButtonAdsEnabledMobile = 0x200000,
	FirstPreviewTierFreeEnabled = 0x400000
}
