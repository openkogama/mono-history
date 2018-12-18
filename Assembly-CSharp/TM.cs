using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using GNU.Gettext;
using MV.Common;
using UnityEngine;

public class TM
{
	private Catalog catalog = new Catalog();

	private string fileName = string.Empty;

	private bool languageLoadingDone;

	private readonly List<Action> languageChangedCallback = new List<Action>();

	private string cultureName = "en-US";

	private const string baseResourcesPath = "Languages/";

	private static TM instance;

	private static TM Instance
	{
		get
		{
			if (instance == null)
			{
				instance = new TM();
			}
			return instance;
		}
	}

	private TM()
	{
	}

	public static void Destroy()
	{
		if (instance != null)
		{
			instance = null;
			try
			{
				AsyncWWWManager.UnsubscribeWWWRequest(StreamingAssetCallback);
			}
			catch (Exception ex)
			{
				Debug.Log("TM failed to unsubscribe www request: " + ex.Message);
			}
		}
	}

	public static string _(string key)
	{
		key = StripAssetStringFromFuncIdentifier(key);
		if (!string.IsNullOrEmpty(key) && Instance.catalog != null)
		{
			CatalogEntry catalogEntry = Instance.catalog.FindItem(key, string.Empty);
			if (catalogEntry != null)
			{
				string translation = catalogEntry.GetTranslation(0);
				if (!string.IsNullOrEmpty(translation))
				{
					key = StringEscaping.FromGettextFormat(translation);
				}
			}
		}
		return key;
	}

	public static void LanguageChanged(Action onLanguageChangedCallback)
	{
		if (!Instance.languageLoadingDone)
		{
			Instance.languageChangedCallback.Add(onLanguageChangedCallback);
		}
	}

	public static string GetTextWithValues(string index, ValueInsert values)
	{
		string text = _(index);
		if (values != null)
		{
			text = string.Format(new CultureInfo(Instance.cultureName), text, values.GetValueParams());
		}
		return text;
	}

	public static string[] GetTextAsArray(string toArrayString)
	{
		return toArrayString.Split('{', '}');
	}

	public static void LoadLanguage(string languageName)
	{
		string filename = GetFilename(languageName);
		if (filename == null || filename.Replace('_', '-') == Instance.cultureName)
		{
			Instance.catalog = null;
			SetLanguageLoadingDone();
		}
		else
		{
			TryGetTextAsset(filename);
		}
	}

	private static string GetFilename(string languageName)
	{
		if (languageName == null)
		{
			return null;
		}
		if (languageName == "en_GB")
		{
			return "en_US";
		}
		if (languageName == "es")
		{
			return "es_ES";
		}
		return languageName;
	}

	private static void TryGetTextAsset(string fileName)
	{
		Instance.fileName = fileName;
		string text = "Languages/" + fileName + ".txt?" + MVGameControllerBase.KoGaMaSettings.VersionGuid;
		AsyncWWWManager.WWWRequest(new GetRequest(Urls.StreamingAssets + text, StreamingAssetCallback, WWWRequestPriority.ExecuteWhileSyncronizing));
	}

	private static void StreamingAssetCallback(WWW www)
	{
		if (www.error != null)
		{
			Debug.LogWarning($"Error loading text {www.url} {www.error}");
			return;
		}
		try
		{
			Instance.catalog = new Catalog();
			Instance.catalog.Load(www.text, Instance.fileName);
			Instance.cultureName = Instance.fileName.Replace('_', '-');
		}
		catch (Exception message)
		{
			Debug.LogWarning(message);
			Instance.catalog = null;
		}
		SetLanguageLoadingDone();
	}

	private static void SetLanguageLoadingDone()
	{
		Instance.languageLoadingDone = true;
		foreach (Action item in Instance.languageChangedCallback)
		{
			item();
		}
		Instance.languageChangedCallback.Clear();
	}

	private static string StripAssetStringFromFuncIdentifier(string key)
	{
		if (key == null)
		{
			return null;
		}
		string pattern = "_\\s*\\(\\s*(\"(?:[^\"]|\"\")*\"|\"(?:\\\\.|[^\\\\\"])*\")";
		Regex regex = new Regex(pattern);
		Match match = regex.Match(key);
		if (match.Success)
		{
			key = match.Groups[1].ToString();
			key = key.Substring(1, key.Length - 2);
			key = StringEscaping.FromGettextFormat(key);
		}
		return key;
	}
}
