using System;

namespace MV.WorldObject;

[Flags]
public enum CubeCompression : byte
{
	None = 0,
	IdentityCorners = 1,
	SingleMaterial = 2
}
