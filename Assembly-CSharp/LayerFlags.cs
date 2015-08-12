using System;

[Flags]
public enum LayerFlags
{
	Default = 1,
	TransparentFX = 2,
	IgnoreRaycast = 4,
	Water = 0x10,
	CamRotateTarget = 0x100,
	Selected = 0x200,
	GUI = 0x400,
	Logic = 0x800,
	Player = 0x1000,
	Preview = 0x2000,
	LogicSelected = 0x4000,
	UIItems = 0x8000,
	UXElement = 0x10000,
	HudObjects = 0x20000,
	Hidden = 0x40000,
	PlayerSelected = 0x80000,
	UXElementSecondary = 0x100000
}
