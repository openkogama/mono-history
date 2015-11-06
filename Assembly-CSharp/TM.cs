using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using GNU.Gettext;
using MV.Common;
using UnityEngine;

public class TM : MonoBehaviour
{
	private static TM instance;

	private static Catalog catalog = new Catalog();

	private static string fileName = string.Empty;

	private static bool languageLoadingDone = false;

	private static readonly List<Action> languageChangedCallback = new List<Action>();

	private static string baseResourcesPath = "Languages/";

	private static string cultureName = "en-US";

	public static string CultureName => cultureName;

	private static TM Instance
	{
		get
		{
			if (instance == null)
			{
				GameObject gameObject = new GameObject("Default TextManager");
				instance = (TM)gameObject.AddComponent(typeof(TM));
				UnityEngine.Object.DontDestroyOnLoad(gameObject);
			}
			return instance;
		}
	}

	private TM()
	{
	}

	public static TM GetInstance()
	{
		return Instance;
	}

	public static string _(string key)
	{
		key = StripAssetStringFromFuncIdentifier(key);
		if (!string.IsNullOrEmpty(key) && catalog != null)
		{
			CatalogEntry catalogEntry = catalog.FindItem(key, string.Empty);
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
		if (!languageLoadingDone)
		{
			languageChangedCallback.Add(onLanguageChangedCallback);
		}
	}

	public static string GetTextWithValues(string index, ValueInsert values)
	{
		string text = _(index);
		if (values != null)
		{
			text = string.Format(new CultureInfo(CultureName), text, values.GetValueParams());
		}
		return text;
	}

	public static string[] GetTextAsArray(string toArrayString)
	{
		return toArrayString.Split('{', '}');
	}

	public static void LoadLanguage(string filename)
	{
		GetInstance();
		if (filename == null || filename.Replace('_', '-') == cultureName)
		{
			catalog = null;
			SetLanguageLoadingDone();
		}
		else
		{
			TryGetTextAsset(filename);
		}
	}

	private static void TryGetTextAsset(string fileName)
	{
		TM.fileName = fileName;
		string text = baseResourcesPath + fileName + ".txt?" + MVGameControllerBase.VersionGuid;
		AsyncWWWManager.WWWRequest(new GetRequest(Urls.StreamingAssets + text, StreamingAssetCallback));
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
			catalog = new Catalog();
			catalog.Load(www.text, fileName);
			cultureName = fileName.Replace('_', '-');
		}
		catch (Exception message)
		{
			Debug.LogWarning(message);
			catalog = null;
		}
		SetLanguageLoadingDone();
	}

	private static void SetLanguageLoadingDone()
	{
		languageLoadingDone = true;
		foreach (Action item in languageChangedCallback)
		{
			item();
		}
		languageChangedCallback.Clear();
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
