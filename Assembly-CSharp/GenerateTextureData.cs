using System;
using System.Collections;
using MV.Common;
using UnityEngine;

public class GenerateTextureData : MonoBehaviour
{
	private static bool isCreatingScreenShot;

	public static bool IsCreatingScreenShot => isCreatingScreenShot;

	public void GenerateTextureDataCameraView(Action<byte[], ImageType, int> callback, ImageType imageType, int imageId)
	{
		if (isCreatingScreenShot)
		{
			Object.Destroy((Object)(object)((Component)this).gameObject);
			Debug.LogError((object)"Texture is being generated");
		}
		else
		{
			((MonoBehaviour)this).StartCoroutine(GenerateTexture(callback, imageType, imageId));
		}
	}

	private IEnumerator GenerateTexture(Action<byte[], ImageType, int> textureDataCallback, ImageType imageType, int imageId)
	{
		isCreatingScreenShot = true;
		int width = 600;
		int height = 240;
		GameObject screenshotCamObject = new GameObject();
		screenshotCamObject.layer = LayerMask.NameToLayer("Default");
		SkyboxManager skybox = Object.FindObjectOfType(typeof(SkyboxManager)) as SkyboxManager;
		Color color = ((!((Object)(object)skybox != (Object)null)) ? SkyboxManager.defaultColor : skybox.currentColor);
		Camera screenshotCam = screenshotCamObject.AddComponent<Camera>();
		screenshotCam.clearFlags = (CameraClearFlags)2;
		screenshotCam.backgroundColor = color;
		screenshotCam.fieldOfView = Camera.main.fieldOfView;
		screenshotCam.aspect = (float)width / (float)height;
		LayerFlags layers = LayerFlags.Default | LayerFlags.Water | LayerFlags.Player;
		screenshotCam.cullingMask = LayerMask.op_Implicit(LayerUtil.GetMask(layers));
		screenshotCamObject.transform.position = ((Component)Camera.main).gameObject.transform.position;
		screenshotCamObject.transform.rotation = ((Component)Camera.main).gameObject.transform.rotation;
		RenderTexture screenshotRenderTexture = (screenshotCam.targetTexture = new RenderTexture(width, height, 24));
		yield return 0;
		yield return 0;
		RenderTexture.active = screenshotRenderTexture;
		Texture2D screenshotTexture = new Texture2D(width, height, (TextureFormat)3, false);
		screenshotTexture.ReadPixels(new Rect(0f, 0f, (float)width, (float)height), 0, 0);
		screenshotTexture.Apply();
		byte[] bytes = screenshotTexture.EncodeToPNG();
		textureDataCallback((byte[])bytes.Clone(), imageType, imageId);
		Object.Destroy((Object)(object)screenshotCamObject);
		Object.Destroy((Object)(object)((Component)this).gameObject);
		isCreatingScreenShot = false;
	}
}
