using System;
using UnityEngine.Networking;

public class CustomPostRequest : AsyncWebRequest
{
	private new readonly UnityWebRequest request;

	public CustomPostRequest(UnityWebRequest request, Action<UnityWebRequest> callback, WWWRequestPriority requestPriority)
		: base(string.Empty, callback, requestPriority)
	{
		this.request = request;
	}

	protected override UnityWebRequest Create()
	{
		return request;
	}
}
