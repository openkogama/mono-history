using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GoldRewardNotification : Notification
{
	[SerializeField]
	private Button claimRewardBtn;

	public override void Initialize(Dictionary<object, object> data)
	{
		base.Initialize(data);
		Lifetime = (NotificationLifetime)(int)data[(byte)2];
	}

	public void RewardClicked()
	{
		if (!TimedPlayReward.IsCollected)
		{
			ExecuteEvents.ExecuteHierarchy(gameObject, null, (IOfferController x, BaseEventData y) =>
			{
				x.RequestShowOffer(OnFinishedViewingAd);
			});
			TimedPlayReward.IsCollected = true;
			if (TimedPlayReward.CollectedChanged != null)
			{
				TimedPlayReward.CollectedChanged();
			}
		}
	}

	private void OnFinishedViewingAd()
	{
		ParticleSystem particleSystem = Object.Instantiate(PrefabPool.Instance.GoldExplosion);
		particleSystem.transform.parent = MVGameControllerBase.WOCM.AvatarLocal.Transform;
		particleSystem.transform.localPosition = new Vector3(0f, 1f, 0f);
	}
}
