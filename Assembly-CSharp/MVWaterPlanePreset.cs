using System.Collections.Generic;
using UnityEngine;

public class MVWaterPlanePreset : MVWaterPlane
{
	public MVWaterPlanePreset(Dictionary<object, object> data, Dictionary<int, MVWorldObjectClient> worldObjects)
		: base(data, worldObjects)
	{
		if (Application.isEditor)
		{
			interactionFlags &= ~(InteractionFlags.CanClone | InteractionFlags.HasSettings);
		}
	}

	public override void Initialize()
	{
		if (!Data.ContainsKey("waterColor"))
		{
			Debug.LogError("Inserted Preset Waterplane without any color data");
		}
		base.Initialize();
	}
}
