using System.Collections;
using System.Collections.Generic;

public class MVSpawnPointYellow : MVSpawnPoint
{
	private const string prefabPath = "Prefabs/SpawnPointYellowObject";

	public MVSpawnPointYellow(Hashtable data, Dictionary<int, MVWorldObjectClient> worldObjects)
		: base(data, "Prefabs/SpawnPointYellowObject", worldObjects)
	{
	}
}
