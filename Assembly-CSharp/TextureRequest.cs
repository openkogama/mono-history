using System;
using UnityEngine.Networking;

public class TextureRequest : GetRequest
{
	private bool ReadableTextureData { get; set; }

	public TextureRequest(string path, Action<UnityWebRequest> callback, WWWRequestPriority requestPriority, bool readable = false)
		: base(path, callback, requestPriority)
	{
		ReadableTextureData = readable;
	}

	protected override UnityWebRequest Create()
	{
		string uri = path + MVGameControllerBase.KoGaMaSettings.UrlCacheAssetVersionArgument;
		return UnityWebRequestTexture.GetTexture(uri, !ReadableTextureData);
	}
}
