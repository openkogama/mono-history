using System;
using System.Collections.Generic;

public class MVGUICameraSettingsBoxClassic : UXCustomDialogBox
{
	public UXText distanceToAvatarLabel;

	public UXSlider distanceToAvatarSlider;

	private float distanceToAvatarIntermediate;

	public override object GetResult()
	{
		Dictionary<object, object> dictionary = new Dictionary<object, object>();
		dictionary.Add("distanceToAvatar", distanceToAvatarIntermediate);
		return dictionary;
	}

	public override void OnShowDialog()
	{
		base.OnShowDialog();
		UpdateDistanceToAvatar(distanceToAvatarSlider.Value);
		UXSlider uXSlider = distanceToAvatarSlider;
		uXSlider.OnValueChanged = (UXSlider.OnValueChangedDelegate)Delegate.Combine(uXSlider.OnValueChanged, (UXSlider.OnValueChangedDelegate)((UXSlider slider) =>
		{
			UpdateDistanceToAvatar(slider.Value);
			FireIntermediateResult();
		}));
		UXSlider uXSlider2 = distanceToAvatarSlider;
		uXSlider2.OnValueChangedIntermediate = (UXSlider.OnValueChangedIntermediateDelegate)Delegate.Combine(uXSlider2.OnValueChangedIntermediate, (UXSlider.OnValueChangedIntermediateDelegate)((UXSlider slider, float v) =>
		{
			UpdateDistanceToAvatar(v);
			FireIntermediateResult();
		}));
	}

	private void UpdateDistanceToAvatar(float v)
	{
		distanceToAvatarIntermediate = v;
		distanceToAvatarLabel.Text = FormatValue(distanceToAvatarIntermediate);
	}

	private string FormatValue(float v)
	{
		return $"{v:0.0}";
	}
}
