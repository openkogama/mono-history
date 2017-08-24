using System;
using MV.Common;
using UnityEngine;
using UnityEngine.Rendering;

public class MaterialLoader : MonoBehaviour
{
	private static string highResAtlasFileName = "Atlas/atlas512glowing.unity3d";

	[SerializeField]
	private Material cubeModelMaterialHigh;

	[SerializeField]
	private Material cubeModelMaterialLow;

	[SerializeField]
	private Shader pickupItemShader;

	[SerializeField]
	private Shader wireframeShader;

	[SerializeField]
	private Shader defaultDiffuseShader;

	[SerializeField]
	private Texture2D lowResMaterials;

	private Material cubeModelMaterial;

	private uint atlasHash;

	public Material CubeModelMaterial => cubeModelMaterial;

	public Shader PickupItemShader => pickupItemShader;

	public Shader WireframeShader => wireframeShader;

	public Shader DefaultDiffuseShader => defaultDiffuseShader;

	public void Start()
	{
		cubeModelMaterialHigh = UnityEngine.Object.Instantiate(cubeModelMaterialHigh);
		cubeModelMaterialLow = UnityEngine.Object.Instantiate(cubeModelMaterialLow);
		SetMainTexture(lowResMaterials);
		SetupMaterials();
		MeshPool.Instance.MaxAmtMeshes = 100;
	}

	public bool CheckAtlasIntegrity()
	{
		bool flag = Hash((Texture2D)cubeModelMaterial.mainTexture) == atlasHash;
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
		if (cubeModelMaterialHigh == null || cubeModelMaterialLow == null)
		{
			throw new NullReferenceException();
		}
		if (flag)
		{
			cubeModelMaterial = cubeModelMaterialHigh;
		}
		else
		{
			cubeModelMaterial = cubeModelMaterialLow;
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
			AsyncWWWManager.WWWRequest(new CachedGetRequest(Urls.StreamingAssets + highResAtlasFileName + "?version=" + MVGameControllerBase.KoGaMaSettings.WebCacheInvalidationCode, Callback, WWWRequestPriority.WaitUntilSyncronizingIsDone));
		}
		else
		{
			Urls.onStreamingAssetsUrlAvailable = (Urls.OnStreamingAssetsUrlAvailable)Delegate.Combine(Urls.onStreamingAssetsUrlAvailable, new Urls.OnStreamingAssetsUrlAvailable(DownloadWhenPossible));
		}
	}

	private void OnDestroy()
	{
		AsyncWWWManager.UnsubscribeWWWRequest(Callback);
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
		}
	}

	private void InitAllMaterials(bool useSM3)
	{
		int num = Mathf.CeilToInt((float)cubeModelMaterial.mainTexture.width * (TextureAtlas.UV[0].width + 1f / (float)cubeModelMaterial.mainTexture.width));
		int num2 = Mathf.FloorToInt(Mathf.Log(num, 2f));
		cubeModelMaterial.SetVector("_MaterialSize", new Vector4(TextureAtlas.UV[0].width, TextureAtlas.UV[0].height, num, num2));
		cubeModelMaterial.mainTexture.filterMode = FilterMode.Point;
		cubeModelMaterial.mainTexture.anisoLevel = 1;
		if (useSM3)
		{
			cubeModelMaterial.SetVector("_MaterialSize", new Vector4(TextureAtlas.UV[0].width, TextureAtlas.UV[0].height, num, num2));
			cubeModelMaterial.mainTexture.filterMode = FilterMode.Bilinear;
			cubeModelMaterial.mainTexture.anisoLevel = 2;
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
