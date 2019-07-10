using System;
using System.Collections.Generic;
using MV.Common;
using UnityEngine;
using UnityEngine.EventSystems;

public class LevelRewardsLobbyState : MonoBehaviour
{
	[SerializeField]
	private LevelRewardAnimation levelRewardAnimation;

	private static int previousNextLevelRewardShown;

	private void Start()
	{
		Dictionary<int, int> rewardsToShow = MVGameControllerBase.Game.LevelRewardsManager.RewardsToShow;
		if (rewardsToShow.Count == 0)
		{
			LevelRewardsManager levelRewardsManager = MVGameControllerBase.Game.LevelRewardsManager;
			levelRewardsManager.OnRewardsReturned = (Action)Delegate.Combine(levelRewardsManager.OnRewardsReturned, new Action(ShowRewards));
		}
		else
		{
			ShowRewards();
		}
	}

	private void OnEnable()
	{
		if (MVGameControllerBase.SpawnRoleDataMediatorLocal.SpawnRoleModeTypeWrapper.IsInMode(SpawnRoleModeType.Playing) || MVGameControllerBase.SpawnRoleDataMediatorLocal.SpawnRoleModeTypeWrapper.IsInMode(SpawnRoleModeType.Dead))
		{
			ShowRewards();
		}
	}

	private void ShowRewards()
	{
		if (!gameObject.activeInHierarchy)
		{
			return;
		}
		Dictionary<int, int> rewardsToShow = MVGameControllerBase.Game.LevelRewardsManager.RewardsToShow;
		if (rewardsToShow.Count > 0)
		{
			LevelRewardAnimation levelReward = UnityEngine.Object.Instantiate(levelRewardAnimation);
			ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
			{
				x.Push(levelReward.gameObject, UIPushOption.Blocking | UIPushOption.HideAll, null, UIGroupFlags.Popup);
			});
			levelReward.Initialize(rewardsToShow);
			MVGameControllerBase.Game.LevelRewardsManager.ClearRewards();
		}
	}

	public void ShowLevelNotification()
	{
		int key = MVGameControllerBase.Game.LevelRewardsManager.NextReward.Key;
		if (key != previousNextLevelRewardShown)
		{
			NotificationController.PushNotification(NotificationType.NextLevelReward, NotificationLifetime.SuperHigh);
			previousNextLevelRewardShown = key;
		}
	}

	private void OnDestroy()
	{
		if (MVGameControllerBase.IsAlive && MVGameControllerBase.Game != null)
		{
			LevelRewardsManager levelRewardsManager = MVGameControllerBase.Game.LevelRewardsManager;
			levelRewardsManager.OnRewardsReturned = (Action)Delegate.Remove(levelRewardsManager.OnRewardsReturned, new Action(ShowRewards));
		}
	}
}
