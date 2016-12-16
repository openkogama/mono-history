using System;

namespace MV.Common;

[Flags]
public enum ClientSettingFlags
{
	None = 0,
	TouristChatAllowed = 1,
	ShowErrorPopupClient = 2,
	EnableClientSentry = 4,
	ShowTouristPromotion = 8,
	ShowTouristAd = 0x10,
	SpinEnabled = 0x20
}
