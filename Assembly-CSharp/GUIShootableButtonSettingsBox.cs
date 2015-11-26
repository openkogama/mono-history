using System;
using System.Collections.Generic;

public class GUIShootableButtonSettingsBox : UXCustomDialogBox
{
	public UXText durationTextLabel;

	public UXText durationNumberLabel;

	public UXSlider durationSlider;

	private float durationIntermediate = 10f;

	public override object GetResult()
	{
		Dictionary<object, object> dictionary = new Dictionary<object, object>();
		dictionary.Add("duration", durationIntermediate);
		return dictionary;
	}

	public override void OnShowDialog()
	{
		base.OnShowDialog();
		UpdateDuration(durationSlider.Value);
		UXSlider uXSlider = durationSlider;
		uXSlider.OnValueChanged = (UXSlider.OnValueChangedDelegate)Delegate.Combine(uXSlider.OnValueChanged, (UXSlider.OnValueChangedDelegate)((UXSlider slider) =>
		{
			UpdateDuration(slider.Value);
			FireIntermediateResult();
		}));
		UXSlider uXSlider2 = durationSlider;
		uXSlider2.OnValueChangedIntermediate = (UXSlider.OnValueChangedIntermediateDelegate)Delegate.Combine(uXSlider2.OnValueChangedIntermediate, (UXSlider.OnValueChangedIntermediateDelegate)((UXSlider slider, float v) =>
		{
			UpdateDuration(v);
			FireIntermediateResult();
		}));
	}

	private void UpdateDuration(float v)
	{
		durationIntermediate = v;
		durationNumberLabel.Text = durationIntermediate.ToString("F1");
	}
}
