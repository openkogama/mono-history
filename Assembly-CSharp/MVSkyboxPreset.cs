using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MVSkyboxPreset : MVSkybox
{
	public MVSkyboxPreset(Hashtable data, Dictionary<int, MVWorldObjectClient> worldObjects)
		: base(data, worldObjects)
	{
		if (!Application.isEditor && !Debug.isDebugBuild)
		{
			interactionFlags &= ~InteractionFlags.HasSettings;
		}
	}
}
