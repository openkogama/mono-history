using System;
using System.Collections;
using UnityEngine;

public class GenerateTextureData : MonoBehaviour
{
	private static bool isCreatingScreenShot;

	public static bool IsCreatingScreenShot => isCreatingScreenShot;

	public void GenerateTextureDataCameraView(Action<byte[]> callback)
	{
		if (isCreatingScreenShot)
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
		isCreatingScreenShot = true;
		int width = 600;
		int height = 240;
		GameObject screenshotCamObject = new GameObject
		{
			layer = LayerMask.NameToLayer("Default")
		};
		SkyboxManager skybox = UnityEngine.Object.FindObjectOfType(typeof(SkyboxManager)) as SkyboxManager;
		Color color = ((!(skybox != null)) ? SkyboxManager.defaultColor : skybox.currentColor);
		Camera screenshotCam = screenshotCamObject.AddComponent<Camera>();
		screenshotCam.clearFlags = CameraClearFlags.Color;
		screenshotCam.backgroundColor = color;
		screenshotCam.fieldOfView = Camera.main.fieldOfView;
		screenshotCam.aspect = Camera.main.aspect;
		height = (int)((float)width / Camera.main.aspect);
		screenshotCam.nearClipPlane = Camera.main.nearClipPlane;
		LayerFlags layers = LayerFlags.Default | LayerFlags.Water | LayerFlags.Player;
		screenshotCam.cullingMask = LayerUtil.GetMask(layers);
		screenshotCamObject.transform.position = Camera.main.gameObject.transform.position;
		screenshotCamObject.transform.rotation = Camera.main.gameObject.transform.rotation;
		RenderTexture screenshotRenderTexture = (screenshotCam.targetTexture = new RenderTexture(width, height, 24));
		yield return 0;
		yield return 0;
		RenderTexture.active = screenshotRenderTexture;
		Texture2D screenshotTexture = new Texture2D(width, height, TextureFormat.RGB24, mipmap: false);
		screenshotTexture.ReadPixels(new Rect(0f, 0f, width, height), 0, 0);
		screenshotTexture.Apply();
		byte[] bytes = screenshotTexture.EncodeToPNG();
		textureDataCallback((byte[])bytes.Clone());
		screenshotCam.targetTexture = null;
		RenderTexture.active = null;
		UnityEngine.Object.Destroy(screenshotCamObject);
		UnityEngine.Object.Destroy(gameObject);
		isCreatingScreenShot = false;
	}
}
