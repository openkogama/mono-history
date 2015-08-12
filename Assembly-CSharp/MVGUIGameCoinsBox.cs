using System;
using System.Collections.Generic;

public class MVGUIGameCoinsBox : UXCustomDialogBox
{
	public UXText gameCoinsValueLabel;

	public UXSlider gameCoinsSlider;

	private int gameCoinsIntermediate;

	public override object GetResult()
	{
		Dictionary<object, object> dictionary = new Dictionary<object, object>();
		dictionary.Add("gameCoinAmount", gameCoinsIntermediate);
		return dictionary;
	}

	public override void OnShowDialog()
	{
		base.OnShowDialog();
		UpdateGameCoins(gameCoinsSlider.Value);
		UXSlider uXSlider = gameCoinsSlider;
		uXSlider.OnValueChanged = (UXSlider.OnValueChangedDelegate)Delegate.Combine(uXSlider.OnValueChanged, (UXSlider.OnValueChangedDelegate)((UXSlider slider) =>
		{
			UpdateGameCoins(slider.Value);
			FireIntermediateResult();
		}));
		UXSlider uXSlider2 = gameCoinsSlider;
		uXSlider2.OnValueChangedIntermediate = (UXSlider.OnValueChangedIntermediateDelegate)Delegate.Combine(uXSlider2.OnValueChangedIntermediate, (UXSlider.OnValueChangedIntermediateDelegate)((UXSlider slider, float v) =>
		{
			UpdateGameCoins(v);
			FireIntermediateResult();
		}));
	}

	private void UpdateGameCoins(float v)
	{
		gameCoinsIntermediate = (int)v;
		gameCoinsValueLabel.Text = FormatValue(gameCoinsIntermediate);
	}

	private string FormatValue(float v)
	{
		return $"{v:0}";
	}
}
