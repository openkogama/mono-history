using MV.WorldObject.Subscription;
using UnityEngine;

public class SubscriberBenefitUIHandler : MonoBehaviour
{
	[SerializeField]
	private GameObject SubscriberUI;

	[SerializeField]
	private GameObject NonSubscriberUI;

	[SerializeField]
	private SubscriptionBenefit benefitType;

	private void Start()
	{
		if (MVGameControllerBase.Game.LocalPlayer.SubscriptionRules.HasBenefit(benefitType))
		{
			if (SubscriberUI != null && !SubscriberUI.activeSelf)
			{
				SubscriberUI.SetActive(value: true);
			}
			if (NonSubscriberUI != null && NonSubscriberUI.activeSelf)
			{
				NonSubscriberUI.SetActive(value: false);
			}
		}
		else
		{
			if (NonSubscriberUI != null && !NonSubscriberUI.activeSelf)
			{
				NonSubscriberUI.SetActive(value: true);
			}
			if (SubscriberUI != null && SubscriberUI.activeSelf)
			{
				SubscriberUI.SetActive(value: false);
			}
		}
	}
}
