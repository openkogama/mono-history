using System;
using UnityEngine.Networking;

public class AssetBundleRequest : GetRequest
{
	public AssetBundleRequest(string path, Action<UnityWebRequest> callback, WWWRequestPriority requestPriority)
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
