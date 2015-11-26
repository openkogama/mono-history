using System;
using System.Collections.Generic;

public class GUIStarTriggerSettingsBox : UXCustomDialogBox
{
	public UXText starTextLabel;

	public UXText starsNeededNumberLabel;

	public UXText noStarsTextLabel;

	public UXSlider starsNeededSlider;

	private int starsNeededIntermediate;

	public override object GetResult()
	{
		Dictionary<object, object> dictionary = new Dictionary<object, object>();
		dictionary.Add("starAmount", starsNeededIntermediate);
		return dictionary;
	}

	public override void OnShowDialog()
	{
		base.OnShowDialog();
		UpdateStarsNeeded(starsNeededSlider.Value);
		UXSlider uXSlider = starsNeededSlider;
		uXSlider.OnValueChanged = (UXSlider.OnValueChangedDelegate)Delegate.Combine(uXSlider.OnValueChanged, (UXSlider.OnValueChangedDelegate)((UXSlider slider) =>
		{
			UpdateStarsNeeded(slider.Value);
			FireIntermediateResult();
		}));
		UXSlider uXSlider2 = starsNeededSlider;
		uXSlider2.OnValueChangedIntermediate = (UXSlider.OnValueChangedIntermediateDelegate)Delegate.Combine(uXSlider2.OnValueChangedIntermediate, (UXSlider.OnValueChangedIntermediateDelegate)((UXSlider slider, float v) =>
		{
			UpdateStarsNeeded(v);
			FireIntermediateResult();
		}));
	}

	private void UpdateStarsNeeded(float v)
	{
		starsNeededIntermediate = (int)v;
		starsNeededNumberLabel.Text = starsNeededIntermediate.ToString();
		AllCollectiblesCollectedClient singletonWinnerConditionByType = MVGameControllerBase.Game.WinningConditionManager.GetSingletonWinnerConditionByType<AllCollectiblesCollectedClient>();
		noStarsTextLabel.Text = "There are not enough stars in level to use Plate.";
		if (singletonWinnerConditionByType != null)
		{
			if (singletonWinnerConditionByType.Limit > starsNeededIntermediate)
			{
				noStarsTextLabel.Text = "Add stars to increase slider limit.";
			}
		}
		else if (starsNeededIntermediate == 0)
		{
			noStarsTextLabel.Text = "Add stars to increase slider limit.";
		}
	}
}
