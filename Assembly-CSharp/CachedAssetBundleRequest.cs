using System;
using UnityEngine.Networking;

public class CachedAssetBundleRequest : CachedGetRequest
{
	public CachedAssetBundleRequest(string path, Action<UnityWebRequest> callback, WWWRequestPriority requestPriority)
		: base(path, callback, requestPriority)
	{
	}

	protected override UnityWebRequest Create()
	{
		string uri = path + MVGameControllerBase.KoGaMaSettings.UrlCacheAssetVersionArgument;
		uint crc = 0u;
		uint localDiscCacheAssetVersion = (uint)MVGameControllerBase.KoGaMaSettings.LocalDiscCacheAssetVersion;
		return UnityWebRequestAssetBundle.GetAssetBundle(uri, localDiscCacheAssetVersion, crc);
	}
}
