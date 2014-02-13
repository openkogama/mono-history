using System;

namespace MV.Common;

[Flags]
public enum WorldObjectOwnerState : byte
{
	None = 0,
	OwnerActorID = 1,
	PreviewOwnerActorID = 2
}
