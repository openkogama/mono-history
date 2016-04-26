using System;
using UnityEngine;

public class StreamingAssetRequest : CachedGetRequest
{
	public StreamingAssetRequest(string path, Action<WWW> callback, WWWRequestPriority requestPriority)
		: base(path, callback, requestPriority)
	{
	}

	protected override WWW Create()
	{
		WWW wWW;
		if (MVGameControllerBase.VersionStreamingAssets >= 0)
		{
			wWW = ((!AsyncWebRequest.UseCaching) ? new WWW(path + "?version=" + MVGameControllerBase.VersionStreamingAssets) : WWW.LoadFromCacheOrDownload(path + "?version=" + MVGameControllerBase.VersionStreamingAssets, MVGameControllerBase.VersionStreamingAssets));
		}
		else
		{
			Debug.LogError("Streaming assets version must be >= 0 to enable WWW.LoadFromCacheOrDownload. Using non-cached approach");
			wWW = new WWW(path + "?version=" + MVGameControllerBase.VersionStreamingAssets);
		}
		if (wWW == null)
		{
			Debug.LogError("WWW null");
		}
		return wWW;
	}
}
