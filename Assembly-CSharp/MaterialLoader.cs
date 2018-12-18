using System;
using MV.Common;
using UnityEngine;
using UnityEngine.Rendering;

public class MaterialLoader : MonoBehaviour
{
	private const string highResAtlasFileName = "AssetBundles/Atlas/atlas.unity3d";

	[SerializeField]
	private Material cubeModelMaterialHigh;

	[SerializeField]
	private Material cubeModelMaterialLow;

	[SerializeField]
	private Material cubeModelMaterialMobile;

	[SerializeField]
	private Shader pickupItemShader;

	[SerializeField]
	private Shader wireframeShader;

	[SerializeField]
	private Shader defaultDiffuseShader;

	[SerializeField]
	private Texture2D lowResMaterials;

	private uint atlasHash;

	public Material CubeModelMaterial { get; private set; }

	public Shader PickupItemShader => pickupItemShader;

	public Shader WireframeShader => wireframeShader;

	public Shader DefaultDiffuseShader => defaultDiffuseShader;

	protected void Awake()
	{
		cubeModelMaterialHigh = UnityEngine.Object.Instantiate(cubeModelMaterialHigh);
		cubeModelMaterialLow = UnityEngine.Object.Instantiate(cubeModelMaterialLow);
		cubeModelMaterialMobile = UnityEngine.Object.Instantiate(cubeModelMaterialMobile);
		SetMainTexture(lowResMaterials);
	}

	protected void Start()
	{
		SetupMaterials();
	}

	protected void OnDestroy()
	{
		UnityEngine.Object.Destroy(cubeModelMaterialLow);
		UnityEngine.Object.Destroy(cubeModelMaterialHigh);
		AsyncWWWManager.UnsubscribeWWWRequest(Callback);
	}

	public bool CheckAtlasIntegrity()
	{
		bool flag = Hash((Texture2D)CubeModelMaterial.mainTexture) == atlasHash;
		if (!flag)
		{
			StatHatWrapper.Count("TextureAtlasHackDetected", 1);
		}
		return flag;
	}

	private void SetMainTexture(Texture2D texture)
	{
		atlasHash = Hash(texture);
		cubeModelMaterialHigh.mainTexture = texture;
		cubeModelMaterialLow.mainTexture = texture;
		cubeModelMaterialMobile.mainTexture = texture;
	}

	private void SetupMaterials()
	{
		bool flag = false;
		if (SystemInfo.graphicsShaderLevel >= 30 && (SystemInfo.graphicsDeviceType == GraphicsDeviceType.Direct3D12 || SystemInfo.graphicsDeviceType == GraphicsDeviceType.Direct3D11))
		{
			flag = true;
		}
		if (SystemInfo.graphicsDeviceType == GraphicsDeviceType.OpenGLES3)
		{
			flag = true;
		}
		if (SystemInfo.graphicsDeviceType == GraphicsDeviceType.OpenGLES2 || SystemInfo.graphicsDeviceType == GraphicsDeviceType.OpenGLCore)
		{
			flag = false;
		}
		if (cubeModelMaterialHigh == null || cubeModelMaterialLow == null)
		{
			throw new NullReferenceException();
		}
		if (flag)
		{
			CubeModelMaterial = cubeModelMaterialHigh;
		}
		else
		{
			CubeModelMaterial = cubeModelMaterialLow;
		}
		InitAllMaterials(flag);
	}

	public void Initialize()
	{
		DownloadWhenPossible();
	}

	private void DownloadWhenPossible()
	{
		if (Urls.StreamingAssetUrlReady())
		{
			Urls.onStreamingAssetsUrlAvailable = (Urls.OnStreamingAssetsUrlAvailable)Delegate.Remove(Urls.onStreamingAssetsUrlAvailable, new Urls.OnStreamingAssetsUrlAvailable(DownloadWhenPossible));
			AsyncWWWManager.WWWRequest(new CachedGetRequest(Urls.StreamingAssets + "AssetBundles/Atlas/atlas.unity3d" + MVGameControllerBase.KoGaMaSettings.UrlCacheAssetVersionArgument, Callback, WWWRequestPriority.WaitUntilSyncronizingIsDone));
		}
		else
		{
			Urls.onStreamingAssetsUrlAvailable = (Urls.OnStreamingAssetsUrlAvailable)Delegate.Combine(Urls.onStreamingAssetsUrlAvailable, new Urls.OnStreamingAssetsUrlAvailable(DownloadWhenPossible));
		}
	}

	private void Callback(WWW www)
	{
		if (string.IsNullOrEmpty(www.error))
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
			www.assetBundle.Unload(unloadAllLoadedObjects: false);
		}
	}

	private void InitAllMaterials(bool useSM3)
	{
		int num = Mathf.CeilToInt((float)CubeModelMaterial.mainTexture.width * (TextureAtlas.UV[0].width + 1f / (float)CubeModelMaterial.mainTexture.width));
		int num2 = Mathf.FloorToInt(Mathf.Log(num, 2f));
		CubeModelMaterial.SetVector("_MaterialSize", new Vector4(TextureAtlas.UV[0].width, TextureAtlas.UV[0].height, num, num2));
		CubeModelMaterial.mainTexture.filterMode = FilterMode.Point;
		CubeModelMaterial.mainTexture.anisoLevel = 1;
		if (useSM3)
		{
			CubeModelMaterial.SetVector("_MaterialSize", new Vector4(TextureAtlas.UV[0].width, TextureAtlas.UV[0].height, num, num2));
			CubeModelMaterial.mainTexture.filterMode = FilterMode.Bilinear;
			CubeModelMaterial.mainTexture.anisoLevel = 2;
		}
	}

	private uint Hash(Texture2D tex)
	{
		uint num = 0u;
		byte[] rawTextureData = tex.GetRawTextureData();
		for (int i = 0; i < rawTextureData.Length; i += 10)
		{
			num += rawTextureData[i];
		}
		return num;
	}
}
