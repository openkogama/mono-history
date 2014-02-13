using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MVWaterPlanePreset : MVWaterPlane
{
	public MVWaterPlanePreset(Hashtable data, Dictionary<int, MVWorldObjectClient> worldObjects)
		: base(data, worldObjects)
	{
		if (!Application.isEditor && !Debug.isDebugBuild)
		{
			interactionFlags &= ~(InteractionFlags.CanClone | InteractionFlags.HasSettings);
		}
	}

	public override void Initialize()
	{
		if (!Data.ContainsKey("waterColor"))
		{
			Debug.LogError((object)"Inserted Preset Waterplane without any color data");
		}
		base.Initialize();
	}
}
