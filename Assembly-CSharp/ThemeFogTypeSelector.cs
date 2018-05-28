using System;
using ThemeAttributes;
using UnityEngine;
using UnityEngine.UI;

public class ThemeFogTypeSelector : IntAttribute.Setter
{
	private class NamedFogMode
	{
		public FogMode Mode { get; set; }

		public string Name { get; set; }
	}

	[SerializeField]
	private Text settingNameLabel;

	[SerializeField]
	private Text presetNameLabel;

	private Action<int> onChange;

	private int modeIndex = -1;

	private NamedFogMode[] availableModes = new NamedFogMode[2]
	{
		new NamedFogMode
		{
			Mode = FogMode.Exponential,
			Name = "Light"
		},
		new NamedFogMode
		{
			Mode = FogMode.ExponentialSquared,
			Name = "Heavy"
		}
	};

	public override void Initialize(IntAttribute attrib, Action<int> onChange)
	{
		this.onChange = onChange;
		for (int i = 0; i < availableModes.Length; i++)
		{
			if (availableModes[i].Mode == (FogMode)attrib.Value)
			{
				modeIndex = i;
			}
		}
		modeIndex = Constrain(modeIndex);
		settingNameLabel.text = attrib.Name;
		presetNameLabel.text = availableModes[modeIndex].Name;
	}

	public void Decrement()
	{
		modeIndex--;
		if (modeIndex < 0)
		{
			modeIndex += availableModes.Length;
		}
		OnSettingChanged();
	}

	public void Increment()
	{
		modeIndex++;
		if (modeIndex >= availableModes.Length)
		{
			modeIndex -= availableModes.Length;
		}
		OnSettingChanged();
	}

	private int Constrain(int value)
	{
		int num = Mathf.Abs(value % availableModes.Length);
		if (num != value)
		{
			num = 0;
			Debug.LogError("Fog mode is outside expected range.");
		}
		return num;
	}

	public void OnSettingChanged()
	{
		modeIndex = Constrain(modeIndex);
		presetNameLabel.text = availableModes[modeIndex].Name;
		onChange((int)availableModes[modeIndex].Mode);
	}
}
