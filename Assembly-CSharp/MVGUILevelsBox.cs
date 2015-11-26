using System;
using System.Collections.Generic;

public class MVGUILevelsBox : UXCustomDialogBox
{
	public UXText levelValueLabel;

	public UXSlider levelSlider;

	private int levelIntermediate;

	public override object GetResult()
	{
		Dictionary<object, object> dictionary = new Dictionary<object, object>();
		dictionary.Add("levelAmount", levelIntermediate);
		return dictionary;
	}

	public override void OnShowDialog()
	{
		base.OnShowDialog();
		UpdateLevel(levelSlider.Value);
		UXSlider uXSlider = levelSlider;
		uXSlider.OnValueChanged = (UXSlider.OnValueChangedDelegate)Delegate.Combine(uXSlider.OnValueChanged, (UXSlider.OnValueChangedDelegate)((UXSlider slider) =>
		{
			UpdateLevel(slider.Value);
			FireIntermediateResult();
		}));
		UXSlider uXSlider2 = levelSlider;
		uXSlider2.OnValueChangedIntermediate = (UXSlider.OnValueChangedIntermediateDelegate)Delegate.Combine(uXSlider2.OnValueChangedIntermediate, (UXSlider.OnValueChangedIntermediateDelegate)((UXSlider slider, float v) =>
		{
			UpdateLevel(v);
			FireIntermediateResult();
		}));
	}

	private void UpdateLevel(float v)
	{
		levelIntermediate = (int)v;
		levelValueLabel.Text = $"{levelIntermediate:0}";
	}
}
