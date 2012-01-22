using UnityEngine;

public static class HelperFunctions
{
	public static void SetLayerRecursively(Transform t, string layer)
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Expected Obj, but got Unknown
		((Component)t).gameObject.layer = LayerMask.NameToLayer(layer);
		foreach (Transform item in t)
		{
			Transform t2 = item;
			SetLayerRecursively(t2, layer);
		}
	}
}
