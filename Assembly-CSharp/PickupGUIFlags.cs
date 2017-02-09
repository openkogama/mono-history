using System;

[Flags]
public enum PickupGUIFlags
{
	None = 0,
	CanFire = 1,
	CanUnequip = 2,
	ShowCrosshair = 4,
	CanHolster = 8,
	IsHolstered = 0x10
}
