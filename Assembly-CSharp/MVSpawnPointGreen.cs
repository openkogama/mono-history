using System.Collections.Generic;

public class MVSpawnPointGreen : MVSpawnPoint
{
	public MVSpawnPointGreen(Dictionary<object, object> data, Dictionary<int, MVWorldObjectClient> worldObjects)
		: base(data, PrefabPool.Instance.MVSpawnPointGreenPrefab, worldObjects)
	{
	}
}
