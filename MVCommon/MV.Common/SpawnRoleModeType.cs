using System;

namespace MV.Common;

[Flags]
public enum SpawnRoleModeType
{
	None = 0,
	Playing = 1,
	Dead = 2,
	Hidden = 4
}
