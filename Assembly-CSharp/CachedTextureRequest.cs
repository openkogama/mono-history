using System;
using UnityEngine.Networking;

public class CachedTextureRequest : CachedGetRequest
{
	private bool ReadableTextureData { get; set; }

	public CachedTextureRequest(string path, Action<UnityWebRequest> callback, WWWRequestPriority requestPriority, bool readable = false)
		: base(path, callback, requestPriority)
	{
	}

	protected override UnityWebRequest Create()
	{
		string uri = path + MVGameControllerBase.KoGaMaSettings.UrlCacheAssetVersionArgument;
		return UnityWebRequestTexture.GetTexture(uri, !ReadableTextureData);
	}
}
