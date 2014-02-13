using System;

public class MVGUIMovableSpeedSettingsBox : UXCustomDialogBox
{
	public UXText speedValueLabel;

	public UXSlider speedSlider;

	public override object GetResult()
	{
		return speedSlider.Value;
	}

	public override void OnShowDialog()
	{
		base.OnShowDialog();
		UpdateSpeedText(speedSlider.Value);
		UXSlider uXSlider = speedSlider;
		uXSlider.OnValueChanged = (UXSlider.OnValueChangedDelegate)Delegate.Combine(uXSlider.OnValueChanged, (UXSlider.OnValueChangedDelegate)((UXSlider slider) =>
		{
			UpdateSpeedText(slider.Value);
		}));
		UXSlider uXSlider2 = speedSlider;
		uXSlider2.OnValueChangedIntermediate = (UXSlider.OnValueChangedIntermediateDelegate)Delegate.Combine(uXSlider2.OnValueChangedIntermediate, (UXSlider.OnValueChangedIntermediateDelegate)((UXSlider slider, float v) =>
		{
			UpdateSpeedText(v);
		}));
	}

	private void UpdateSpeedText(float v)
	{
		speedValueLabel.Text = $"{v:0.0}";
	}
}
