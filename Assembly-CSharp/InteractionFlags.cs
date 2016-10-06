using System;

[Flags]
public enum InteractionFlags
{
	None = 0,
	Selectable = 1,
	HasCubeModel = 2,
	IsTerrain = 4,
	DirectlySelectable = 8,
	SelectionRequiresEditGroup = 0x10,
	NotUserTransformable = 0x20,
	DontPushGroupToSelectionStack = 0x40,
	CanRotateX = 0x80,
	CanRotateY = 0x100,
	CanRotateZ = 0x200,
	NotTranslatbleY = 0x400,
	NotTranslatbleXZ = 0x800,
	CanEdit = 0x1000,
	CanClone = 0x2000,
	CanAddToInventory = 0x4000,
	HasSettings = 0x8000,
	CanResetLogic = 0x10000,
	IsPreview = 0x20000,
	IsUsable = 0x40000,
	CantAddChildren = 0x80000,
	CanUseGameCoins = 0x100000,
	CanUseLevel = 0x200000,
	CanUseStars = 0x400000,
	TranslatbleXZ2D = 0x800000,
	Sounds = 0x1000000,
	CanUseTeam = 0x2000000
}
