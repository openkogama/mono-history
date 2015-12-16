using System;
using System.Collections.Generic;

public class MVGUICountingCubeSettingsBox : UXCustomDialogBox
{
	public UXText triggerValueLabel;

	public UXSlider startingValueSlider;

	public UXToggleIconButton resetToggle;

	private int startingValue;

	private bool resetValue;

	public override object GetResult()
	{
		Dictionary<object, object> dictionary = new Dictionary<object, object>();
		dictionary.Add("startingValue", startingValue);
		dictionary.Add("reset", resetValue);
		return dictionary;
	}

	public override void OnShowDialog()
	{
		base.OnShowDialog();
		UpdateTriggerValue(startingValueSlider.Value);
		UpdateResetValue(resetToggle.ToggleState);
		UXSlider uXSlider = startingValueSlider;
		uXSlider.OnValueChanged = (UXSlider.OnValueChangedDelegate)Delegate.Combine(uXSlider.OnValueChanged, (UXSlider.OnValueChangedDelegate)((UXSlider slider) =>
		{
			UpdateTriggerValue(slider.Value);
			FireIntermediateResult();
		}));
		UXSlider uXSlider2 = startingValueSlider;
		uXSlider2.OnValueChangedIntermediate = (UXSlider.OnValueChangedIntermediateDelegate)Delegate.Combine(uXSlider2.OnValueChangedIntermediate, (UXSlider.OnValueChangedIntermediateDelegate)((UXSlider slider, float v) =>
		{
			UpdateTriggerValue(v);
			FireIntermediateResult();
		}));
		UXToggleIconButton uXToggleIconButton = resetToggle;
		uXToggleIconButton.OnToggle = (UXToggleIconButton.OnToggleDelegate)Delegate.Combine(uXToggleIconButton.OnToggle, (UXToggleIconButton.OnToggleDelegate)((bool toggle) =>
		{
			UpdateResetValue(toggle);
		}));
	}

	private void UpdateResetValue(bool newValue)
	{
		resetValue = newValue;
	}

	private void UpdateTriggerValue(float v)
	{
		startingValue = (int)v;
		triggerValueLabel.Text = startingValue.ToString();
	}
}
