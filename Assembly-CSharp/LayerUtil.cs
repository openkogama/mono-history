using UnityEngine;

public static class LayerUtil
{
	public static string GetName(LayerFlags layers)
	{
		return LayerMask.LayerToName((int)layers);
	}

	public static int GetLayerNumber(LayerFlags layer)
	{
		int num = (int)layer;
		if (num <= 0)
		{
			Debug.LogError((object)("layer parameter constant should be bigger that 0, instead got " + num));
			return 0;
		}
		int num2 = 0;
		while ((1 & num) != 1)
		{
			num2++;
			num >>= 1;
		}
		return num2;
	}

	public static LayerMask GetMask(LayerFlags layerFlags)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		return LayerMask.op_Implicit((int)layerFlags);
	}

	public static bool HasFlags(LayerFlags layersMask, int layerFlags)
	{
		return HasFlags(layersMask, layerFlags);
	}

	public static bool HasFlags(int layersMask, LayerFlags layerFlags)
	{
		return HasFlags(layersMask, layerFlags);
	}

	public static bool HasFlags(LayerFlags layersMask, LayerFlags layerFlags)
	{
		return HasFlags(layersMask, layerFlags);
	}

	public static bool HasFlags(int layersMask, int layerFlags)
	{
		return (layersMask & layerFlags) == layerFlags;
	}

	public static void SetLayerRecursively(this Transform transfrom, int layer)
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Expected Obj, but got Unknown
		((Component)transfrom).gameObject.layer = layer;
		foreach (Transform item in transfrom)
		{
			Transform transfrom2 = item;
			transfrom2.SetLayerRecursively(layer);
		}
	}

	public static void SetLayerRecursively(Transform transfrom, string layer)
	{
		transfrom.SetLayerRecursively(LayerMask.NameToLayer(layer));
	}

	public static void SetLayerRecursively(Transform transfrom, LayerMask layersToChange, int layer)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Expected Obj, but got Unknown
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		int layerFlags = 1 << ((Component)transfrom).gameObject.layer;
		if (HasFlags(LayerMask.op_Implicit(layersToChange), layerFlags))
		{
			((Component)transfrom).gameObject.layer = layer;
		}
		foreach (Transform item in transfrom)
		{
			Transform transfrom2 = item;
			SetLayerRecursively(transfrom2, layersToChange, layer);
		}
	}

	public static void SetLayerRecursively(Transform transfrom, string layerToChange, string layer)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		int num = 1 << LayerMask.NameToLayer(layerToChange);
		int layer2 = LayerMask.NameToLayer(layer);
		SetLayerRecursively(transfrom, LayerMask.op_Implicit(num), layer2);
	}

	public static void SetLayerRecursively(this GameObject gameObject, int layer)
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Expected Obj, but got Unknown
		gameObject.layer = layer;
		foreach (Transform item in gameObject.transform)
		{
			Transform transfrom = item;
			transfrom.SetLayerRecursively(layer);
		}
	}
}
