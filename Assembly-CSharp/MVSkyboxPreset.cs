using System.Collections.Generic;
using UnityEngine;

public class MVSkyboxPreset : MVSkybox
{
	public MVSkyboxPreset(Dictionary<object, object> data, Dictionary<int, MVWorldObjectClient> worldObjects)
		: base(data, worldObjects)
	{
		if (Application.isEditor)
		{
			interactionFlags &= ~InteractionFlags.HasSettings;
		}
	}
}
