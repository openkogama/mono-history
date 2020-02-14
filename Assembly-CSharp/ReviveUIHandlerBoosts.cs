using System.Collections.Generic;
using Assets.Scripts.AdIntegration;
using MV.Common;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.UI;

public class ReviveUIHandlerBoosts : ReviveUIHandlerBase
{
	[SerializeField]
	private GameObject defaultUI;

	[SerializeField]
	private GameObject tempClassUI;

	[SerializeField]
	private CurrentSpawnRolePreviewer spawnRolePreviewer;

	[SerializeField]
	private GameObject backgroundTier1;

	[SerializeField]
	private GameObject backgroundTier2;

	[SerializeField]
	private GameObject backgroundTier3;

	[SerializeField]
	private BoostImageController boostImageSelector;

	[SerializeField]
	private GameObject boostContent;

	private GamePassTier tier;

	public override void Initialize(UnityAction onContinueClicked)
	{
		base.Initialize(onContinueClicked);
		tier = GamePassesManager.PlayerPlanetData.previewGamePassTier;
		bool flag = IsInTempClass();
		bool flag2 = IsInTempTier();
		defaultUI.SetActive(!flag && flag2);
		tempClassUI.SetActive(flag || !flag2);
		BoostController boostController = MVGameControllerBase.LocalPlayer.BoostController;
		Dictionary<BoostType, Boost>.ValueCollection activeBoosts = boostController.GetActiveBoosts();
		foreach (Boost item in activeBoosts)
		{
			Image image = Object.Instantiate(boostImageSelector.GetBoostVisualization(item.Type));
			image.transform.SetParent(boostContent.transform, worldPositionStays: false);
		}
		if (flag || !flag2)
		{
			spawnRolePreviewer.SetupPreviewer((int)targetTexture.rectTransform.rect.width, (int)targetTexture.rectTransform.rect.height);
			ChangeBackground();
		}
	}

	private void ChangeBackground()
	{
		backgroundTier1.SetActive(tier == GamePassTier.Tier1);
		backgroundTier2.SetActive(tier == GamePassTier.Tier2);
		backgroundTier3.SetActive(tier == GamePassTier.Tier3);
	}

	protected override void OnAdFinishedContinue()
	{
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
		{
			x.Pop();
		});
		MVGameControllerBase.GameEventManager.AvatarCommandsPlayMode.EnterPlayingState();
	}

	private bool IsInTempClass()
	{
		GamePassTier gamePassTier = MVGameControllerBase.LocalPlayer.SpawnRoleDataMediator.TierRequirement;
		GamePassTier gamePassTier2 = GamePassesManager.PlayerPlanetData.gamePassTier;
		return (int)gamePassTier > (int)gamePassTier2;
	}

	private bool IsInTempTier()
	{
		GamePassTier gamePassTier = MVGameControllerBase.LocalPlayer.SpawnRoleDataMediator.TierRequirement;
		GamePassTier previewGamePassTier = GamePassesManager.PlayerPlanetData.previewGamePassTier;
		return (int)gamePassTier > (int)previewGamePassTier;
	}

	protected override void Update()
	{
		bool isBlocked = false;
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
		{
			isBlocked = x.IsUIElementBlocked(gameObject);
		});
		if (!isBlocked)
		{
			base.Update();
		}
	}

	protected override void OnRewardedAdWatched(RewardedAdResult result)
	{
		Debug.Log("RESULT OF REBOOST: " + result);
		switch (result)
		{
		case RewardedAdResult.RewardUnlocked:
			OnAdFinishedContinue();
			break;
		case RewardedAdResult.ErrorTimeout:
			continueButton.onClick.Invoke();
			break;
		case RewardedAdResult.ErrorClient:
		case RewardedAdResult.ErrorInternal:
		case RewardedAdResult.RewardNotUnlocked:
		{
			NotificationPopup popup = Object.Instantiate(errorNotification);
			popup.Initialize(MVGameControllerBase.AdManager.RewardedAdNotAvailableText, TM._("No Ad Available"));
			ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
			{
				x.Push(popup.gameObject, UIPushOption.Blocking, () =>
				{
					continueButton.onClick.Invoke();
				}, UIGroupFlags.Popup);
			});
			break;
		}
		}
	}
}
