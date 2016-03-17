using System.Collections.Generic;
using UnityEngine;

internal interface IEditModeObjectPicker
{
	bool Pick(ref VoxelHit hit, HashSet<int> ignoreWoIds = null, int layerMask = -5);

	bool DrawPlanePick(ref Vector3 hit);
}
