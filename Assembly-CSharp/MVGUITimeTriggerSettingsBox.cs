using System;
using System.Collections.Generic;

public class MVGUITimeTriggerSettingsBox : UXCustomDialogBox
{
	public UXText delayTimeValueLabel;

	public UXText durationTimeValueLabel;

	public UXSlider delayTimeSlider;

	public UXSlider durationTimeSlider;

	private float delayTimeIntermediate;

	private float durationTimeIntermediate;

	public override object GetResult()
	{
		Dictionary<object, object> dictionary = new Dictionary<object, object>();
		dictionary.Add("time", delayTimeIntermediate);
		dictionary.Add("duration", durationTimeIntermediate);
		return dictionary;
	}

	public override void OnShowDialog()
	{
		base.OnShowDialog();
		UpdateDelayTime(delayTimeSlider.Value);
		UpdateDurationTime(durationTimeSlider.Value);
		UXSlider uXSlider = delayTimeSlider;
		uXSlider.OnValueChanged = (UXSlider.OnValueChangedDelegate)Delegate.Combine(uXSlider.OnValueChanged, (UXSlider.OnValueChangedDelegate)((UXSlider slider) =>
		{
			UpdateDelayTime(slider.Value);
			FireIntermediateResult();
		}));
		UXSlider uXSlider2 = durationTimeSlider;
		uXSlider2.OnValueChanged = (UXSlider.OnValueChangedDelegate)Delegate.Combine(uXSlider2.OnValueChanged, (UXSlider.OnValueChangedDelegate)((UXSlider slider) =>
		{
			UpdateDurationTime(slider.Value);
			FireIntermediateResult();
		}));
		UXSlider uXSlider3 = delayTimeSlider;
		uXSlider3.OnValueChangedIntermediate = (UXSlider.OnValueChangedIntermediateDelegate)Delegate.Combine(uXSlider3.OnValueChangedIntermediate, (UXSlider.OnValueChangedIntermediateDelegate)((UXSlider slider, float v) =>
		{
			UpdateDelayTime(v);
			FireIntermediateResult();
		}));
		UXSlider uXSlider4 = durationTimeSlider;
		uXSlider4.OnValueChangedIntermediate = (UXSlider.OnValueChangedIntermediateDelegate)Delegate.Combine(uXSlider4.OnValueChangedIntermediate, (UXSlider.OnValueChangedIntermediateDelegate)((UXSlider slider, float v) =>
		{
			UpdateDurationTime(v);
			FireIntermediateResult();
		}));
	}

	private void UpdateDelayTime(float v)
	{
		delayTimeIntermediate = v;
		delayTimeValueLabel.Text = FormatValue(delayTimeIntermediate);
	}

	private void UpdateDurationTime(float v)
	{
		durationTimeIntermediate = v;
		durationTimeValueLabel.Text = FormatValue(durationTimeIntermediate);
	}

	private string FormatValue(float v)
	{
		return $"{v:0.0}";
	}
}
