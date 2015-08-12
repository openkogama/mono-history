using System;
using System.Collections.Generic;
using UnityEngine;

namespace Localize;

public class EnumLocalizeBookkeeping
{
	private Dictionary<int, string> enumToStringKeyMap;

	private Action<Dictionary<int, string>> initCallback;

	public EnumLocalizeBookkeeping(Action<Dictionary<int, string>> initCallback)
	{
		this.initCallback = initCallback;
		TM.LanguageChanged(Init);
		Init();
	}

	public string GetLocalizedString(int enumVal)
	{
		if (!enumToStringKeyMap.ContainsKey(enumVal))
		{
			Debug.LogWarning("No localized string found for: " + enumVal);
			return enumVal.ToString();
		}
		return enumToStringKeyMap[enumVal];
	}

	private void Init()
	{
		enumToStringKeyMap = new Dictionary<int, string>();
		initCallback(enumToStringKeyMap);
	}
}
