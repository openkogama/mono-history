using System.Collections.Generic;

public class MVSpawnPointYellow : MVSpawnPoint
{
	private const string prefabPath = "Prefabs/SpawnPointYellowObject";

	public MVSpawnPointYellow(Dictionary<object, object> data, Dictionary<int, MVWorldObjectClient> worldObjects)
		: base(data, "Prefabs/SpawnPointYellowObject", worldObjects)
	{
	}
}
