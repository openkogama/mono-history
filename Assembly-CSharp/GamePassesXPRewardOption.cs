using MV.Common;
using MV.WorldObject.AntiCheat;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GamePassesXPRewardOption : MonoBehaviour
{
	[SerializeField]
	private Slider slider;

	[SerializeField]
	private InputField inputField;

	[SerializeField]
	private Text bonusXPAmountText;

	private GamePassTier gamePassTier;

	public void Initialize(GamePassesXpRewardInfo rewardInfo, GamePassTier gamePassTier)
	{
		this.gamePassTier = gamePassTier;
		RangeValidator<int> xPRewardRangeValidator = GamePassProgressionController.GetXPRewardRangeValidator(gamePassTier);
		slider.minValue = xPRewardRangeValidator.min;
		slider.maxValue = xPRewardRangeValidator.max;
		int xPReward = GamePassProgressionController.GetXPReward(gamePassTier);
		slider.value = xPReward;
		inputField.text = xPReward.ToString();
		UpdateBonusXpAmountText(xPReward);
	}

	public void OnSliderChange()
	{
		int xpRewardAmount = Mathf.FloorToInt(slider.value);
		inputField.text = xpRewardAmount.ToString();
		UpdateBonusXpAmountText(xpRewardAmount);
	}

	public void OnInputFieldChange()
	{
		if (float.TryParse(inputField.text, out var result))
		{
			result = Mathf.Clamp(result, slider.minValue, slider.maxValue);
			int num = Mathf.FloorToInt(result);
			inputField.text = num.ToString();
			slider.value = num;
			UpdateBonusXpAmountText(num);
		}
	}

	public void UpdateXPData(int xpAmount)
	{
		GamePassProgressionController.SetXPReward(gamePassTier, xpAmount);
	}

	public void OnOkayPressed()
	{
		UpdateXPData(Mathf.FloorToInt(slider.value));
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
		{
			x.Pop();
		});
	}

	private void UpdateBonusXpAmountText(int xpRewardAmount)
	{
		float num = SubscriberRewardDataManager.GetBaseXpAmount();
		bonusXPAmountText.text = Mathf.FloorToInt((float)xpRewardAmount * (1f + num / 100f)) + " XP for Elites";
	}
}
