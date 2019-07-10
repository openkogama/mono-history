using System;
using MV.Common;
using UnityEngine;
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

	private void StreamingTextureLoaded(WWW www)
	{
		currentlyDownloading = false;
		Texture2D texture = www.texture;
		if (texture != null && string.IsNullOrEmpty(www.error))
		{
			SetImageTexture(texture);
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
