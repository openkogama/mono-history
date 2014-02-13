using System;

[Flags]
public enum UpdateCondition
{
	ALLWAYS = 1,
	INGAME = 2,
	EDITOR = 4,
	EDITOR_PLAYMODE = 8
}
