using System;

[Flags]
public enum InteractionFlags
{
	None = 0,
	Selectable = 1,
	HasCubeModel = 2,
	IsTerrain = 4
}
