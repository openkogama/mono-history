using System;
using UnityEngine.Networking;

public class CustomPostRequest : AsyncWebRequest
{
	public CustomPostRequest(UnityWebRequest request, Action<UnityWebRequest> callback, WWWRequestPriority requestPriority)
		: base(string.Empty, callback, requestPriority)
	{
		base.request = request;
	}

	protected override UnityWebRequest Create()
	{
		return request;
	}
}
