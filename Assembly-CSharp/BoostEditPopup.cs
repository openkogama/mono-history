using System;
using System.Collections.Generic;
using MV.WorldObject.KogamaSettings.KogamaSettingsCore.KogamaSettingTypes;
using MV.WorldObject.KogamaSettings.SpecializedSettingsTypes.GameBoosterSettings.GameBoosterSettingTypes;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class BoostEditPopup : MonoBehaviour
{
	[Serializable]
	private struct BoosterDef
	{
		public BoostType type;

		public GameObject iconPrefab;
	}

	[SerializeField]
	private RectTransform boostImageParent;

	[SerializeField]
	private Text headerText;

	[SerializeField]
	private Slider priceSlider;

	[SerializeField]
	private InputField priceInputField;

	[SerializeField]
	private List<BoosterDef> boosterList;

	protected bool isInitialized;

	protected GameBoosterSettingWithGoldSetting boostSetting;

	protected UnityAction<object> settingChangedCallback;

	protected UnityAction<int> priceChangedCallback;

	protected UnityAction submitDataCallback;

	public virtual void Initialize(Boost boost, GameBoosterSettingWithGoldSetting boostSetting, UnityAction<object> settingChangedCallback, UnityAction<int> priceChangedCallback, UnityAction submitDataCallback)
	{
		this.boostSetting = boostSetting;
		this.settingChangedCallback = settingChangedCallback;
		this.priceChangedCallback = priceChangedCallback;
		this.submitDataCallback = submitDataCallback;
		CreateBoostImage(boost);
		headerText.text = boost.EditTitle;
		KogamaSettingNumericBase<int> goldPrice = boostSetting.GoldPrice;
		priceSlider.maxValue = goldPrice.KogamaSettingNumeric.RangeValidator.max;
		priceSlider.minValue = goldPrice.KogamaSettingNumeric.RangeValidator.min;
		priceSlider.value = goldPrice.NumericValue;
		isInitialized = true;
	}

	private void CreateBoostImage(Boost boost)
	{
		for (int i = 0; i < boosterList.Count; i++)
		{
			if (boosterList[i].type == boost.Type)
			{
				GameObject gameObject = UnityEngine.Object.Instantiate(boosterList[i].iconPrefab);
				gameObject.transform.SetParent(boostImageParent, worldPositionStays: false);
				break;
			}
		}
	}

	private void OnDestroy()
	{
		submitDataCallback();
	}

	private void UpdatePriceTextInputFieldWithPriceSliderValue()
	{
		priceInputField.text = Mathf.FloorToInt(priceSlider.value).ToString();
	}

	private void UpdatePriceSliderValueWithPriceInputFieldText()
	{
		float.TryParse(priceInputField.text, out var result);
		priceSlider.value = result;
	}

	private void UpdatePriceData()
	{
		if (isInitialized)
		{
			priceChangedCallback((int)priceSlider.value);
		}
	}

	public void PriceSliderValueChanged()
	{
		UpdatePriceTextInputFieldWithPriceSliderValue();
		UpdatePriceData();
	}

	public void PriceInputFieldChange()
	{
		UpdatePriceSliderValueWithPriceInputFieldText();
		UpdatePriceTextInputFieldWithPriceSliderValue();
		UpdatePriceData();
	}
}
