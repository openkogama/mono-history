using System;
using UnityEngine;
using UnityEngine.Networking;

public class PostRequest : AsyncWebRequest
{
	private readonly WWWForm form;

	public PostRequest(string path, WWWForm form, Action<UnityWebRequest> callback, WWWRequestPriority requestPriority)
		: base(path, callback, requestPriority)
	{
		form.AddBinaryData("binary", new byte[1]);
		this.form = form;
	}

	protected override UnityWebRequest Create()
	{
		return UnityWebRequest.Post(path, form);
	}
}
