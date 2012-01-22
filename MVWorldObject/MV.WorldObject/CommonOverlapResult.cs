using System.Collections.Generic;

namespace MV.WorldObject;

public struct CommonOverlapResult(int woId, List<OverlapCubeData> cubes)
{
	public int woId = woId;

	public List<OverlapCubeData> cubes = cubes;
}
