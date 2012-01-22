using System.Collections;
using UnityEngine;

public class BrowserComm : MonoBehaviour
{
	public bool CreatePlanetScreenshot()
	{
		if (MVGameController.Instance.Game == null)
		{
			return false;
		}
		((MonoBehaviour)this).StartCoroutine(GenerateTextureData());
		return true;
	}

	protected IEnumerator GenerateTextureData()
	{
		int width = 600;
		int height = 240;
		GameObject screenshotCamObject = new GameObject();
		screenshotCamObject.layer = LayerMask.NameToLayer("Default");
		Camera screenshotCam = screenshotCamObject.AddComponent<Camera>();
		screenshotCam.clearFlags = (CameraClearFlags)1;
		screenshotCam.backgroundColor = Color.white;
		screenshotCam.fieldOfView = Camera.main.fieldOfView;
		screenshotCam.aspect = (float)width / (float)height;
		screenshotCam.cullingMask = 1 << LayerMask.NameToLayer("Default");
		screenshotCamObject.transform.position = ((Component)Camera.main).gameObject.transform.position;
		screenshotCamObject.transform.rotation = ((Component)Camera.main).gameObject.transform.rotation;
		RenderTexture screenshotRenderTexture = (screenshotCam.targetTexture = new RenderTexture(width, height, 24));
		yield return (object)new WaitForEndOfFrame();
		RenderTexture.active = screenshotRenderTexture;
		Texture2D screenshotTexture = new Texture2D(width, height, (TextureFormat)3, false);
		screenshotTexture.ReadPixels(new Rect(0f, 0f, (float)width, (float)height), 0, 0);
		screenshotTexture.Apply();
		byte[] bytes = screenshotTexture.EncodeToPNG();
		MVGameController.Instance.Game.UploadPlanetScreenshot((byte[])bytes.Clone());
		Object.Destroy((Object)(object)screenshotCamObject);
	}

	public void Exit()
	{
		if (MVGameController.Instance.Game != null)
		{
			MVGameController.Instance.Game.Leave();
		}
	}
}
