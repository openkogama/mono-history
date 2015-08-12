using System.Collections.Generic;

public class MVSpawnPointRed : MVSpawnPoint
{
	private const string prefabPath = "Prefabs/SpawnPointRedObject";

	public MVSpawnPointRed(Dictionary<object, object> data, Dictionary<int, MVWorldObjectClient> worldObjects)
		: base(data, "Prefabs/SpawnPointRedObject", worldObjects)
	{
	}
}
