using System.Collections.Generic;

public class MVSpawnPointGreen : MVSpawnPoint
{
	private const string prefabPath = "Prefabs/SpawnPointGreenObject";

	public MVSpawnPointGreen(Dictionary<object, object> data, Dictionary<int, MVWorldObjectClient> worldObjects)
		: base(data, "Prefabs/SpawnPointGreenObject", worldObjects)
	{
	}
}
