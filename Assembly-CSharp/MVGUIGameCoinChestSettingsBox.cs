using System;
using System.Collections.Generic;

public class MVGUIGameCoinChestSettingsBox : UXCustomDialogBox
{
	public UXText gameCoinAmountValueLabel;

	public UXSlider gameCoinAmountSlider;

	private int gameCoinAmountIntermediate;

	public override object GetResult()
	{
		Dictionary<object, object> dictionary = new Dictionary<object, object>();
		dictionary.Add("gameCoinAmount", gameCoinAmountIntermediate);
		return dictionary;
	}

	public override void OnShowDialog()
	{
		base.OnShowDialog();
		UpdateGameCoinAmount(gameCoinAmountSlider.Value);
		UXSlider uXSlider = gameCoinAmountSlider;
		uXSlider.OnValueChanged = (UXSlider.OnValueChangedDelegate)Delegate.Combine(uXSlider.OnValueChanged, (UXSlider.OnValueChangedDelegate)((UXSlider slider) =>
		{
			UpdateGameCoinAmount(slider.Value);
			FireIntermediateResult();
		}));
		UXSlider uXSlider2 = gameCoinAmountSlider;
		uXSlider2.OnValueChangedIntermediate = (UXSlider.OnValueChangedIntermediateDelegate)Delegate.Combine(uXSlider2.OnValueChangedIntermediate, (UXSlider.OnValueChangedIntermediateDelegate)((UXSlider slider, float v) =>
		{
			UpdateGameCoinAmount(v);
			FireIntermediateResult();
		}));
	}

	private void UpdateGameCoinAmount(float v)
	{
		gameCoinAmountIntermediate = (int)v;
		gameCoinAmountValueLabel.Text = FormatValue(gameCoinAmountIntermediate);
	}

	private string FormatValue(float v)
	{
		return $"{v:0}";
	}
}
