using System.Collections.Generic;

public struct WorldObjectsIdsLodBookkeeping(List<int> worldObjectsIdsLod)
{
	public int currentPosition = 0;

	public List<int> worldObjectsIdsLod = worldObjectsIdsLod;

	public MVWorldObjectClient currentWorldObject = null;
}
