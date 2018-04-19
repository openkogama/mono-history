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
	LowResClouds = 0x40,
	LowResStars = 0x80,
	CloudLayers_1 = 0x100,
	CloudLayers_3 = 0x200
}
