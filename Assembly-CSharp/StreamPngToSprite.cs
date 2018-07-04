using MV.Common;
using UnityEngine;
using UnityEngine.UI;

public class StreamPngToSprite : MonoBehaviour
{
	[Header("Dependencies")]
	[SerializeField]
	protected RawImage rawImage;

	private string url;

	private bool isInitialized;

	public bool IsInitialized => isInitialized;

	public void Initialize(string downloadUrl)
	{
		isInitialized = true;
		url = downloadUrl;
	}

	public void StartDownloading()
	{
		if (!isInitialized)
		{
			Debug.LogError("StreamingAssetManual can't start downloading while being uninitialized.");
		}
		AsyncWWWManager.WWWRequest(new CachedGetRequest(Urls.StreamingAssets + url, StreamingTextureLoaded, WWWRequestPriority.WaitUntilSyncronizingIsDone));
	}

	private void StreamingTextureLoaded(WWW www)
	{
		if (www != null && www.texture != null)
		{
			if (string.IsNullOrEmpty(www.error))
			{
				SetPromotionTexture(www.texture);
			}
			else
			{
				Debug.LogError("Stream static image 'StreamingTextureLoaded' failed : " + www.error);
			}
		}
	}

	public void SetPromotionTexture(Texture tex)
	{
		rawImage.texture = tex;
	}

	public void Reset()
	{
		if (rawImage == null)
		{
			rawImage = GetComponent<RawImage>();
		}
	}
}
