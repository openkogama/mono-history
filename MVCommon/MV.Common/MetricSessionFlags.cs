using System;

namespace MV.Common;

[Flags]
public enum MetricSessionFlags : short
{
	None = 0,
	IsTourist = 1,
	IsRegistered = 2,
	IsOnSite = 4,
	IsOffSite = 8,
	IsFirstTimeSession = 0x10,
	IsReturningSession = 0x20,
	Android = 0x40,
	WebGL = 0x80,
	StandAlone = 0x100,
	IOS = 0x200
}
