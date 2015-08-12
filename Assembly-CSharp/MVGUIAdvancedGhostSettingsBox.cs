using System;
using System.Collections.Generic;

public class MVGUIAdvancedGhostSettingsBox : UXCustomDialogBox
{
	public UXSlider radiusSlider;

	public UXSlider speedSlider;

	private float radius;

	private float speed;

	public override void OnShowDialog()
	{
		base.OnShowDialog();
		radius = radiusSlider.Value;
		speed = speedSlider.Value;
		AddListeners();
	}

	private void AddListeners()
	{
		UXSlider uXSlider = radiusSlider;
		uXSlider.OnValueChangedIntermediate = (UXSlider.OnValueChangedIntermediateDelegate)Delegate.Combine(uXSlider.OnValueChangedIntermediate, (UXSlider.OnValueChangedIntermediateDelegate)((UXSlider s, float v) =>
		{
			radius = v;
			FireIntermediateResult();
		}));
		UXSlider uXSlider2 = speedSlider;
		uXSlider2.OnValueChangedIntermediate = (UXSlider.OnValueChangedIntermediateDelegate)Delegate.Combine(uXSlider2.OnValueChangedIntermediate, (UXSlider.OnValueChangedIntermediateDelegate)((UXSlider s, float v) =>
		{
			speed = v;
			FireIntermediateResult();
		}));
		UXSlider uXSlider3 = radiusSlider;
		uXSlider3.OnValueChanged = (UXSlider.OnValueChangedDelegate)Delegate.Combine(uXSlider3.OnValueChanged, (UXSlider.OnValueChangedDelegate)((UXSlider s) =>
		{
			radius = s.Value;
		}));
		UXSlider uXSlider4 = speedSlider;
		uXSlider4.OnValueChanged = (UXSlider.OnValueChangedDelegate)Delegate.Combine(uXSlider4.OnValueChanged, (UXSlider.OnValueChangedDelegate)((UXSlider s) =>
		{
			speed = s.Value;
		}));
	}

	public override object GetResult()
	{
		Dictionary<object, object> dictionary = new Dictionary<object, object>();
		dictionary.Add("Radius", radius);
		dictionary.Add("Speed", speed);
		return dictionary;
	}
}
