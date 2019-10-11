using System;
using UnityEngine.Networking;

public class GetRequest : AsyncWebRequest
{
	public GetRequest(string path, Action<UnityWebRequest> callback, WWWRequestPriority requestPriority)
		: base(path, callback, requestPriority)
	{
	}

	protected override UnityWebRequest Create()
	{
		return UnityWebRequest.Get(path);
	}
}
