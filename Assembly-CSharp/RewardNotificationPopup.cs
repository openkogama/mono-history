using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class RewardNotificationPopup : Notification
{
	[SerializeField]
	private RewardMinigame rewardMinigamePrefab;

	public override void Initialize(Dictionary<object, object> data)
	{
		base.Initialize(data);
		Lifetime = (NotificationLifetime)(int)data[(byte)2];
	}

	public void OnRewardClicked()
	{
		RewardMinigame rewardMinigame = Object.Instantiate(rewardMinigamePrefab);
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
		{
			x.Push(rewardMinigame.gameObject, UIPushOption.Blocking, null, UIGroupFlags.GameObjectUI);
		});
	}
}
