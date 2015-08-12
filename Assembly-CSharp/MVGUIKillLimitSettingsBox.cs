using System;
using System.Collections.Generic;

public class MVGUIKillLimitSettingsBox : UXCustomDialogBox
{
	public UXSlider limitSlider;

	public UXText intervalValueLabel;

	private int limit;

	public override void OnShowDialog()
	{
		base.OnShowDialog();
		limit = (int)limitSlider.Value;
		UpdateIntervalTime(limitSlider.Value);
		AddListeners();
	}

	private void AddListeners()
	{
		UXSlider uXSlider = limitSlider;
		uXSlider.OnValueChangedIntermediate = (UXSlider.OnValueChangedIntermediateDelegate)Delegate.Combine(uXSlider.OnValueChangedIntermediate, (UXSlider.OnValueChangedIntermediateDelegate)((UXSlider s, float v) =>
		{
			UpdateIntervalTime(v);
		}));
		UXSlider uXSlider2 = limitSlider;
		uXSlider2.OnValueChanged = (UXSlider.OnValueChangedDelegate)Delegate.Combine(uXSlider2.OnValueChanged, (UXSlider.OnValueChangedDelegate)((UXSlider s) =>
		{
			UpdateIntervalTime(s.Value);
		}));
	}

	private void UpdateIntervalTime(float v)
	{
		limit = (int)v;
		intervalValueLabel.Text = $"{limit}";
	}

	public override object GetResult()
	{
		Dictionary<object, object> dictionary = new Dictionary<object, object>();
		dictionary.Add("killLimit", limit);
		return dictionary;
	}
}
