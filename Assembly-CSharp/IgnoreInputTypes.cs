using System;

[Flags]
public enum IgnoreInputTypes
{
	None = 0,
	MouseMovement = 1,
	MouseLeft = 2,
	MouseRight = 4,
	MouseScroll = 8,
	Keys = 0x10,
	Avatar = 0x20,
	AvatarIgnoreY = 0x40
}
