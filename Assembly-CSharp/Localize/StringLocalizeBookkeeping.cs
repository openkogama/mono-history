using System;
using System.Collections.Generic;
using UnityEngine;

namespace Localize;

public class StringLocalizeBookkeeping
{
	private Dictionary<string, string> stringToStringKeyMap;

	private Action<Dictionary<string, string>> initCallback;

	public StringLocalizeBookkeeping(Action<Dictionary<string, string>> initCallback)
	{
		this.initCallback = initCallback;
		TM.LanguageChanged(Init);
		Init();
	}

	public string GetLocalizedString(string stringVal)
	{
		if (!stringToStringKeyMap.ContainsKey(stringVal))
		{
			Debug.LogWarning("No localized string found for: " + stringVal);
			return stringVal;
		}
		return stringToStringKeyMap[stringVal];
	}

	private void Init()
	{
		stringToStringKeyMap = new Dictionary<string, string>();
		initCallback(stringToStringKeyMap);
	}
}
