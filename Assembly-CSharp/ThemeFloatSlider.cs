using System;
using ThemeAttributes;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ThemeFloatSlider : FloatAttribute.Setter, IEventSystemHandler, IHandleSettingChanged
{
	[SerializeField]
	private SettingsSlider slider;

	[SerializeField]
	private Text label;

	private Action<float> onChange;

	public override void Initialize(FloatAttribute attrib, Action<float> onChange)
	{
		this.onChange = onChange;
		label.text = attrib.Name;
		slider.Initialize(attrib.Key, attrib.Value, attrib.Min, attrib.Max);
	}

	public void OnSettingChanged(string key, object value)
	{
		onChange((float)value);
	}
}
