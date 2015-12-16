using System.Collections.Generic;

public class MVSpawnPointYellow : MVSpawnPoint
{
	public MVSpawnPointYellow(Dictionary<object, object> data, Dictionary<int, MVWorldObjectClient> worldObjects)
		: base(data, PrefabPool.Instance.MVSpawnPointYellowPrefab, worldObjects)
	{
	}
}
