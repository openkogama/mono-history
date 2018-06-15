using System;
using ThemeAttributes;
using UnityEngine;
using UnityEngine.UI;

public class ThemeDayNightCycleColorPresetSetter : IntAttribute.Setter
{
	[SerializeField]
	private DayNightCycleColorPresets colorPresets;

	[SerializeField]
	private Text settingNameLabel;

	[SerializeField]
	private Text presetNameLabel;

	private Action<int> onChange;

	private int colorPresetIndex;

	public override void Initialize(IntAttribute attrib, Action<int> onChange)
	{
		this.onChange = onChange;
		colorPresetIndex = Constrain(attrib.Value);
		settingNameLabel.text = attrib.Name;
		presetNameLabel.text = colorPresets[colorPresetIndex].Name;
	}

	public void Decrement()
	{
		colorPresetIndex--;
		if (colorPresetIndex < 0)
		{
			colorPresetIndex += colorPresets.Length;
		}
		OnSettingChanged();
	}

	public void Increment()
	{
		colorPresetIndex++;
		if (colorPresetIndex >= colorPresets.Length)
		{
			colorPresetIndex -= colorPresets.Length;
		}
		OnSettingChanged();
	}

	private int Constrain(int value)
	{
		int num = Mathf.Abs(value % colorPresets.Length);
		if (num != value)
		{
			num = 0;
			Debug.LogError("Day/Night cycle color preset index is outside expected range.");
		}
		return num;
	}

	public void OnSettingChanged()
	{
		colorPresetIndex = Constrain(colorPresetIndex);
		presetNameLabel.text = colorPresets[colorPresetIndex].Name;
		onChange(colorPresetIndex);
	}
}
