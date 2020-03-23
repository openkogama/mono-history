using System;

[Flags]
public enum UIPushOption
{
	None = 0,
	Blocking = 1,
	HideAll = 2,
	InvisibleBlocker = 4,
	HideAllExceptStackBottom = 8,
	SuppressInput = 0x10
}
