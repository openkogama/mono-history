using System;
using System.Collections;
using UnityEngine;

public class MVGUILightSettingsBox : UXCustomDialogBox
{
	public UXPlane colorCube;

	public UXSlider redSlider;

	public UXSlider greenSlider;

	public UXSlider blueSlider;

	public UXSlider rangeSlider;

	public UXSlider intensitySlider;

	private float r;

	private float g;

	private float b;

	private float range;

	private float intensity;

	public override void OnShowDialog()
	{
		base.OnShowDialog();
		r = redSlider.Value;
		g = greenSlider.Value;
		b = blueSlider.Value;
		range = rangeSlider.Value;
		intensity = intensitySlider.Value;
		AddListeners();
		UpdateColor();
	}

	private void AddListeners()
	{
		UXSlider uXSlider = redSlider;
		uXSlider.OnValueChangedIntermediate = (UXSlider.OnValueChangedIntermediateDelegate)Delegate.Combine(uXSlider.OnValueChangedIntermediate, (UXSlider.OnValueChangedIntermediateDelegate)((UXSlider s, float v) =>
		{
			r = v;
			UpdateColor();
			FireIntermediateResult();
		}));
		UXSlider uXSlider2 = greenSlider;
		uXSlider2.OnValueChangedIntermediate = (UXSlider.OnValueChangedIntermediateDelegate)Delegate.Combine(uXSlider2.OnValueChangedIntermediate, (UXSlider.OnValueChangedIntermediateDelegate)((UXSlider s, float v) =>
		{
			g = v;
			UpdateColor();
			FireIntermediateResult();
		}));
		UXSlider uXSlider3 = blueSlider;
		uXSlider3.OnValueChangedIntermediate = (UXSlider.OnValueChangedIntermediateDelegate)Delegate.Combine(uXSlider3.OnValueChangedIntermediate, (UXSlider.OnValueChangedIntermediateDelegate)((UXSlider s, float v) =>
		{
			b = v;
			UpdateColor();
			FireIntermediateResult();
		}));
		UXSlider uXSlider4 = rangeSlider;
		uXSlider4.OnValueChangedIntermediate = (UXSlider.OnValueChangedIntermediateDelegate)Delegate.Combine(uXSlider4.OnValueChangedIntermediate, (UXSlider.OnValueChangedIntermediateDelegate)((UXSlider s, float v) =>
		{
			range = v;
			FireIntermediateResult();
		}));
		UXSlider uXSlider5 = intensitySlider;
		uXSlider5.OnValueChangedIntermediate = (UXSlider.OnValueChangedIntermediateDelegate)Delegate.Combine(uXSlider5.OnValueChangedIntermediate, (UXSlider.OnValueChangedIntermediateDelegate)((UXSlider s, float v) =>
		{
			intensity = v;
			FireIntermediateResult();
		}));
		UXSlider uXSlider6 = redSlider;
		uXSlider6.OnValueChanged = (UXSlider.OnValueChangedDelegate)Delegate.Combine(uXSlider6.OnValueChanged, (UXSlider.OnValueChangedDelegate)((UXSlider s) =>
		{
			r = s.Value;
			UpdateColor();
		}));
		UXSlider uXSlider7 = greenSlider;
		uXSlider7.OnValueChanged = (UXSlider.OnValueChangedDelegate)Delegate.Combine(uXSlider7.OnValueChanged, (UXSlider.OnValueChangedDelegate)((UXSlider s) =>
		{
			g = s.Value;
			UpdateColor();
		}));
		UXSlider uXSlider8 = blueSlider;
		uXSlider8.OnValueChanged = (UXSlider.OnValueChangedDelegate)Delegate.Combine(uXSlider8.OnValueChanged, (UXSlider.OnValueChangedDelegate)((UXSlider s) =>
		{
			b = s.Value;
			UpdateColor();
		}));
		UXSlider uXSlider9 = rangeSlider;
		uXSlider9.OnValueChanged = (UXSlider.OnValueChangedDelegate)Delegate.Combine(uXSlider9.OnValueChanged, (UXSlider.OnValueChangedDelegate)((UXSlider s) =>
		{
			range = s.Value;
		}));
		UXSlider uXSlider10 = intensitySlider;
		uXSlider10.OnValueChanged = (UXSlider.OnValueChangedDelegate)Delegate.Combine(uXSlider10.OnValueChanged, (UXSlider.OnValueChangedDelegate)((UXSlider s) =>
		{
			intensity = s.Value;
		}));
	}

	public override object GetResult()
	{
		Hashtable hashtable = new Hashtable();
		hashtable.Add("color", new float[3] { r, g, b });
		hashtable.Add("range", range);
		hashtable.Add("intensity", intensity);
		return hashtable;
	}

	private void UpdateColor()
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		colorCube.SetColor(new Color(r, g, b), string.Empty);
	}
}
