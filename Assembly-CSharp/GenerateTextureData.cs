using System;
using System.Collections;
using UnityEngine;

public class GenerateTextureData : MonoBehaviour
{
	public static bool IsCreatingScreenShot { get; private set; }

	public void GenerateTextureDataCameraView(Action<byte[]> callback)
	{
		if (IsCreatingScreenShot)
		{
			UnityEngine.Object.Destroy(gameObject);
			Debug.LogError("Texture is being generated");
		}
		else
		{
			StartCoroutine(GenerateTexture(callback));
		}
	}

	private IEnumerator GenerateTexture(Action<byte[]> textureDataCallback)
	{
		IsCreatingScreenShot = true;
		GameObject screenshotCamObject = new GameObject
		{
			layer = LayerMask.NameToLayer("Default")
		};
		SkyboxManager skyboxManager = MVGameControllerBase.SkyboxManager;
		Color color = ((!(skyboxManager != null)) ? SkyboxManager.defaultColor : skyboxManager.currentColor);
		Camera screenshotCam = screenshotCamObject.AddComponent<Camera>();
		screenshotCam.clearFlags = Camera.main.clearFlags;
		screenshotCam.backgroundColor = color;
		Skybox skybox = screenshotCamObject.AddComponent<Skybox>();
		screenshotCamObject.AddComponent<FlareLayer>();
		skybox.material = MVGameControllerBase.MainCameraManager.Skybox.material;
		screenshotCam.fieldOfView = Camera.main.fieldOfView;
		screenshotCam.aspect = Camera.main.aspect;
		int height = (int)(920f / Camera.main.aspect);
		screenshotCam.nearClipPlane = Camera.main.nearClipPlane;
		LayerFlags layers = LayerFlags.Default | LayerFlags.Water | LayerFlags.Player;
		screenshotCam.cullingMask = LayerUtil.GetMask(layers);
		screenshotCamObject.transform.position = Camera.main.gameObject.transform.position;
		screenshotCamObject.transform.rotation = Camera.main.gameObject.transform.rotation;
		RenderTexture screenshotRenderTexture = (screenshotCam.targetTexture = new RenderTexture(920, height, 24)
		{
			antiAliasing = 8,
			anisoLevel = 16
		});
		yield return 0;
		yield return 0;
		RenderTexture.active = screenshotRenderTexture;
		Texture2D screenshotTexture = new Texture2D(920, height, TextureFormat.RGB24, mipChain: false);
		screenshotTexture.ReadPixels(new Rect(0f, 0f, 920f, height), 0, 0);
		screenshotTexture.Apply();
		byte[] bytes = screenshotTexture.EncodeToPNG();
		textureDataCallback((byte[])bytes.Clone());
		screenshotCam.targetTexture = null;
		screenshotRenderTexture.Release();
		RenderTexture.active = null;
		UnityEngine.Object.Destroy(screenshotCamObject);
		UnityEngine.Object.Destroy(gameObject);
		IsCreatingScreenShot = false;
	}
}
