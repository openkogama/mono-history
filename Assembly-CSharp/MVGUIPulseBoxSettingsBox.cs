using System;
using System.Collections.Generic;

public class MVGUIPulseBoxSettingsBox : UXCustomDialogBox
{
	public UXText enabledTimeValueLabel;

	public UXText disabledTimeValueLabel;

	public UXSlider enabledTimeSlider;

	public UXSlider disabledTimeSlider;

	private float enabledTimeIntermediate;

	private float disabledTimeIntermediate;

	public override object GetResult()
	{
		Dictionary<object, object> dictionary = new Dictionary<object, object>();
		dictionary.Add("intervalOn", enabledTimeIntermediate);
		dictionary.Add("intervalOff", disabledTimeIntermediate);
		return dictionary;
	}

	public override void OnShowDialog()
	{
		base.OnShowDialog();
		UpdateEnabledTime(enabledTimeSlider.Value);
		UpdateDisabledTime(disabledTimeSlider.Value);
		UXSlider uXSlider = enabledTimeSlider;
		uXSlider.OnValueChanged = (UXSlider.OnValueChangedDelegate)Delegate.Combine(uXSlider.OnValueChanged, (UXSlider.OnValueChangedDelegate)((UXSlider slider) =>
		{
			UpdateEnabledTime(slider.Value);
			FireIntermediateResult();
		}));
		UXSlider uXSlider2 = disabledTimeSlider;
		uXSlider2.OnValueChanged = (UXSlider.OnValueChangedDelegate)Delegate.Combine(uXSlider2.OnValueChanged, (UXSlider.OnValueChangedDelegate)((UXSlider slider) =>
		{
			UpdateDisabledTime(slider.Value);
			FireIntermediateResult();
		}));
		UXSlider uXSlider3 = enabledTimeSlider;
		uXSlider3.OnValueChangedIntermediate = (UXSlider.OnValueChangedIntermediateDelegate)Delegate.Combine(uXSlider3.OnValueChangedIntermediate, (UXSlider.OnValueChangedIntermediateDelegate)((UXSlider slider, float v) =>
		{
			UpdateEnabledTime(v);
			FireIntermediateResult();
		}));
		UXSlider uXSlider4 = disabledTimeSlider;
		uXSlider4.OnValueChangedIntermediate = (UXSlider.OnValueChangedIntermediateDelegate)Delegate.Combine(uXSlider4.OnValueChangedIntermediate, (UXSlider.OnValueChangedIntermediateDelegate)((UXSlider slider, float v) =>
		{
			UpdateDisabledTime(v);
			FireIntermediateResult();
		}));
	}

	private void UpdateEnabledTime(float v)
	{
		enabledTimeIntermediate = v;
		enabledTimeValueLabel.Text = FormatValue(enabledTimeIntermediate);
	}

	private void UpdateDisabledTime(float v)
	{
		disabledTimeIntermediate = v;
		disabledTimeValueLabel.Text = FormatValue(disabledTimeIntermediate);
	}

	private string FormatValue(float v)
	{
		return $"{v:0.0}";
	}
}
