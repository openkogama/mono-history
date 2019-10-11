using System;
using System.Text;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using UnityEngine;

public static class EmbeddedSiteDetector
{
	private class JSONDomainObject
	{
		public string domain = string.Empty;
	}

	private static readonly string[] crazyGamesSites = new string[4] { "gioca.re", "1001juegos.com", "speelspelletjes.nl", "onlinegame.co.id" };

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
			if (IsOnCrazyGames(text))
			{
				site = EmbeddedSite.CrazyGames;
			}
		};
		BrowserComm.ToJavaScript.ExternalCall("requestDomain", callback);
		return site;
	}

	private static bool IsOnCrazyGames(string host)
	{
		string[] array = host.Split("."[0]);
		int num = -1;
		for (int i = 0; i < array.Length; i++)
		{
			string text = array[i].ToLower();
			if (text == "crazygames" || text == "dev-crazygames")
			{
				num = i;
				break;
			}
		}
		Debug.Log("crazyIndex: " + num);
		if ((num >= 0 && array.Length == num + 2) || (array.Length == num + 3 && array[num + 1].Length <= 3))
		{
			return IsValidHost(host, crazyGamesSites);
		}
		return false;
	}

	private static bool IsValidHost(string host, string[] hosts)
	{
		if (Debug.isDebugBuild)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("Checking against list of hosts: ");
			foreach (string value in hosts)
			{
				stringBuilder.Append(value);
				stringBuilder.Append(",");
			}
			Debug.Log(stringBuilder.ToString());
		}
		Regex regex = new Regex("^(\\w+)://(?<hostname>[^/]+?)(?<port>:\\d+)?/");
		Match match = regex.Match(host);
		if (!match.Success)
		{
			return false;
		}
		string value2 = match.Groups["hostname"].Value;
		string[] array = value2.Split("."[0]);
		foreach (string text in hosts)
		{
			if (DoesHostMatch(text, array))
			{
				Debug.Log("Host match found: " + text + " matching " + array);
				return true;
			}
		}
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
