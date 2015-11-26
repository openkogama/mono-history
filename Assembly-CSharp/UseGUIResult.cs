using System;

[Flags]
public enum UseGUIResult
{
	NoUseButton = 1,
	NoCost = 2,
	CanAfford = 4,
	CannotAfford = 8
}
