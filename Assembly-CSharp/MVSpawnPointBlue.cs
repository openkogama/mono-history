using System.Collections.Generic;

public class MVSpawnPointBlue : MVSpawnPoint
{
	public override MVWorldObjectDocumentationType DocumentationType => MVWorldObjectDocumentationType.SpawnPointBlue;

	public MVSpawnPointBlue(Dictionary<object, object> data, Dictionary<int, MVWorldObjectClient> worldObjects)
		: base(data, PrefabPool.Instance.MVSpawnPointBluePrefab, worldObjects)
	{
	}
}
