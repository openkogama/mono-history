using System;
using UnityEngine;

public class GetRequest : AsyncWebRequest
{
	public GetRequest(string path, Action<WWW> callback, WWWRequestPriority requestPriority)
		: base(path, callback, requestPriority)
	{
	}

	protected override WWW Create()
	{
		return new WWW(path);
	}
}
