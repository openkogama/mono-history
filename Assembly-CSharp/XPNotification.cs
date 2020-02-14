using System.Collections.Generic;
using MV.WorldObject.Subscription;
using MV.WorldObject.Subscription.SubscriptionRules;
using UnityEngine;
using UnityEngine.UI;

public class XPNotification : Notification
{
	[SerializeField]
	private Text AmountLabel;

	[SerializeField]
	private NotificationSlideOut slider;

	[SerializeField]
	private GameObject boostedNotification;

	[SerializeField]
	private GameObject defaultNotification;

	[SerializeField]
	private XPNotificationBoostedBehaviour boostedBehaviour;

	private const float boostedXpNotificationLifeTime = 5f;

	protected override NotificationLifetime Lifetime => NotificationLifetime.High;

	public override void Initialize(Dictionary<object, object> data)
	{
		base.Initialize(data);
		int num = int.Parse(data[(byte)4].ToString());
		int membersCount = (int)data[(byte)19];
		int boostedXp = MVGameControllerBase.Game.LocalPlayer.SubscriptionRules.GetRule<XpBooster>(SubscriptionBenefit.XPBoost).GetBoostedXp(num, membersCount);
		int num2 = boostedXp - num;
		if (num2 > 0)
		{
			AmountLabel.text = boostedXp.ToString() + " XP! (" + num2 + " from boost)";
			boostedNotification.SetActive(value: true);
			defaultNotification.SetActive(value: false);
		}
		else
		{
			AmountLabel.text = boostedXp + " XP!";
			boostedNotification.SetActive(value: false);
			defaultNotification.SetActive(value: true);
			timeSinceStart = (float)Lifetime - 5f;
		}
		boostedBehaviour.Initialize(boostedXp, num);
	}

	protected override void Update()
	{
		timeSinceStart += Time.deltaTime;
		if (timeSinceStart >= (float)Lifetime)
		{
			pool.Return(this);
		}
	}
}
