using System.Collections.Generic;

public class MVSpawnPointBlue : MVSpawnPoint
{
	private const string prefabPath = "Prefabs/SpawnPointObject";

	public MVSpawnPointBlue(Dictionary<object, object> data, Dictionary<int, MVWorldObjectClient> worldObjects)
		: base(data, "Prefabs/SpawnPointObject", worldObjects)
	{
	}
}
