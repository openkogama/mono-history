using System;
using System.Collections;

public class MVGUIRoundCubeSettingsBox : UXCustomDialogBox
{
	public UXText intervalValueLabel;

	public UXSlider intervalSlider;

	public UXComboBox winningConditionComboBox;

	private float intervalIntermediate;

	private int winningConditionIntermediate;

	public override object GetResult()
	{
		Hashtable hashtable = new Hashtable();
		hashtable.Add("interval", Convert.ToInt32(intervalIntermediate));
		hashtable.Add("winningCondition", winningConditionIntermediate);
		return hashtable;
	}

	public override void OnShowDialog()
	{
		base.OnShowDialog();
		UpdateIntervalTime(intervalSlider.Value);
		UXSlider uXSlider = intervalSlider;
		uXSlider.OnValueChanged = (UXSlider.OnValueChangedDelegate)Delegate.Combine(uXSlider.OnValueChanged, (UXSlider.OnValueChangedDelegate)((UXSlider slider) =>
		{
			UpdateIntervalTime(slider.Value);
		}));
		UXSlider uXSlider2 = intervalSlider;
		uXSlider2.OnValueChangedIntermediate = (UXSlider.OnValueChangedIntermediateDelegate)Delegate.Combine(uXSlider2.OnValueChangedIntermediate, (UXSlider.OnValueChangedIntermediateDelegate)((UXSlider slider, float v) =>
		{
			UpdateIntervalTime(v);
		}));
		UXComboBox uXComboBox = winningConditionComboBox;
		uXComboBox.OnComboBoxItemSelect = (UXComboBox.OnComboBoxItemSelectDelegate)Delegate.Combine(uXComboBox.OnComboBoxItemSelect, (UXComboBox.OnComboBoxItemSelectDelegate)((int item) =>
		{
			winningConditionIntermediate = item;
		}));
	}

	private void UpdateIntervalTime(float v)
	{
		intervalIntermediate = v;
		intervalValueLabel.Text = $"{Convert.ToInt32(v) / 60:00}:{Convert.ToInt32(v) % 60:00}";
	}

	public override void OnPositiveClose()
	{
		base.OnPositiveClose();
	}
}
