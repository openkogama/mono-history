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
	InterstitalAdsEnabled = 0x40,
	RewardedAdsEnabled = 0x80,
	GamePassSilentReleaseEnabled = 0x100,
	PostGameInterstitialEnabled = 0x200,
	BoostersEnabledWebGL = 0x400,
	BoostersEnabledAndroid = 0x800,
	SeekAdConsent = 0x1000
}
