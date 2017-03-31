using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UploadGameScreenshotHandler : MonoBehaviour
{
	[SerializeField]
	private AudioSource screenShotSound;

	[SerializeField]
	private RawImage fromImage;

	[SerializeField]
	private RawImage toImage;

	public void TakeScreenshot()
	{
		GameObject gameObject = new GameObject("GenerateTexture");
		GenerateTextureData generateTextureData = gameObject.AddComponent<GenerateTextureData>();
		generateTextureData.GenerateTextureDataCameraView(OnScreenshotReady);
	}

	private void OnScreenshotReady(byte[] imageData)
	{
		screenShotSound.Play();
		Texture2D texture2D = new Texture2D(600, 240, TextureFormat.ARGB32, mipmap: false);
		texture2D.LoadImage(imageData);
		toImage.texture = texture2D;
	}

	public void UploadScreenshot()
	{
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IModalPopupCreator x, BaseEventData y) =>
		{
			x.Create();
		});
		MVGameControllerBase.Game.ScreenshotUploaded += OnScreenShotUploaded;
		MVGameControllerBase.OperationRequests.UploadGameScreenShot();
	}

	private void OnScreenShotUploaded(object sender, ScreenshotUploadedEventArgs args)
	{
		MVGameControllerBase.Game.ScreenshotUploaded -= OnScreenShotUploaded;
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
		{
			x.Pop();
		});
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
		{
			x.Pop();
		});
		string text = TM._("Screenshot Successfully uploaded");
		if (!args.Uploaded)
		{
			text = TM._("Failed to upload screenshot");
		}
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IModalPopupCreator x, BaseEventData y) =>
		{
			x.Create(text, TM._("Screenshot upload"));
		});
	}
}
