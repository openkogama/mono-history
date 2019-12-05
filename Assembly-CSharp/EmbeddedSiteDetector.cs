using System;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

public static class EmbeddedSiteDetector
{
	private class JSONDomainObject
	{
		public string domain = string.Empty;
	}

	private static readonly string[] gameDistributionSites = new string[1] { "gamedistribution.com" };

	private static readonly string[] pokiSites = new string[1] { "poki.com" };

	public static EmbeddedSite GetEmbeddedSite()
	{
		EmbeddedSite site = EmbeddedSite.None;
		Action<bool, string> callback = (bool ok, string json) =>
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
				site = EmbeddedSite.None;
				return;
			}
			if (Uri.TryCreate(text, UriKind.Absolute, out var result))
			{
				text = result.Host;
				Debug.Log("Made uri from URL: " + text);
				Debug.Log("uri: " + result.ToString());
			}
			string text2 = UnityWebRequest.UnEscapeURL(text);
			Debug.Log("unescape url: " + text2);
			try
			{
				if (IsOnGameDistribution(text2))
				{
					site = EmbeddedSite.GameDistribution;
				}
				else if (IsOnPoki(text2))
				{
					site = EmbeddedSite.Poki;
				}
			}
			catch (Exception message)
			{
				Debug.Log(message);
				Debug.Log("Unable to generate URI from fetched domain.");
			}
		};
		BrowserComm.ToJavaScript.ExternalCall("requestDomain", callback);
		return site;
	}

	private static bool IsOnGameDistribution(string host)
	{
		return IsValidHost(host, gameDistributionSites);
	}

	private static bool IsOnPoki(string host)
	{
		return IsValidHost(host, pokiSites);
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
}
