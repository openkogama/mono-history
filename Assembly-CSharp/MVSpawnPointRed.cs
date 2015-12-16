using System.Collections.Generic;

public class MVSpawnPointRed : MVSpawnPoint
{
	public MVSpawnPointRed(Dictionary<object, object> data, Dictionary<int, MVWorldObjectClient> worldObjects)
		: base(data, PrefabPool.Instance.MVSpawnPointRedPrefab, worldObjects)
	{
	}
}
