using System;
using MV.Common;
using UnityEngine;
using UnityEngine.Rendering;

public class MaterialLoader : MonoBehaviour
{
	private static string highResAtlasFileName = "Atlas/atlas512.unity3d";

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

	[SerializeField]
	private Shader pickupItemShader;

	[SerializeField]
	private Shader wireframeShader;

	[SerializeField]
	private Shader defaultDiffuseShader;

	[SerializeField]
	private Texture2D lowResMaterials;

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

	public Shader PickupItemShader => pickupItemShader;

	public Shader WireframeShader => wireframeShader;

	public Shader DefaultDiffuseShader => defaultDiffuseShader;

	public void Start()
	{
		SetMainTexture(lowResMaterials);
		SetupMaterials();
		MeshPool.Instance.MaxAmtMeshes = 100;
	}

	private void SetMainTexture(Texture texture)
	{
		Material material = cubeModelMaterialHigh;
		Texture texture2 = texture;
		avatarMaterialLow.mainTexture = texture2;
		texture2 = texture2;
		avatarTransparentMaterialHigh.mainTexture = texture2;
		texture2 = texture2;
		avatarMaterialLow.mainTexture = texture2;
		texture2 = texture2;
		avatarMaterialHigh.mainTexture = texture2;
		texture2 = texture2;
		cubeModelMaterialLow.mainTexture = texture2;
		material.mainTexture = texture2;
	}

	private void SetupMaterials()
	{
		bool flag = false;
		if (SystemInfo.graphicsShaderLevel >= 30 && (SystemInfo.graphicsDeviceType == GraphicsDeviceType.Direct3D9 || SystemInfo.graphicsDeviceType == GraphicsDeviceType.Direct3D12 || SystemInfo.graphicsDeviceType == GraphicsDeviceType.Direct3D11))
		{
			flag = true;
		}
		if (SystemInfo.graphicsDeviceType == GraphicsDeviceType.OpenGLES3)
		{
			flag = true;
		}
		if (SystemInfo.graphicsDeviceType == GraphicsDeviceType.OpenGLES2 || SystemInfo.graphicsDeviceType == GraphicsDeviceType.OpenGL2 || SystemInfo.graphicsDeviceType == GraphicsDeviceType.OpenGLCore)
		{
			flag = false;
		}
		Debug.Log("Using Shader Model " + ((!flag) ? "2" : "3"));
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
		Debug.Log(SystemInfo.graphicsDeviceName);
		Debug.Log(SystemInfo.graphicsDeviceType);
		Debug.Log(SystemInfo.graphicsShaderLevel);
		Debug.Log(SystemInfo.graphicsDeviceVersion);
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
	}

	public void Initialize()
	{
		AsyncWWWManager.WWWRequest(new CachedGetRequest(Urls.StreamingAssets + highResAtlasFileName, Callback, WWWRequestPriority.WaitUntilSyncronizingIsDone));
	}

	private void Callback(WWW www)
	{
		string[] allAssetNames = www.assetBundle.GetAllAssetNames();
		if (allAssetNames.Length != 1)
		{
			Debug.LogError("Failed to load highres texture from bundle as multiple assets where included");
			return;
		}
		Texture2D mainTexture = www.assetBundle.LoadAsset<Texture2D>(allAssetNames[0]);
		SetMainTexture(mainTexture);
		SetupMaterials();
	}

	private void InitAllMaterials(bool useSM3)
	{
		int num = Mathf.CeilToInt((float)cubeModelMaterial.mainTexture.width * (TextureAtlas.UV[0].width + 1f / (float)cubeModelMaterial.mainTexture.width));
		int num2 = Mathf.FloorToInt(Mathf.Log(num, 2f));
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
