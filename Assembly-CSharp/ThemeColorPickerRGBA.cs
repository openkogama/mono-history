using System;
using ThemeAttributes;
using UnityEngine;
using UnityEngine.EventSystems;

public class ThemeColorPickerRGBA : ThemeColorPickerRGB, IEventSystemHandler, IHandleSettingChanged
{
	[SerializeField]
	private SettingsSlider sliderA;

	protected override void Reset()
	{
		base.Reset();
		SettingsSlider[] componentsInChildren = GetComponentsInChildren<SettingsSlider>();
		sliderA = componentsInChildren[3];
	}

	public override void Initialize(ColorAttribute attrib, Action<Color> onChange)
	{
		base.Initialize(attrib, onChange);
		base.onChange = (Color c) =>
		{
		};
		sliderA.Initialize("Alpha", attrib.Value.a, 0f, 1f);
		base.onChange = onChange;
	}

	public override void OnSettingChanged(string key, object value)
	{
		ChangeColor(new Color(sliderR.Value, sliderG.Value, sliderB.Value, sliderA.Value));
	}
}
