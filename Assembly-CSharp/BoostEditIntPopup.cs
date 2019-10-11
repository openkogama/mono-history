using MV.WorldObject.KogamaSettings.KogamaSettingsCore.KogamaSettingTypes;
using MV.WorldObject.KogamaSettings.SpecializedSettingsTypes.GameBoosterSettings.GameBoosterSettingTypes;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class BoostEditIntPopup : BoostEditPopup
{
	[SerializeField]
	private Text boostSliderDescription;

	[SerializeField]
	private Slider boostSlider;

	[SerializeField]
	private InputField boostInputField;

	private int originalSettingValue;

	public override void Initialize(Boost boost, GameBoosterSettingWithGoldSetting boostSetting, UnityAction<object> settingChangedCallback, UnityAction<int> priceChangedCallback, UnityAction submitDataCallback)
	{
		boostSliderDescription.text = boost.ValueDescription;
		KogamaSettingNumericBase<int> kogamaSettingNumericBase = (KogamaSettingNumericBase<int>)boostSetting.Setting;
		boostSlider.maxValue = kogamaSettingNumericBase.KogamaSettingNumeric.RangeValidator.max;
		boostSlider.minValue = kogamaSettingNumericBase.KogamaSettingNumeric.RangeValidator.min;
		boostSlider.value = kogamaSettingNumericBase.NumericValue;
		originalSettingValue = kogamaSettingNumericBase.NumericValue;
		base.Initialize(boost, boostSetting, settingChangedCallback, priceChangedCallback, submitDataCallback);
	}

	public override void OnCancelBoostEdit()
	{
		boostSlider.value = originalSettingValue;
		UpdateSettingData();
		base.OnCancelBoostEdit();
	}

	private void UpdateBoostTextInputFieldWithBoostSliderValue()
	{
		boostInputField.text = Mathf.FloorToInt(boostSlider.value).ToString();
	}

	private void UpdateBoostSliderValueWithBoostInputFieldText()
	{
		float.TryParse(boostInputField.text, out var result);
		boostSlider.value = result;
	}

	private void UpdateSettingData()
	{
		if (isInitialized)
		{
			settingChangedCallback((int)boostSlider.value);
		}
	}

	public void BoostSliderValueChanged()
	{
		UpdateBoostTextInputFieldWithBoostSliderValue();
		UpdateSettingData();
	}

	public void BoostInputFieldChange()
	{
		UpdateBoostSliderValueWithBoostInputFieldText();
		UpdateBoostTextInputFieldWithBoostSliderValue();
		UpdateSettingData();
	}
}
