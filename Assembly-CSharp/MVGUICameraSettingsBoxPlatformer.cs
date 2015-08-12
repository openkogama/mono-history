using System;
using System.Collections.Generic;

public class MVGUICameraSettingsBoxPlatformer : UXCustomDialogBox
{
	public UXText heightLabel;

	public UXText distanceToAvatarLabel;

	public UXText smoothnessLabel;

	public UXText speedDistanceModifierLabel;

	public UXSlider heightSlider;

	public UXSlider distanceToAvatarSlider;

	public UXSlider smoothnessSlider;

	public UXSlider speedDistanceModifierSlider;

	private float heightIntermediate;

	private float distanceToAvatarIntermediate;

	private float smoothnessIntermediate;

	private float speedDistanceModifierIntermediate;

	private bool avatarWorldCollisionIntermediate;

	public override object GetResult()
	{
		Dictionary<object, object> dictionary = new Dictionary<object, object>();
		dictionary.Add("height", heightIntermediate);
		dictionary.Add("distanceToAvatar", distanceToAvatarIntermediate);
		dictionary.Add("smoothness", smoothnessIntermediate);
		dictionary.Add("speedDistanceModifier", speedDistanceModifierIntermediate);
		dictionary.Add("avatarWorldCollision", avatarWorldCollisionIntermediate);
		return dictionary;
	}

	public override void OnShowDialog()
	{
		base.OnShowDialog();
		UpdateHeight(heightSlider.Value);
		UpdateDistanceToAvatar(distanceToAvatarSlider.Value);
		UpdateSmoothness(smoothnessSlider.Value);
		UpdateSpeedDistanceModifier(speedDistanceModifierSlider.Value);
		UXSlider uXSlider = heightSlider;
		uXSlider.OnValueChanged = (UXSlider.OnValueChangedDelegate)Delegate.Combine(uXSlider.OnValueChanged, (UXSlider.OnValueChangedDelegate)((UXSlider slider) =>
		{
			UpdateHeight(slider.Value);
			FireIntermediateResult();
		}));
		UXSlider uXSlider2 = distanceToAvatarSlider;
		uXSlider2.OnValueChanged = (UXSlider.OnValueChangedDelegate)Delegate.Combine(uXSlider2.OnValueChanged, (UXSlider.OnValueChangedDelegate)((UXSlider slider) =>
		{
			UpdateDistanceToAvatar(slider.Value);
			FireIntermediateResult();
		}));
		UXSlider uXSlider3 = smoothnessSlider;
		uXSlider3.OnValueChanged = (UXSlider.OnValueChangedDelegate)Delegate.Combine(uXSlider3.OnValueChanged, (UXSlider.OnValueChangedDelegate)((UXSlider slider) =>
		{
			UpdateSmoothness(slider.Value);
			FireIntermediateResult();
		}));
		UXSlider uXSlider4 = speedDistanceModifierSlider;
		uXSlider4.OnValueChanged = (UXSlider.OnValueChangedDelegate)Delegate.Combine(uXSlider4.OnValueChanged, (UXSlider.OnValueChangedDelegate)((UXSlider slider) =>
		{
			UpdateSpeedDistanceModifier(slider.Value);
			FireIntermediateResult();
		}));
		UXSlider uXSlider5 = heightSlider;
		uXSlider5.OnValueChangedIntermediate = (UXSlider.OnValueChangedIntermediateDelegate)Delegate.Combine(uXSlider5.OnValueChangedIntermediate, (UXSlider.OnValueChangedIntermediateDelegate)((UXSlider slider, float v) =>
		{
			UpdateHeight(v);
			FireIntermediateResult();
		}));
		UXSlider uXSlider6 = distanceToAvatarSlider;
		uXSlider6.OnValueChangedIntermediate = (UXSlider.OnValueChangedIntermediateDelegate)Delegate.Combine(uXSlider6.OnValueChangedIntermediate, (UXSlider.OnValueChangedIntermediateDelegate)((UXSlider slider, float v) =>
		{
			UpdateDistanceToAvatar(v);
			FireIntermediateResult();
		}));
		UXSlider uXSlider7 = smoothnessSlider;
		uXSlider7.OnValueChangedIntermediate = (UXSlider.OnValueChangedIntermediateDelegate)Delegate.Combine(uXSlider7.OnValueChangedIntermediate, (UXSlider.OnValueChangedIntermediateDelegate)((UXSlider slider, float v) =>
		{
			UpdateSmoothness(v);
			FireIntermediateResult();
		}));
		UXSlider uXSlider8 = speedDistanceModifierSlider;
		uXSlider8.OnValueChangedIntermediate = (UXSlider.OnValueChangedIntermediateDelegate)Delegate.Combine(uXSlider8.OnValueChangedIntermediate, (UXSlider.OnValueChangedIntermediateDelegate)((UXSlider slider, float v) =>
		{
			UpdateSpeedDistanceModifier(v);
			FireIntermediateResult();
		}));
	}

	private void UpdateHeight(float v)
	{
		heightIntermediate = v;
		heightLabel.Text = FormatValue(heightIntermediate);
	}

	private void UpdateDistanceToAvatar(float v)
	{
		distanceToAvatarIntermediate = v;
		distanceToAvatarLabel.Text = FormatValue(distanceToAvatarIntermediate);
	}

	private void UpdateSmoothness(float v)
	{
		smoothnessIntermediate = v;
		smoothnessLabel.Text = FormatValueTwoDigits(smoothnessIntermediate);
	}

	private void UpdateSpeedDistanceModifier(float v)
	{
		speedDistanceModifierIntermediate = v;
		speedDistanceModifierLabel.Text = FormatValue(speedDistanceModifierIntermediate);
	}

	private string FormatValue(float v)
	{
		return $"{v:0.0}";
	}

	private string FormatValueTwoDigits(float v)
	{
		return $"{v:0.00}";
	}
}
