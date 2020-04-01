using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

[Serializable]
[CreateAssetMenu]
public class EmbeddedPlayerConfig : ScriptableObject
{
	private class JSONDomainObject
	{
		public string domain = string.Empty;
	}

	[SerializeField]
	public List<EmbeddedSiteConfigData> siteData;

	[SerializeField]
	public EmbeddedSiteConfigData kogamaDefaultData;

	private bool initialized;

	private EmbeddedSiteConfigData currentSite;

	public void Initialize()
	{
		currentSite = kogamaDefaultData;
		InitializeWithURL("kogama.com");
		initialized = true;
	}

	private void OnURLSet(bool ok, string json)
	{
		Debug.Log("json: " + json);
		string text = string.Empty;
		try
		{
			if (ok)
			{
				JSONDomainObject jSONDomainObject = JsonConvert.DeserializeObject<JSONDomainObject>(json);
				text = jSONDomainObject.domain;
				Debug.Log("URL: " + text);
			}
			else
			{
				Debug.Log("requestDomain failed.");
			}
		}
		catch
		{
			Debug.LogError("Json could not be deserialized");
			return;
		}
		InitializeWithURL(text);
	}

	private void InitializeWithURL(string url)
	{
		if (Uri.TryCreate(url, UriKind.Absolute, out var result))
		{
			url = result.Host;
			Debug.Log("Made uri from URL: " + url);
			Debug.Log("uri: " + result.ToString());
		}
		string text = UnityWebRequest.UnEscapeURL(url);
		Debug.Log("unescape url: " + text);
		try
		{
			for (int i = 0; i < siteData.Count; i++)
			{
				if (IsValidHost(text, siteData[i].sites.ToArray()))
				{
					currentSite = siteData[i];
					break;
				}
			}
		}
		catch (Exception message)
		{
			Debug.Log(message);
			Debug.Log("Unable to generate URI from fetched domain.");
		}
	}

	private static bool IsValidHost(string host, string[] hosts)
	{
		int num = host.IndexOf("://");
		if (num > 0)
		{
			host = host.Substring(num + 3);
		}
		Uri uri = new UriBuilder("https", host).Uri;
		foreach (string text in hosts)
		{
			Debug.Log("comparing " + text.ToString() + " to " + uri.Host.ToString());
			if (uri.Host.Contains(text))
			{
				Debug.Log("Host match found: " + text + " matching " + uri.Host);
				return true;
			}
		}
		Debug.Log("no host match found for host: " + host);
		return false;
	}

	private static bool DoesHostMatch(string allowedHost, string[] applicationHost)
	{
		string[] array = allowedHost.Split("."[0]);
		if (applicationHost.Length < array.Length)
		{
			return false;
		}
		for (int i = 0; i < array.Length; i++)
		{
			string text = array[i];
			string value = applicationHost[applicationHost.Length - array.Length + i];
			if (!text.Equals(value))
			{
				return false;
			}
		}
		return true;
	}

	public EmbeddedSiteConfigData GetCurrentSiteData()
	{
		if (!initialized)
		{
			Debug.LogError("Embedded site data not initialized.");
			return kogamaDefaultData;
		}
		return currentSite;
	}
}
