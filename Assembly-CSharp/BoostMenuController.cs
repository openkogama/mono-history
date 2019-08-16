using System;
using System.Collections.Generic;
using Assets.Scripts.AdIntegration;
using MV.WorldObject.KogamaSettings.SpecializedSettingsTypes.GameBoosterSettings.GameBoosterSettingTypes;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BoostMenuController : MonoBehaviour, IBoostAdController, IEventSystemHandler
{
	[SerializeField]
	private BoostMenuItem boostPrefab;

	[SerializeField]
	private RectTransform boostItemsScrollRect;

	[SerializeField]
	private RectTransform boostItemsContent;

	private BoostType adRewardType;

	private Action<bool> boostUnlockedCallback;

	public void Initialize()
	{
		BoostController boostController = MVGameControllerBase.Game.LocalPlayer.BoostController;
		Dictionary<BoostType, Boost>.ValueCollection allBoosts = boostController.GetAllBoosts();
		MVGameBoosterDataObject singletonWorldObject = MVGameControllerBase.WOCM.GetSingletonWorldObject<MVGameBoosterDataObject>();
		List<GameBoosterSettingWithGoldSetting> activeSettingsList = singletonWorldObject.GameBoosterSettingsManager.ActiveSettingsList;
		foreach (Boost item in allBoosts)
		{
			for (int i = 0; i < activeSettingsList.Count; i++)
			{
				if (item.BoostKey == activeSettingsList[i].Key)
				{
					BoostMenuItem boostMenuItem = UnityEngine.Object.Instantiate(boostPrefab);
					boostMenuItem.transform.SetParent(boostItemsContent, worldPositionStays: false);
					boostMenuItem.Initialize(item, boostController.IsBoostActive(item.Type));
				}
			}
		}
		LayoutRebuilder.ForceRebuildLayoutImmediate(boostItemsContent);
		if (boostItemsScrollRect.rect.width < boostItemsContent.rect.width)
		{
			boostItemsContent.pivot = new Vector2(0f, 0.5f);
		}
		if (MVGameControllerBase.MainCameraManager.CamMaskMode == MaskMode.AvatarLobbyFocus)
		{
			MVGameControllerBase.MainCameraManager.CamMaskMode = MaskMode.SkyBoxOnly;
		}
	}

	public void TryShowAd(BoostType type, Action<bool> OnUnlockedCallback)
	{
		boostUnlockedCallback = OnUnlockedCallback;
		adRewardType = type;
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IModalPopupCreator x, BaseEventData y) =>
		{
			x.Create();
		});
		if (MVGameControllerBase.AdManager.ReadyForRewardedAdRequest)
		{
			MVGameControllerBase.AdManager.RequestRewardedAd(RewardedAdCallback, "Booster");
		}
		else
		{
			OnAdFinished(adWasSuccessful: false);
		}
	}

	private void RewardedAdCallback(RewardedAdResult obj)
	{
		OnAdFinished(obj == RewardedAdResult.RewardUnlocked);
	}

	private void OnAdFinished(bool adWasSuccessful)
	{
		Debug.Log("Ad finished");
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
		{
			x.Pop();
		});
		if (adWasSuccessful)
		{
			MVGameControllerBase.Game.LocalPlayer.BoostController.ActivateBoost(adRewardType);
		}
		else
		{
			ExecuteEvents.ExecuteHierarchy(gameObject, null, (IModalPopupCreator x, BaseEventData y) =>
			{
				x.Create(TM._("An error occurred. Try again later."), TM._("Error"));
			});
		}
		if (boostUnlockedCallback != null)
		{
			boostUnlockedCallback(adWasSuccessful);
		}
	}
}
