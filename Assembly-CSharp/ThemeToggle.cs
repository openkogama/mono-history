using System;
using ThemeAttributes;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ThemeToggle : BoolAttribute.Setter, IEventSystemHandler, IHandleSettingChanged
{
	[SerializeField]
	protected Text label;

	[SerializeField]
	private SettingsToggle toggle;

	private Action<bool> onChange;

	public override void Initialize(BoolAttribute attrib, Action<bool> onChange)
	{
		this.onChange = onChange;
		label.text = attrib.Name;
		toggle.Initialize(attrib.Key, attrib.Value);
	}

	public void OnSettingChanged(string key, object value)
	{
		onChange((bool)value);
	}
}
