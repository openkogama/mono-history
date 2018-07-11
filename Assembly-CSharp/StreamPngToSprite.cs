using MV.Common;
using UnityEngine;
using UnityEngine.UI;

public class StreamPngToSprite : MonoBehaviour
{
	[SerializeField]
	[Header("Dependencies")]
	protected RawImage rawImage;

	private bool currentlyDownloading;

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
			Object.Destroy(rawImage.texture);
			rawImage.texture = null;
		}
	}

	private void StreamingTextureLoaded(WWW www)
	{
		currentlyDownloading = false;
		if (www != null && www.texture != null)
		{
			if (string.IsNullOrEmpty(www.error))
			{
				SetImageTexture(www.texture);
			}
			else
			{
				Debug.LogError("Stream static image 'StreamingTextureLoaded' failed : " + www.error);
			}
		}
		AsyncWWWManager.UnsubscribeWWWRequest(StreamingTextureLoaded);
	}

	public void SetImageTexture(Texture texture)
	{
		rawImage.texture = texture;
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
