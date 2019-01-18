using MV.WorldObject.Subscription;
using MV.WorldObject.Subscription.SubscriptionRules;
using UnityEngine;
using UnityEngine.UI;

public class GameCoinMeterBoostHandler : MonoBehaviour
{
	[SerializeField]
	private float ExpandAmount;

	[SerializeField]
	private LayoutElement elementToExpand;

	[SerializeField]
	private Text boostAmountText;

	private void Start()
	{
		if (MVGameControllerBase.Game.LocalPlayer.SubscriptionRules.HasBenefit(SubscriptionBenefit.GameCoinBoost))
		{
			elementToExpand.minWidth += ExpandAmount;
			boostAmountText.text = "x" + MVGameControllerBase.Game.LocalPlayer.SubscriptionRules.GetRule<GameCoinBooster>(SubscriptionBenefit.GameCoinBoost).GetBoostedGameCoins(1);
		}
		else
		{
			gameObject.SetActive(value: false);
		}
	}
}
