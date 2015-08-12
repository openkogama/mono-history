using System;
using System.Collections.Generic;
using UnityEngine;

public class MVGUISkyBoxSettingsBox : UXCustomDialogBox
{
	public UXPlane colorCube;

	public UXSlider redSlider;

	public UXSlider greenSlider;

	public UXSlider blueSlider;

	public UXSlider sunAngleSlider;

	public UXSlider fogDensitySlider;

	private float r;

	private float g;

	private float b;

	private float sunAngle;

	private float fogDensity;

	public override void OnShowDialog()
	{
		base.OnShowDialog();
		r = redSlider.Value;
		g = greenSlider.Value;
		b = blueSlider.Value;
		sunAngle = sunAngleSlider.Value;
		fogDensity = fogDensitySlider.Value;
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
		UXSlider uXSlider4 = sunAngleSlider;
		uXSlider4.OnValueChangedIntermediate = (UXSlider.OnValueChangedIntermediateDelegate)Delegate.Combine(uXSlider4.OnValueChangedIntermediate, (UXSlider.OnValueChangedIntermediateDelegate)((UXSlider s, float v) =>
		{
			sunAngle = v;
			FireIntermediateResult();
		}));
		UXSlider uXSlider5 = fogDensitySlider;
		uXSlider5.OnValueChangedIntermediate = (UXSlider.OnValueChangedIntermediateDelegate)Delegate.Combine(uXSlider5.OnValueChangedIntermediate, (UXSlider.OnValueChangedIntermediateDelegate)((UXSlider s, float v) =>
		{
			fogDensity = v;
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
		UXSlider uXSlider9 = sunAngleSlider;
		uXSlider9.OnValueChanged = (UXSlider.OnValueChangedDelegate)Delegate.Combine(uXSlider9.OnValueChanged, (UXSlider.OnValueChangedDelegate)((UXSlider s) =>
		{
			sunAngle = s.Value;
		}));
		UXSlider uXSlider10 = fogDensitySlider;
		uXSlider10.OnValueChanged = (UXSlider.OnValueChangedDelegate)Delegate.Combine(uXSlider10.OnValueChanged, (UXSlider.OnValueChangedDelegate)((UXSlider s) =>
		{
			fogDensity = s.Value;
		}));
	}

	public override object GetResult()
	{
		Dictionary<object, object> dictionary = new Dictionary<object, object>();
		dictionary.Add("color", new float[3] { r, g, b });
		dictionary.Add("sunAngle", sunAngle);
		dictionary.Add("fogDensity", fogDensity);
		return dictionary;
	}

	private void UpdateColor()
	{
		colorCube.SetColor(new Color(r, g, b), string.Empty);
	}
}
