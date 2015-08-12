using System;
using System.Collections.Generic;

public class MVGUIRoundCubeSettingsBox : UXCustomDialogBox
{
	public UXText intervalValueLabel;

	public UXSlider intervalSlider;

	public UXComboBox winningConditionComboBox;

	private float intervalIntermediate;

	private int winningConditionIntermediate;

	public override object GetResult()
	{
		Dictionary<object, object> dictionary = new Dictionary<object, object>();
		dictionary.Add("interval", Convert.ToInt32(intervalIntermediate));
		dictionary.Add("winningCondition", winningConditionIntermediate);
		return dictionary;
	}

	public override void OnShowDialog()
	{
		base.OnShowDialog();
		UpdateIntervalTime(intervalSlider.Value);
		winningConditionIntermediate = (int)MVGUISettingsDialogRoundCube.availableGameStatCounters[winningConditionComboBox.CurrentlySelectedItemIndex].gameStatCounterType;
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
			winningConditionIntermediate = (int)MVGUISettingsDialogRoundCube.availableGameStatCounters[item].gameStatCounterType;
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
