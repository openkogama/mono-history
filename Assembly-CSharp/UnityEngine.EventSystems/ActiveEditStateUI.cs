using System;

namespace UnityEngine.EventSystems;

[Flags]
public enum ActiveEditStateUI
{
	None = 0,
	CubeModelTools = 1,
	AvatarManagement = 2,
	Third = 4,
	Fourth = 8
}
