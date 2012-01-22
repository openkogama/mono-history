using System;

namespace MV.Common;

[Flags]
public enum PropertyType : byte
{
	None = 0,
	Game = 1,
	Actor = 2,
	GameAndActor = Game | Actor
}
