using System;

namespace MV.Common;

[Flags]
public enum JoinSessionFlags
{
	None = 0,
	IsOnSite = 1,
	IsFirstTimeSession = 2,
	IsReturning = 4,
	IsReturningAsSignedUp = 8
}
