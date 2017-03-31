using System.Collections.Generic;

public class MVSpawnPointYellow : MVSpawnPoint
{
	public override MVWorldObjectDocumentationType DocumentationType => MVWorldObjectDocumentationType.SpawnPointYellow;

	public MVSpawnPointYellow(Dictionary<object, object> data, Dictionary<int, MVWorldObjectClient> worldObjects)
		: base(data, PrefabPool.Instance.MVSpawnPointYellowPrefab, worldObjects)
	{
	}
}
