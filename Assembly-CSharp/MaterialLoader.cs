using System;
using UnityEngine;

public class MaterialLoader : MonoBehaviour
{
	[SerializeField]
	private Material cubeModelMaterialHigh;

	[SerializeField]
	private Material cubeModelMaterialLow;

	[SerializeField]
	private Material avatarMaterialHigh;

	[SerializeField]
	private Material avatarTransparentMaterialHigh;

	[SerializeField]
	private Material avatarMaterialLow;

	[SerializeField]
	private Material avatarTransparentMaterialLow;

	[SerializeField]
	private Shader avatarHigh;

	[SerializeField]
	private Shader avatarLow;

	[SerializeField]
	private Shader avatarTransparentHigh;

	[SerializeField]
	private Shader avatarTransparentLow;

	private Material cubeModelMaterial;

	private Material avatarMaterial;

	private Material avatarTransparentMaterial;

	private Shader avatarShader;

	private Shader avatarTransparentShader;

	public Material CubeModelMaterial => cubeModelMaterial;

	public Material AvatarMaterial => avatarMaterial;

	public Material AvatarTransparentMaterial => avatarTransparentMaterial;

	public Shader AvatarShader => avatarShader;

	public Shader AvatarTransparentShader => avatarTransparentShader;

	public void Start()
	{
		bool flag = (SystemInfo.graphicsShaderLevel >= 30 && !SystemInfo.graphicsDeviceVersion.Contains("ES 2.0")) || SystemInfo.graphicsDeviceVersion.Contains("WebGL");
		if (cubeModelMaterialHigh == null || cubeModelMaterialLow == null)
		{
			throw new NullReferenceException();
		}
		if (avatarMaterialHigh == null || avatarMaterialLow == null)
		{
			throw new NullReferenceException();
		}
		if (avatarTransparentMaterialHigh == null || avatarTransparentMaterialLow == null)
		{
			throw new NullReferenceException();
		}
		cubeModelMaterial = cubeModelMaterialLow;
		avatarMaterial = avatarMaterialLow;
		avatarTransparentMaterial = avatarTransparentMaterialLow;
		avatarShader = avatarLow;
		avatarTransparentShader = avatarTransparentLow;
		if (flag)
		{
			cubeModelMaterial = cubeModelMaterialHigh;
			avatarMaterial = avatarMaterialHigh;
			avatarTransparentMaterial = avatarTransparentMaterialHigh;
			avatarShader = avatarHigh;
			avatarTransparentShader = avatarTransparentHigh;
		}
		InitAllMaterials(flag);
		MeshPool.Instance.MaxAmtMeshes = 100;
	}

	private void InitAllMaterials(bool useSM3)
	{
		int num = Mathf.CeilToInt((float)cubeModelMaterial.mainTexture.width * (TextureAtlas.UV[0].width + 1f / (float)cubeModelMaterial.mainTexture.width));
		int num2 = Mathf.CeilToInt(Mathf.Log(num, 2f));
		cubeModelMaterial.SetVector("_MaterialSize", new Vector4(TextureAtlas.UV[0].width, TextureAtlas.UV[0].height, num, num2));
		cubeModelMaterial.mainTexture.filterMode = FilterMode.Point;
		cubeModelMaterial.mainTexture.anisoLevel = 1;
		avatarMaterial.SetVector("_MaterialSize", new Vector4(TextureAtlas.UV[0].width, TextureAtlas.UV[0].height, num, num2));
		avatarMaterial.mainTexture.filterMode = FilterMode.Point;
		avatarMaterial.mainTexture.anisoLevel = 1;
		avatarTransparentMaterial.SetVector("_MaterialSize", new Vector4(TextureAtlas.UV[0].width, TextureAtlas.UV[0].height, num, num2));
		avatarTransparentMaterial.mainTexture.filterMode = FilterMode.Point;
		avatarTransparentMaterial.mainTexture.anisoLevel = 1;
		if (useSM3)
		{
			cubeModelMaterial.SetVector("_MaterialSize", new Vector4(TextureAtlas.UV[0].width, TextureAtlas.UV[0].height, num, num2));
			cubeModelMaterial.mainTexture.filterMode = FilterMode.Bilinear;
			cubeModelMaterial.mainTexture.anisoLevel = 2;
			avatarMaterial.SetVector("_MaterialSize", new Vector4(TextureAtlas.UV[0].width, TextureAtlas.UV[0].height, num, num2));
			avatarMaterial.mainTexture.filterMode = FilterMode.Bilinear;
			avatarMaterial.mainTexture.anisoLevel = 2;
			avatarTransparentMaterial.SetVector("_MaterialSize", new Vector4(TextureAtlas.UV[0].width, TextureAtlas.UV[0].height, num, num2));
			avatarTransparentMaterial.mainTexture.filterMode = FilterMode.Bilinear;
			avatarTransparentMaterial.mainTexture.anisoLevel = 2;
		}
	}
}
