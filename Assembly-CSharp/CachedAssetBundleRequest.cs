using System;
using UnityEngine;

public class CachedAssetBundleRequest : CachedGetRequest
{
	public CachedAssetBundleRequest(string path, Action<WWW> callback, WWWRequestPriority requestPriority)
		: base(path, callback, requestPriority)
	{
	}

	protected override WWW Create()
	{
		return WWW.LoadFromCacheOrDownload(path + MVGameControllerBase.KoGaMaSettings.UrlCacheAssetVersionArgument, MVGameControllerBase.KoGaMaSettings.LocalDiscCacheAssetVersion);
	}
}
