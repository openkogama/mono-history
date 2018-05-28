using System;
using ThemeSettings;
using UnityEngine;

namespace ThemeAttributes;

public abstract class ThemeAttribute
{
	protected const string valueOutOfBoundsMsg = "Theme attribute is out of expected range.";

	private SettingsWrapper themeSettings;

	public string Key { get; private set; }

	public int Groups { get; private set; }

	public abstract object Data { get; }

	public abstract RectTransform GetSettingsUIObject();

	public abstract void ApplyValue();

	protected virtual void Initialize(SettingsWrapper settings, string key, int groups)
	{
		Key = key;
		Groups = groups;
		themeSettings = settings;
		themeSettings.Add(this);
	}

	protected void UpdateSettings(string key, object value)
	{
		themeSettings.UpdateData(key, value);
	}
}
public abstract class ThemeAttribute<T> : ThemeAttribute
{
	[SerializeField]
	[Header("Configuration")]
	protected T value;

	protected Action<T> themeCallback;

	public T Value => value;

	public virtual void Initialize(SettingsWrapper settings, string key, int groups, Action<T> onChange)
	{
		base.Initialize(settings, key, groups);
		themeCallback = onChange;
	}

	protected abstract void OnSettingsChanged(T arg);

	public override void ApplyValue()
	{
		OnSettingsChanged(value);
	}
}
