using System;

[Flags]
public enum UpdateItemState
{
	None = 0,
	Holster = 1,
	Unholster = 2,
	ResetAmmo = 4
}
