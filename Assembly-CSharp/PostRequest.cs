using System;
using UnityEngine;

public class PostRequest : AsyncWebRequest
{
	private readonly WWWForm form;

	public PostRequest(string path, WWWForm form, Action<WWW> callback)
		: base(path, callback)
	{
		form.AddBinaryData("binary", new byte[1]);
		this.form = form;
	}

	protected override WWW Create()
	{
		return new WWW(path, form);
	}
}
