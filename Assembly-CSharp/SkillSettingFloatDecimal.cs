using MV.Common;
using MV.WorldObject.KogamaSettings.KogamaSettingsCore.KogamaSettingTypes;
using MV.WorldObject.KogamaSettings.SpecializedSettingsTypes.AttributeSettings.AttributeSettingTypes;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class SkillSettingFloatDecimal : SkillSettingBase
{
	[SerializeField]
	private Slider slider;

	[SerializeField]
	private InputField inputField;

	[SerializeField]
	private int decimalPlaces = 2;

	private bool isInitialized;

	private float settingValue;

	private AttributeSettingFloat skillPercentageSetting;

	public override void Initialize(string skill, SpawnRolesSkillDataManager skillDataManager, int skillCost, int spawnRoleCost, GamePassTier spawnRoleTier, KogamaSettingValueWrapperBase skillSetting, UnityAction<KogamaSettingValueWrapperBase> removeSkillCallback, UnityAction<KogamaSettingValueWrapperBase> updateSkillCallback, UnityAction cantUpdateSkillCallback, UnityAction cantRemoveSkillCallback)
	{
		skillPercentageSetting = (AttributeSettingFloat)skillSetting;
		settingValue = skillPercentageSetting.NumericValue;
		base.Initialize(skill, skillDataManager, skillCost, spawnRoleCost, spawnRoleTier, skillSetting, removeSkillCallback, updateSkillCallback, cantUpdateSkillCallback, cantRemoveSkillCallback);
		slider.minValue = skillPercentageSetting.KogamaSettingNumeric.RangeValidator.min;
		slider.maxValue = skillPercentageSetting.KogamaSettingNumeric.RangeValidator.max;
		slider.value = settingValue;
		UpdateTextInputFieldWithSliderValue();
		isInitialized = true;
	}

	protected override void InitializeInfoButton(string skill, int skillCost, SpawnRolesSkillDataManager skillDataManager)
	{
		infoButton.Initialize(skill, settingValue, skillCost, skillDataManager);
	}

	protected override void UpdateSkillData(object newValue)
	{
		if (isInitialized)
		{
			base.UpdateSkillData(newValue);
			float numericValue = skillPercentageSetting.NumericValue;
			float num = Mathf.Pow(decimalPlaces, 10f);
			float numericValue2 = Mathf.Floor((float)newValue * num) / num;
			skillPercentageSetting.NumericValue = numericValue2;
			int attributeValue = ((IAttributeSetting)skillSetting).AttributeValue;
			if (CanUpdateSkill(attributeValue))
			{
				updateSkillCallback(skillPercentageSetting);
				UpdateSkillCost();
				return;
			}
			skillPercentageSetting.NumericValue = numericValue;
			slider.value = numericValue;
			UpdateTextInputFieldWithSliderValue();
			cantUpdateSkillCallback();
		}
	}

	private void UpdateTextInputFieldWithSliderValue()
	{
		inputField.text = slider.value.ToString();
	}

	private void UpdateSliderValueWithInputFieldText()
	{
		float.TryParse(inputField.text, out var result);
		slider.value = result;
	}

	public void SliderValueChanged()
	{
		float num = Mathf.Pow(decimalPlaces, 10f);
		slider.value = Mathf.Floor(slider.value * num) / num;
		UpdateTextInputFieldWithSliderValue();
		UpdateSkillData(slider.value);
	}

	public void InputFieldChange()
	{
		float num = Mathf.Pow(decimalPlaces, 10f);
		slider.value = Mathf.Floor(slider.value * num) / num;
		UpdateSliderValueWithInputFieldText();
		UpdateTextInputFieldWithSliderValue();
		UpdateSkillData(slider.value);
	}
}
