using System;
using System.Collections;
using UnityEngine;

public class ReviveScreenshotGenerator : MonoBehaviour
{
	public void GenerateTextureDataCameraViewAtTransform(Action<byte[]> callback, Vector3 cameraPos, Quaternion cameraRot, int width, int height)
	{
		StartCoroutine(GenerateTexture(callback, cameraPos, cameraRot, width, height));
	}

	private IEnumerator GenerateTexture(Action<byte[]> textureDataCallback, Vector3 cameraPos, Quaternion cameraRot, int width, int height)
	{
		LayerFlags targetLayers = LayerFlags.Default | LayerFlags.Water;
		int oldLayers = Camera.main.cullingMask;
		Camera mainCam = MVGameControllerBase.MainCameraManager.MainCamera;
		GameObject screenshotCamObject = new GameObject
		{
			layer = LayerMask.NameToLayer("Default")
		};
		SkyboxManager skyboxManager = MVGameControllerBase.SkyboxManager;
		Color color = ((!(skyboxManager != null)) ? SkyboxManager.defaultColor : skyboxManager.currentColor);
		RenderTexture screenshotRenderTexture = new RenderTexture(width, height, 24)
		{
			antiAliasing = 8,
			anisoLevel = 16
		};
		Camera screenshotCam = screenshotCamObject.AddComponent<Camera>();
		screenshotCam.clearFlags = mainCam.clearFlags;
		screenshotCam.backgroundColor = color;
		screenshotCam.fieldOfView = mainCam.fieldOfView;
		screenshotCam.aspect = (float)width / (float)height;
		screenshotCam.nearClipPlane = mainCam.nearClipPlane;
		screenshotCam.transform.position = cameraPos;
		screenshotCam.transform.rotation = cameraRot;
		screenshotCam.cullingMask = (int)targetLayers;
		screenshotCam.targetTexture = screenshotRenderTexture;
		Skybox skybox = screenshotCamObject.AddComponent<Skybox>();
		screenshotCamObject.AddComponent<FlareLayer>();
		skybox.material = MVGameControllerBase.MainCameraManager.Skybox.material;
		Camera cullingCam = CullingApiWrapper.TargetCamera;
		CullingApiWrapper.TargetCamera = screenshotCam;
		yield return new WaitForEndOfFrame();
		yield return 0;
		RenderTexture.active = screenshotRenderTexture;
		Texture2D screenshotTexture = new Texture2D(width, height, TextureFormat.RGB24, mipChain: false);
		screenshotTexture.ReadPixels(new Rect(0f, 0f, width, height), 0, 0);
		screenshotTexture.Apply();
		byte[] bytes = screenshotTexture.EncodeToPNG();
		textureDataCallback((byte[])bytes.Clone());
		screenshotCam.targetTexture = null;
		CullingApiWrapper.TargetCamera = cullingCam;
		RenderTexture.active = null;
		screenshotRenderTexture.Release();
		UnityEngine.Object.Destroy(screenshotCamObject);
		UnityEngine.Object.Destroy(gameObject);
	}
}
