using System;

namespace MV.WorldObject;

[Flags]
public enum FaceFlags : byte
{
	Top = 1,
	Bottom = 2,
	Front = 4,
	Back = 8,
	Left = 0x10,
	Right = 0x20
}
