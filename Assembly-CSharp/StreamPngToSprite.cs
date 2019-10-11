using System;
using MV.Common;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class StreamPngToSprite : MonoBehaviour
{
	[Header("Dependencies")]
	[SerializeField]
	protected RawImage rawImage;

	private bool currentlyDownloading;

	public Action OnDownloadFinish;

	public void StartDownloading(string downloadUrl)
	{
		if (currentlyDownloading)
		{
			CancelDownload();
		}
		currentlyDownloading = true;
		AsyncWWWManager.WWWRequest(new CachedGetRequest(Urls.StreamingAssets + downloadUrl, StreamingTextureLoaded, WWWRequestPriority.WaitUntilSyncronizingIsDone));
	}

	public void CancelDownload()
	{
		DestroyTexture();
		currentlyDownloading = false;
		AsyncWWWManager.UnsubscribeWWWRequest(StreamingTextureLoaded);
	}

	public void DestroyTexture()
	{
		if (rawImage != null)
		{
			UnityEngine.Object.Destroy(rawImage.texture);
			rawImage.texture = null;
		}
	}

	private void StreamingTextureLoaded(UnityWebRequest www)
	{
		currentlyDownloading = false;
		Texture2D content = DownloadHandlerTexture.GetContent(www);
		if (content != null && string.IsNullOrEmpty(www.error))
		{
			SetImageTexture(content);
			if (OnDownloadFinish != null)
			{
				OnDownloadFinish();
			}
		}
		else
		{
			Debug.Log("URL: " + www.url + ": " + www.error);
			Debug.LogError("Error streaming png to sprite");
		}
	}

	public void SetImageTexture(Texture texture)
	{
		if (rawImage != null)
		{
			rawImage.texture = texture;
		}
	}

	public void Reset()
	{
		if (rawImage == null)
		{
			rawImage = GetComponent<RawImage>();
		}
	}

	private void OnDestroy()
	{
		CancelDownload();
	}
}
