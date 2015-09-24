using System;
using System.Collections.Generic;
using UnityEngine;

public class CustomPostRequest : AsyncWebRequest
{
	private readonly string url = string.Empty;

	private readonly byte[] postData;

	private readonly Dictionary<string, string> headers;

	public CustomPostRequest(string url, byte[] postData, Dictionary<string, string> headers, Action<WWW> callback)
		: base(string.Empty, callback)
	{
		this.url = url;
		this.postData = postData;
		this.headers = headers;
	}

	protected override WWW Create()
	{
		return new WWW(url, postData, headers);
	}
}
