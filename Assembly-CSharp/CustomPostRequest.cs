using System;
using UnityEngine;

public class CustomPostRequest : AsyncWebRequest
{
	private WWW wwwReturn;

	public CustomPostRequest(WWW www, Action<WWW> callback)
		: base(string.Empty, callback)
	{
		wwwReturn = www;
	}

	protected override WWW Create()
	{
		return wwwReturn;
	}
}
