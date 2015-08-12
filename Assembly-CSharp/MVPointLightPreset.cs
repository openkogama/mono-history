using System.Collections.Generic;
using UnityEngine;

public class MVPointLightPreset : MVPointLight
{
	public MVPointLightPreset(Dictionary<object, object> data, Dictionary<int, MVWorldObjectClient> worldObjects)
		: base(data, worldObjects)
	{
		if (Application.isEditor)
		{
			interactionFlags &= ~InteractionFlags.HasSettings;
		}
	}

	public override void Initialize()
	{
		if (!Data.ContainsKey("color"))
		{
			Debug.LogError("Inserted Preset Waterplane without any color data");
		}
		base.Initialize();
	}
}
