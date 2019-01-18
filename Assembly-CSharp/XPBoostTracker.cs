using System;
using MV.WorldObject.Subscription;
using MV.WorldObject.Subscription.SubscriptionRules;
using UnityEngine;
using UnityEngine.UI;

public class XPBoostTracker : MonoBehaviour
{
	[SerializeField]
	private Text XPBoostText;

	private int memberCount;

	private int localActorNr;

	private void Start()
	{
		localActorNr = MVGameControllerBase.Game.LocalPlayer.ActorNr;
		UpdateMemberCount();
		UpdateBoostText();
		MVPlayerContainer mVPlayerContainer = MVGameControllerBase.Game.MVPlayerContainer;
		mVPlayerContainer.OnPlayerListChanged = (Action)Delegate.Combine(mVPlayerContainer.OnPlayerListChanged, new Action(OnPlayerJoinOrLeave));
	}

	private void UpdateMemberCount()
	{
		memberCount = 0;
		foreach (MVPlayer value in MVGameControllerBase.Game.MVPlayerContainer.Values)
		{
			if (value != null && value.ActorNr != localActorNr && MVGameControllerBase.Game.MVPlayerContainer.TryGetValue(value.ActorNr, out var player) && player.SubscriptionRules.HasBenefit(SubscriptionBenefit.XPBoost))
			{
				memberCount++;
			}
		}
	}

	private void UpdateBoostText()
	{
		int totalXPBoost = MVGameControllerBase.Game.LocalPlayer.SubscriptionRules.GetRule<XpBooster>(SubscriptionBenefit.XPBoost).GetTotalXPBoost(memberCount);
		XPBoostText.text = "+" + totalXPBoost + "%";
		if (totalXPBoost <= 0)
		{
			gameObject.SetActive(value: false);
		}
		else
		{
			gameObject.SetActive(value: true);
		}
	}

	private void OnPlayerJoinOrLeave()
	{
		UpdateMemberCount();
		UpdateBoostText();
	}

	private void OnDestroy()
	{
		if (MVGameControllerBase.IsAlive)
		{
			MVPlayerContainer mVPlayerContainer = MVGameControllerBase.Game.MVPlayerContainer;
			mVPlayerContainer.OnPlayerListChanged = (Action)Delegate.Remove(mVPlayerContainer.OnPlayerListChanged, new Action(OnPlayerJoinOrLeave));
		}
	}
}
