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
			Debug.LogError("layer parameter constant should be bigger that 0, instead got " + num);
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
		return (int)layerFlags;
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
		transfrom.gameObject.layer = layer;
		foreach (Transform item in transfrom)
		{
			item.SetLayerRecursively(layer);
		}
	}

	public static void SetLayerRecursively(Transform transfrom, string layer)
	{
		transfrom.SetLayerRecursively(LayerMask.NameToLayer(layer));
	}

	public static void SetLayerRecursively(Transform transfrom, LayerMask layersToChange, int layer)
	{
		int layerFlags = 1 << transfrom.gameObject.layer;
		if (HasFlags(layersToChange, layerFlags))
		{
			transfrom.gameObject.layer = layer;
		}
		foreach (Transform item in transfrom)
		{
			SetLayerRecursively(item, layersToChange, layer);
		}
	}

	public static void SetLayerRecursively(Transform transfrom, string layerToChange, string layer)
	{
		int num = 1 << LayerMask.NameToLayer(layerToChange);
		int layer2 = LayerMask.NameToLayer(layer);
		SetLayerRecursively(transfrom, num, layer2);
	}

	public static void SetLayerRecursively(this GameObject gameObject, int layer)
	{
		gameObject.layer = layer;
		foreach (Transform item in gameObject.transform)
		{
			item.SetLayerRecursively(layer);
		}
	}
}
