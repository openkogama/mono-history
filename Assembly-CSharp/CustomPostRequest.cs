using System;
using System.Collections.Generic;
using UnityEngine.Networking;

public class CustomPostRequest : AsyncWebRequest
{
	private readonly string url = string.Empty;

	private readonly byte[] postData;

	private readonly Dictionary<string, string> headers;

	public CustomPostRequest(string url, byte[] postData, Dictionary<string, string> headers, Action<UnityWebRequest> callback, WWWRequestPriority requestPriority)
		: base(string.Empty, callback, requestPriority)
	{
		this.url = url;
		this.postData = postData;
		this.headers = headers;
	}

	protected override UnityWebRequest Create()
	{
		UnityWebRequest unityWebRequest = new UnityWebRequest(url, "POST");
		UploadHandlerRaw uploadHandlerRaw = new UploadHandlerRaw(postData);
		uploadHandlerRaw.contentType = "application/x-www-form-urlencoded";
		unityWebRequest.uploadHandler = uploadHandlerRaw;
		foreach (KeyValuePair<string, string> header in headers)
		{
			unityWebRequest.SetRequestHeader(header.Key, header.Value);
		}
		return unityWebRequest;
	}
}
