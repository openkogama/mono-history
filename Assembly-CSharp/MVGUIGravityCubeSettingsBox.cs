using System;
using System.Collections.Generic;

public class MVGUIGravityCubeSettingsBox : UXCustomDialogBox
{
	public UXText gravityLabel;

	public UXSlider gravitySlider;

	private float gravityIntermediate;

	public override object GetResult()
	{
		Dictionary<object, object> dictionary = new Dictionary<object, object>();
		dictionary.Add("gravity", gravityIntermediate);
		return dictionary;
	}

	public override void OnShowDialog()
	{
		base.OnShowDialog();
		UpdateGravity(gravitySlider.Value);
		UXSlider uXSlider = gravitySlider;
		uXSlider.OnValueChanged = (UXSlider.OnValueChangedDelegate)Delegate.Combine(uXSlider.OnValueChanged, (UXSlider.OnValueChangedDelegate)((UXSlider slider) =>
		{
			UpdateGravity(slider.Value);
			FireIntermediateResult();
		}));
		UXSlider uXSlider2 = gravitySlider;
		uXSlider2.OnValueChangedIntermediate = (UXSlider.OnValueChangedIntermediateDelegate)Delegate.Combine(uXSlider2.OnValueChangedIntermediate, (UXSlider.OnValueChangedIntermediateDelegate)((UXSlider slider, float v) =>
		{
			UpdateGravity(v);
			FireIntermediateResult();
		}));
	}

	private void UpdateGravity(float v)
	{
		gravityIntermediate = v;
		gravityLabel.Text = FormatValue(gravityIntermediate);
	}

	private string FormatValue(float v)
	{
		return $"{v:0.0}";
	}
}
