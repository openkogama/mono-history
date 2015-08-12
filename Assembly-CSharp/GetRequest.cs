using System;
using UnityEngine;

public class GetRequest : AsyncWebRequest
{
	public GetRequest(string path, Action<WWW> callback)
		: base(path, callback)
	{
	}

	protected override WWW Create()
	{
		return new WWW(path);
	}
}
