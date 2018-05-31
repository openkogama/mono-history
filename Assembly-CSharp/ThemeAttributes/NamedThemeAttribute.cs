using System;
using ThemeSettings;
using UnityEngine;

namespace ThemeAttributes;

public abstract class NamedThemeAttribute<T> : ThemeAttribute<T>
{
	[SerializeField]
	private string name;

	public string Name => name;

	public override void Initialize(SettingsWrapper settings, string key, int groups, Action<T> onChange)
	{
		base.Initialize(settings, key, groups, onChange);
		name = TM._(name);
		TM.LanguageChanged(LanguageLoadedCallback);
	}

	public void LanguageLoadedCallback()
	{
		name = TM._(name);
	}

	protected void OnValidate()
	{
		name = Validate(name);
	}

	private string Validate(string str)
	{
		if (str.Length < "_(\"".Length || str.Substring(0, "_(\"".Length) != "_(\"")
		{
			str = "_(\"" + str;
		}
		if (str.Length < "\")".Length || str.Substring(str.Length - "\")".Length, "\")".Length) != "\")")
		{
			str += "\")";
		}
		return str;
	}
}
