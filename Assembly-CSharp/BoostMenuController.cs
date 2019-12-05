using System;
using System.Collections;
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

	[SerializeField]
	private RectTransform reboostContent;

	[SerializeField]
	private ReboostController reboostControllerPrefab;

	[SerializeField]
	private float reboostBackgroundAlpha = 0.8f;

	private BoostType adRewardType;

	private Action<bool> boostUnlockedCallback;

	public void Initialize()
	{
		BoostController boostController = MVGameControllerBase.Game.LocalPlayer.BoostController;
		Dictionary<BoostType, Boost>.ValueCollection allBoosts = boostController.GetAllBoosts();
		MVGameBoosterDataObject singletonWorldObject = MVGameControllerBase.WOCM.GetSingletonWorldObject<MVGameBoosterDataObject>();
		List<GameBoosterSettingWithGoldSetting> activeSettingsList = singletonWorldObject.GameBoosterSettingsManager.ActiveSettingsList;
		List<Boost> sortedBoosts = GetSortedBoosts(allBoosts, boostController);
		for (int i = 0; i < sortedBoosts.Count; i++)
		{
			for (int j = 0; j < activeSettingsList.Count; j++)
			{
				if (sortedBoosts[i].AllowedForGame && sortedBoosts[i].BoostKey == activeSettingsList[j].Key)
				{
					BoostMenuItem boostMenuItem = UnityEngine.Object.Instantiate(boostPrefab);
					boostMenuItem.transform.SetParent(boostItemsContent, worldPositionStays: false);
					boostMenuItem.Initialize(sortedBoosts[i], boostController.IsBoostActive(sortedBoosts[i].Type));
				}
			}
		}
		LayoutRebuilder.ForceRebuildLayoutImmediate(boostItemsContent);
		StartCoroutine(FixContentPivot());
		ReboostController reboostController = UnityEngine.Object.Instantiate(reboostControllerPrefab);
		reboostController.transform.SetParent(reboostContent, worldPositionStays: false);
		reboostController.ChangeBackgroundAlpha(reboostBackgroundAlpha);
	}

	public void TryShowAd(BoostType type, Action<bool> OnUnlockedCallback)
	{
		boostUnlockedCallback = OnUnlockedCallback;
		adRewardType = type;
		if (MVGameControllerBase.AdManager.ReadyForRewardedAdRequest)
		{
			MVGameControllerBase.AdManager.RequestRewardedAd(RewardedAdCallback, AdContext.Booster);
		}
		else
		{
			OnAdFinished(adWasSuccessful: false);
		}
	}

	public void TryShowAdForReboost(BoostType type, Action<bool> OnUnlockedCallback)
	{
		boostUnlockedCallback = OnUnlockedCallback;
		adRewardType = type;
		if (MVGameControllerBase.AdManager.ReadyForRewardedAdRequest)
		{
			MVGameControllerBase.AdManager.RequestRewardedAd(RewardedAdCallback, AdContext.Reboost);
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
		if (adWasSuccessful)
		{
			MVGameControllerBase.Game.LocalPlayer.BoostController.ActivateBoost(adRewardType);
			StatHatWrapper.Count("Ad.RewardRequest.Booster." + adRewardType, 1);
		}
		else
		{
			ExecuteEvents.ExecuteHierarchy(gameObject, null, (IModalPopupCreator x, BaseEventData y) =>
			{
				x.Create(MVGameControllerBase.AdManager.RewardedAdNotAvailableText, TM._("No Ad Available"));
			});
		}
		if (boostUnlockedCallback != null)
		{
			boostUnlockedCallback(adWasSuccessful);
		}
	}

	private IEnumerator FixContentPivot()
	{
		yield return null;
		if (boostItemsScrollRect.rect.width < boostItemsContent.rect.width)
		{
			boostItemsContent.pivot = new Vector2(0f, 0.5f);
		}
	}

	private List<Boost> GetSortedBoosts(Dictionary<BoostType, Boost>.ValueCollection boosts, BoostController boostController)
	{
		List<Boost> list = new List<Boost>();
		for (int i = 0; i < boostController.boostPriorityList.Count; i++)
		{
			foreach (Boost boost in boosts)
			{
				if (boost.Type == boostController.boostPriorityList[i])
				{
					list.Add(boost);
				}
			}
		}
		foreach (Boost boost2 in boosts)
		{
			bool flag = false;
			for (int j = 0; j < list.Count; j++)
			{
				if (list[j].Type == boost2.Type)
				{
					flag = true;
				}
			}
			if (!flag)
			{
				list.Add(boost2);
			}
		}
		return list;
	}
}
