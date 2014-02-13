using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MVPointLightPreset : MVPointLight
{
	public MVPointLightPreset(Hashtable data, Dictionary<int, MVWorldObjectClient> worldObjects)
		: base(data, worldObjects)
	{
		if (!Application.isEditor && !Debug.isDebugBuild)
		{
			interactionFlags &= ~InteractionFlags.HasSettings;
		}
	}

	public override void Initialize()
	{
		if (!Data.ContainsKey("color"))
		{
			Debug.LogError((object)"Inserted Preset Waterplane without any color data");
		}
		base.Initialize();
	}
}
