using System;
using System.Globalization;
using UnityEngine;

namespace Localize;

public class Localization
{
	private LocalizedTextCollections localizedTextCollections = new LocalizedTextCollections();

	private string cultureName = "en-US";

	private static readonly Localization _instance = new Localization();

	public string CultureName
	{
		get
		{
			return cultureName;
		}
		set
		{
			if (localizedTextCollections.TextCollections.ContainsKey(value))
			{
				cultureName = value;
			}
			else
			{
				Debug.LogWarning((object)("Culture name not found " + value));
			}
		}
	}

	public static Localization Instance => _instance;

	private Localization()
	{
	}

	public string GetText(TextSlotIndex index)
	{
		string text = localizedTextCollections.TextCollections[cultureName].GetBaseString(index);
		if (text == null)
		{
			text = localizedTextCollections.TextCollections["en-US"].GetBaseString(index);
			if (text == null)
			{
				text = "Error : Localisation index does not exist";
			}
			else if (MVGameController.Instance.Game.IsDebugMode)
			{
				text = "Error : No localisation for culture " + cultureName + " exists for string '" + text + "'";
			}
		}
		return text;
	}

	public string GetText(string key_as_text)
	{
		TextSlotIndex textSlotIndex = TextSlotIndex.Empty;
		try
		{
			textSlotIndex = (TextSlotIndex)(int)Enum.Parse(typeof(TextSlotIndex), key_as_text);
		}
		catch
		{
			return "[localized key not found]";
		}
		string text = localizedTextCollections.TextCollections[cultureName].GetBaseString(textSlotIndex);
		if (text == null)
		{
			text = localizedTextCollections.TextCollections["en-US"].GetBaseString(textSlotIndex);
			if (text == null)
			{
				text = "Error : Localisation index does not exist";
			}
			else if (MVGameController.Instance.Game.IsDebugMode)
			{
				text = "Error : No localisation for culture " + cultureName + " exists for string '" + text + "'";
			}
		}
		return text;
	}

	public string[] GetTextAsList(TextSlotIndex index)
	{
		string text = localizedTextCollections.TextCollections[cultureName].GetBaseString(index);
		if (text == null)
		{
			text = localizedTextCollections.TextCollections["en-US"].GetBaseString(index);
			if (text == null)
			{
				text = "Error : Localisation index does not exist";
			}
			else if (MVGameController.Instance.Game.IsDebugMode)
			{
				text = "Error : No localisation for culture " + cultureName + " exists for string '" + text + "'";
			}
		}
		return text.Split(new char[2] { '{', '}' });
	}

	public string GetTextWithValues(TextSlotIndex index, ValueInsert values)
	{
		string text = localizedTextCollections.TextCollections[cultureName].GetBaseString(index);
		if (text == null)
		{
			text = localizedTextCollections.TextCollections["en-US"].GetBaseString(index);
			if (text == null)
			{
				text = "Error : Localisation index does not exist";
			}
			else if (MVGameController.Instance.Game.IsDebugMode)
			{
				text = "Error : No localisation for culture " + cultureName + " exists for string '" + text + "'";
			}
		}
		if (values != null)
		{
			text = string.Format(new CultureInfo(CultureName), text, values.GetValueParams());
		}
		return text;
	}
}
