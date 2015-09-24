using System;
using System.Collections.Generic;

public class GUIWindTurbineSettingsBox : UXCustomDialogBox
{
	public UXText windPitchLabel;

	public UXText windSizeLabel;

	public UXSlider windPitchSlider;

	public UXSlider windSizeSlider;

	private float windPitchIntermediate;

	private float windSizeIntermediate = 10f;

	public override object GetResult()
	{
		Dictionary<object, object> dictionary = new Dictionary<object, object>();
		dictionary.Add("windPitch", windPitchIntermediate);
		dictionary.Add("windSize", windSizeIntermediate);
		return dictionary;
	}

	public override void OnShowDialog()
	{
		base.OnShowDialog();
		UpdateWindPitch(windPitchSlider.Value);
		UpdateWindSize(windSizeSlider.Value);
		UXSlider uXSlider = windPitchSlider;
		uXSlider.OnValueChanged = (UXSlider.OnValueChangedDelegate)Delegate.Combine(uXSlider.OnValueChanged, (UXSlider.OnValueChangedDelegate)((UXSlider slider) =>
		{
			UpdateWindPitch(slider.Value);
			FireIntermediateResult();
		}));
		UXSlider uXSlider2 = windSizeSlider;
		uXSlider2.OnValueChanged = (UXSlider.OnValueChangedDelegate)Delegate.Combine(uXSlider2.OnValueChanged, (UXSlider.OnValueChangedDelegate)((UXSlider slider) =>
		{
			UpdateWindSize(slider.Value);
			FireIntermediateResult();
		}));
		UXSlider uXSlider3 = windPitchSlider;
		uXSlider3.OnValueChangedIntermediate = (UXSlider.OnValueChangedIntermediateDelegate)Delegate.Combine(uXSlider3.OnValueChangedIntermediate, (UXSlider.OnValueChangedIntermediateDelegate)((UXSlider slider, float v) =>
		{
			UpdateWindPitch(v);
			FireIntermediateResult();
		}));
		UXSlider uXSlider4 = windSizeSlider;
		uXSlider4.OnValueChangedIntermediate = (UXSlider.OnValueChangedIntermediateDelegate)Delegate.Combine(uXSlider4.OnValueChangedIntermediate, (UXSlider.OnValueChangedIntermediateDelegate)((UXSlider slider, float v) =>
		{
			UpdateWindSize(v);
			FireIntermediateResult();
		}));
	}

	private void UpdateWindPitch(float v)
	{
		windPitchIntermediate = v;
		windPitchLabel.Text = windPitchIntermediate.ToString();
	}

	private void UpdateWindSize(float v)
	{
		windSizeIntermediate = v;
		windSizeLabel.Text = v.ToString();
	}
}
