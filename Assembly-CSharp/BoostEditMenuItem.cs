using System;
using System.Collections.Generic;
using MV.WorldObject.GamePassSystem.GamePassEarnings;
using MV.WorldObject.KogamaSettings.KogamaSettingsCore.KogamaSettingTypes;
using MV.WorldObject.KogamaSettings.SpecializedSettingsTypes.GameBoosterSettings.GameBoosterSettingTypes;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BoostEditMenuItem : MonoBehaviour
{
	[Serializable]
	private struct BoosterDef
	{
		public BoostType type;

		public GameObject iconPrefab;
	}

	[SerializeField]
	private RectTransform boostTypeImageParent;

	[SerializeField]
	private Text boostDescription;

	[SerializeField]
	private Text goldPriceText;

	[SerializeField]
	private Text earningsAmountText;

	[SerializeField]
	private ToggleButtonAnimation activeToggleButton;

	[SerializeField]
	private BoostEditPopup boostEditPopupPrefab;

	[SerializeField]
	private BoostEditIntPopup boostEditIntPopupPrefab;

	[SerializeField]
	private BoostEditFloatPopup boostEditFloatPopupPrefab;

	[SerializeField]
	private List<BoosterDef> boosterList;

	private Boost boost;

	private GameBoosterSettingWithGoldSetting boosterSetting;

	private bool isActive;

	public void Initialize(Boost boost)
	{
		this.boost = boost;
		boostDescription.text = boost.Description;
		activeToggleButton.Initialize();
		MVGameBoosterDataObject singletonWorldObject = MVGameControllerBase.WOCM.GetSingletonWorldObject<MVGameBoosterDataObject>();
		List<GameBoosterSettingWithGoldSetting> activeSettingsList = singletonWorldObject.GameBoosterSettingsManager.ActiveSettingsList;
		List<GameBoosterSettingWithGoldSetting> inactiveGameBoosterSettingsList = singletonWorldObject.GameBoosterSettingsManager.InactiveGameBoosterSettingsList;
		for (int i = 0; i < activeSettingsList.Count; i++)
		{
			if (activeSettingsList[i].Key == boost.BoostKey)
			{
				isActive = true;
				activeToggleButton.SetToggleOnWithoutInterpolation();
				boosterSetting = activeSettingsList[i];
			}
		}
		for (int j = 0; j < inactiveGameBoosterSettingsList.Count; j++)
		{
			if (inactiveGameBoosterSettingsList[j].Key == boost.BoostKey)
			{
				isActive = false;
				activeToggleButton.SetToggleOffWithoutInterpolation();
				boosterSetting = inactiveGameBoosterSettingsList[j];
			}
		}
		for (int k = 0; k < boosterList.Count; k++)
		{
			if (boosterList[k].type == boost.Type)
			{
				GameObject gameObject = UnityEngine.Object.Instantiate(boosterList[k].iconPrefab);
				gameObject.transform.SetParent(boostTypeImageParent, worldPositionStays: false);
				break;
			}
		}
		int numericValue = boosterSetting.GoldPrice.NumericValue;
		goldPriceText.text = numericValue.ToString("N0").Replace(",", ".");
	}

	public void UpdateEarningsText(ProjectEarningsReport projectEarningsReport)
	{
		int profileID = MVGameControllerBase.Game.LocalPlayer.ProfileID;
		int boostEarning = GetBoostEarning(projectEarningsReport);
		earningsAmountText.text = boostEarning.ToString("N0").Replace(",", ".");
	}

	public void ShowEditPopup()
	{
		if (boosterSetting.Setting is KogamaSettingNumericBase<int>)
		{
			BoostEditIntPopup boostEditIntPopup = UnityEngine.Object.Instantiate(boostEditIntPopupPrefab);
			boostEditIntPopup.Initialize(boost, boosterSetting, OnBoostSettingChange, OnPriceSettingChanged, OnSubmitData);
			ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
			{
				x.Push(boostEditIntPopup.gameObject, UIPushOption.InvisibleBlocker, null, UIGroupFlags.InventoryUI);
			});
		}
		else if (boosterSetting.Setting is KogamaSettingNumericBase<float>)
		{
			BoostEditFloatPopup boostEditFloatPopup = UnityEngine.Object.Instantiate(boostEditFloatPopupPrefab);
			boostEditFloatPopup.Initialize(boost, boosterSetting, OnBoostSettingChange, OnPriceSettingChanged, OnSubmitData);
			ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
			{
				x.Push(boostEditFloatPopup.gameObject, UIPushOption.InvisibleBlocker, null, UIGroupFlags.InventoryUI);
			});
		}
		else
		{
			BoostEditPopup boostEditPopup = UnityEngine.Object.Instantiate(boostEditPopupPrefab);
			boostEditPopup.Initialize(boost, boosterSetting, OnBoostSettingChange, OnPriceSettingChanged, OnSubmitData);
			ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
			{
				x.Push(boostEditPopup.gameObject, UIPushOption.InvisibleBlocker, null, UIGroupFlags.InventoryUI);
			});
		}
	}

	public void OnActiveToggle()
	{
		MVGameBoosterDataObject singletonWorldObject = MVGameControllerBase.WOCM.GetSingletonWorldObject<MVGameBoosterDataObject>();
		if (isActive)
		{
			singletonWorldObject.GameBoosterSettingsManager.RemoveSetting(boosterSetting);
			singletonWorldObject.GameBoosterSettingsManager.Submit();
		}
		else
		{
			singletonWorldObject.GameBoosterSettingsManager.UpdateSetting(boosterSetting);
			singletonWorldObject.GameBoosterSettingsManager.Submit();
		}
		isActive = !isActive;
	}

	private int GetBoostEarning(ProjectEarningsReport projectEarningsReport)
	{
		int profileID = MVGameControllerBase.Game.LocalPlayer.ProfileID;
		if (!projectEarningsReport.projectMemberEarningsReports.ContainsKey(profileID))
		{
			return 0;
		}
		if (!projectEarningsReport.projectMemberEarningsReports[profileID].earningsReport.gameBoosterEarningsGold.ContainsKey(boost.BoostKey))
		{
			return 0;
		}
		return projectEarningsReport.projectMemberEarningsReports[profileID].earningsReport.gameBoosterEarningsGold[boost.BoostKey];
	}

	private void OnBoostSettingChange(object newValue)
	{
		MVGameBoosterDataObject singletonWorldObject = MVGameControllerBase.WOCM.GetSingletonWorldObject<MVGameBoosterDataObject>();
		if (boosterSetting.Setting is KogamaSettingNumericBase<int>)
		{
			((KogamaSettingNumericBase<int>)boosterSetting.Setting).NumericValue = (int)newValue;
			singletonWorldObject.GameBoosterSettingsManager.UpdateSetting(boosterSetting);
		}
		else if (boosterSetting.Setting is KogamaSettingNumericBase<float>)
		{
			((KogamaSettingNumericBase<float>)boosterSetting.Setting).NumericValue = (float)newValue;
			singletonWorldObject.GameBoosterSettingsManager.UpdateSetting(boosterSetting);
		}
		boostDescription.text = boost.Description;
	}

	private void OnPriceSettingChanged(int newPrice)
	{
		MVGameBoosterDataObject singletonWorldObject = MVGameControllerBase.WOCM.GetSingletonWorldObject<MVGameBoosterDataObject>();
		boosterSetting.GoldPrice.NumericValue = newPrice;
		singletonWorldObject.GameBoosterSettingsManager.UpdateSetting(boosterSetting);
		goldPriceText.text = newPrice.ToString("N0").Replace(",", ".");
	}

	private void OnSubmitData()
	{
		MVGameBoosterDataObject singletonWorldObject = MVGameControllerBase.WOCM.GetSingletonWorldObject<MVGameBoosterDataObject>();
		singletonWorldObject.GameBoosterSettingsManager.Submit();
	}
}
