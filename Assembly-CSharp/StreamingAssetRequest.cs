using System;
using UnityEngine;

public class StreamingAssetRequest : CachedGetRequest
{
	public StreamingAssetRequest(string path, Action<WWW> callback)
		: base(path, callback)
	{
	}

	protected override WWW Create()
	{
		WWW wWW;
		if (MVGameController.VersionStreamingAssets >= 0)
		{
			wWW = (AsyncWebRequest.useCaching ? WWW.LoadFromCacheOrDownload(path + "?version=" + MVGameController.VersionStreamingAssets, MVGameController.VersionStreamingAssets) : new WWW(path + "?version=" + MVGameController.VersionStreamingAssets));
		}
		else
		{
			Debug.LogError("Streaming assets version must be >= 0 to enable WWW.LoadFromCacheOrDownload. Using non-cached approach");
			wWW = new WWW(path + "?version=" + MVGameController.VersionStreamingAssets);
		}
		if (wWW == null)
		{
			Debug.LogError("WWW null");
		}
		return wWW;
	}
}
