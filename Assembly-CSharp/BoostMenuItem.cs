using System;
using System.Collections.Generic;
using MV.Common;
using MV.WorldObject.KogamaSettings.SpecializedSettingsTypes.GameBoosterSettings.GameBoosterSettingTypes;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BoostMenuItem : MonoBehaviour
{
	[Serializable]
	private struct BoosterDef
	{
		public BoostType type;

		public GameObject iconPrefab;
	}

	[SerializeField]
	private GameObject boostUnlockedGlow;

	[SerializeField]
	private GameObject boostActiveUI;

	[SerializeField]
	private RectTransform boostActiveIcon;

	[SerializeField]
	private NotificationFade boostActiveIconFader;

	[SerializeField]
	private CanvasGroup boostActiveIconCanvasGroup;

	[SerializeField]
	private RectTransform boostTypeImageParent;

	[SerializeField]
	private Button getWithAd;

	[SerializeField]
	private GameObject buttonAdImage;

	[SerializeField]
	private Button getWithAdDisabled;

	[SerializeField]
	private Button getWithGold;

	[SerializeField]
	private Text boostDescription;

	[SerializeField]
	private Text timeLeftText;

	[SerializeField]
	private Text priceText;

	[SerializeField]
	private BoostPurchasePopup purchasePopupPrefab;

	[SerializeField]
	private GameObject boostTouristInformation;

	[SerializeField]
	private List<BoosterDef> boosterList;

	[SerializeField]
	private BoostImageController boostImageController;

	[SerializeField]
	private AnimationCurve activeIconScaleEffect;

	[SerializeField]
	private float activeIconScaleEffectDuration;

	[SerializeField]
	private EmbeddedPlayerConfig embeddedPlayerConfig;

	private float activeIconScaleEffectStartTime;

	private Boost boost;

	private int price;

	public void Initialize(Boost boost, bool boostUnlocked)
	{
		this.boost = boost;
		boostDescription.text = boost.Description;
		Image image = UnityEngine.Object.Instantiate(boostImageController.GetBoostVisualization(boost.Type));
		image.transform.SetParent(boostActiveIcon.transform, worldPositionStays: false);
		SetBoostUIUnlocked(boostUnlocked);
		MVGameControllerBase.LocalPlayer.BoostController.SubscribeToBoostChanged(boost.Type, BoostChanged);
		for (int i = 0; i < boosterList.Count; i++)
		{
			if (boosterList[i].type == boost.Type)
			{
				GameObject gameObject = UnityEngine.Object.Instantiate(boosterList[i].iconPrefab);
				gameObject.transform.SetParent(boostTypeImageParent, worldPositionStays: false);
				break;
			}
		}
		if (boostUnlocked)
		{
			boostActiveIconCanvasGroup.alpha = 1f;
		}
		priceText.text = GetBoostPrice().ToString("N0").Replace(",", ".");
	}

	private void Update()
	{
		float num = activeIconScaleEffect.Evaluate((Time.time - activeIconScaleEffectStartTime) / activeIconScaleEffectDuration);
		boostActiveIcon.localScale = new Vector3(num, num);
	}

	private void SetBoostUIUnlocked(bool boostUnlocked)
	{
		bool flag = false;
		flag = MVClientSettings.BoostersEnabled || MVGameControllerBase.GameMode == MVGameMode.Edit;
		timeLeftText.gameObject.SetActive(boostUnlocked);
		boostUnlockedGlow.SetActive(boostUnlocked);
		boostActiveUI.SetActive(boostUnlocked);
		getWithAd.gameObject.SetActive(!boostUnlocked && flag);
		getWithAdDisabled.gameObject.SetActive(!boostUnlocked && !flag);
		buttonAdImage.SetActive(MVGameControllerBase.GameMode != MVGameMode.Edit);
		EmbeddedSiteConfigData currentSiteData = embeddedPlayerConfig.GetCurrentSiteData();
		bool flag2 = currentSiteData.allowsModals || currentSiteData.allowsOpenInNewTab || currentSiteData.allowsRedirectToWebpage;
		getWithGold.gameObject.SetActive(!boostUnlocked && MVGameControllerBase.GameMode != MVGameMode.Edit && flag2);
	}

	private void ActivateActiveBoostIconEffect()
	{
		boostActiveIconFader.ShouldHideWhenDone = false;
		boostActiveIconFader.Activate();
		activeIconScaleEffectStartTime = Time.time;
	}

	public void OnUnlockBoostWithAdClicked()
	{
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IBoostAdController x, BaseEventData y) =>
		{
			x.TryShowAd(boost.Type, BoostUnlockedResponse);
		});
	}

	public void OnPurchaseBoostPressed()
	{
		if (MVGameControllerBase.IsTouristSession)
		{
			GameObject informationPopup = UnityEngine.Object.Instantiate(boostTouristInformation);
			ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
			{
				x.Push(informationPopup, UIPushOption.InvisibleBlocker, null, UIGroupFlags.InventoryUI);
			});
			return;
		}
		BoostPurchasePopup boostPurchasePopup = UnityEngine.Object.Instantiate(purchasePopupPrefab);
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
		{
			x.Push(boostPurchasePopup.gameObject, UIPushOption.InvisibleBlocker, null, UIGroupFlags.InventoryUI);
		});
		price = GetBoostPrice();
		boostPurchasePopup.Initialize(boost.Type, boost.BoostKey, boost.EditTitle, price, OnPurchaseSuccessful);
	}

	private int GetBoostPrice()
	{
		MVGameOptionDataObject singletonWorldObject = MVGameControllerBase.WOCM.GetSingletonWorldObject<MVGameOptionDataObject>();
		List<GameBoosterSettingWithGoldSetting> activeSettingsList = singletonWorldObject.GameBoosterSettingsManager.ActiveSettingsList;
		for (int i = 0; i < activeSettingsList.Count; i++)
		{
			if (activeSettingsList[i].Key == boost.BoostKey)
			{
				GameBoosterSettingWithGoldSetting gameBoosterSettingWithGoldSetting = activeSettingsList[i];
				return gameBoosterSettingWithGoldSetting.GoldPrice.NumericValue;
			}
		}
		return 0;
	}

	private void BoostChanged()
	{
		bool flag = MVGameControllerBase.LocalPlayer.BoostController.IsBoostActive(boost.Type);
		SetBoostUIUnlocked(flag);
		if (flag)
		{
			ActivateActiveBoostIconEffect();
		}
	}

	private void BoostUnlockedResponse(bool boostUnlocked)
	{
		SetBoostUIUnlocked(boostUnlocked);
		if (boostUnlocked)
		{
			ActivateActiveBoostIconEffect();
		}
	}

	private void OnPurchaseSuccessful()
	{
		MVGameControllerBase.Game.LocalPlayer.BoostController.ActivateBoost(boost.Type);
		StatHatWrapper.Count("Purchase.Booster." + boost.Type, 1);
		StatHatWrapper.Count("Purchase.Booster.GoldSpent", price);
		bool flag = MVGameControllerBase.LocalPlayer.BoostController.IsBoostActive(boost.Type);
		SetBoostUIUnlocked(flag);
		if (flag)
		{
			ActivateActiveBoostIconEffect();
		}
	}

	private void OnDestroy()
	{
		if (MVGameControllerBase.IsAlive)
		{
			MVGameControllerBase.LocalPlayer.BoostController.UnSubscribeToBoostChanged(boost.Type, BoostChanged);
		}
	}
}
