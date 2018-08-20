using System;
using ThemeAttributes;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ThemeColorPickerRGB : ColorAttribute.Setter, IHandleSettingChanged, IEventSystemHandler
{
	[SerializeField]
	private Text label;

	[SerializeField]
	protected SettingsSlider sliderR;

	[SerializeField]
	protected SettingsSlider sliderG;

	[SerializeField]
	protected SettingsSlider sliderB;

	[SerializeField]
	protected RawImage previewImage;

	protected Action<Color> onChange;

	protected virtual void Reset()
	{
		SettingsSlider[] componentsInChildren = GetComponentsInChildren<SettingsSlider>();
		sliderR = componentsInChildren[0];
		sliderG = componentsInChildren[1];
		sliderB = componentsInChildren[2];
	}

	public override void Initialize(ColorAttribute attrib, Action<Color> onChange)
	{
		label.text = attrib.Name;
		this.onChange = (Color c) =>
		{
		};
		sliderR.Initialize("Red", attrib.Value.r, 0f, 1f);
		sliderG.Initialize("Green", attrib.Value.g, 0f, 1f);
		sliderB.Initialize("Blue", attrib.Value.b, 0f, 1f);
		previewImage.color = attrib.Value;
		this.onChange = onChange;
	}

	public virtual void OnSettingChanged(string key, object value)
	{
		ChangeColor(new Color(sliderR.Value, sliderG.Value, sliderB.Value, 1f));
	}

	protected void ChangeColor(Color c)
	{
		previewImage.color = c;
		onChange(c);
	}
}
