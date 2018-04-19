using MV.Common;
using UnityEngine;

public class TextureSelector : MonoBehaviour
{
	private Material material;

	[SerializeField]
	private Material material_5;

	[SerializeField]
	private Material material_3;

	[SerializeField]
	private Material material_1;

	[SerializeField]
	private Texture clouds_lowRes;

	[SerializeField]
	private Texture clouds_hiRes;

	[SerializeField]
	private Texture stars_lowRes;

	[SerializeField]
	private Texture stars_hiRes;

	public Material SetTexturesAndGetMaterial()
	{
		if ((MVClientSettings.ClientSettingFlags & ClientSettingFlags.CloudLayers_1) > ClientSettingFlags.None)
		{
			material = material_1;
		}
		else if ((MVClientSettings.ClientSettingFlags & ClientSettingFlags.CloudLayers_3) > ClientSettingFlags.None)
		{
			material = material_3;
		}
		else
		{
			material = material_5;
		}
		if ((MVClientSettings.ClientSettingFlags & ClientSettingFlags.LowResClouds) > ClientSettingFlags.None)
		{
			material.SetTexture("_CloudsTex", clouds_lowRes);
		}
		else
		{
			material.SetTexture("_CloudsTex", clouds_hiRes);
		}
		if ((MVClientSettings.ClientSettingFlags & ClientSettingFlags.LowResStars) > ClientSettingFlags.None)
		{
			material.SetTexture("_StarsTex", stars_lowRes);
		}
		else
		{
			material.SetTexture("_StarsTex", stars_hiRes);
		}
		return material;
	}
}
