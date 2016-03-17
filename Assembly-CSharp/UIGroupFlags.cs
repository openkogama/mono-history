using System;

[Flags]
public enum UIGroupFlags
{
	None = 0,
	Default = 1,
	GameObjectUI = 2,
	InventoryUI = 4,
	InventoryUISubMenu = 8,
	GameObjectUISubMenu = 0x10,
	Popup = 0x20
}
