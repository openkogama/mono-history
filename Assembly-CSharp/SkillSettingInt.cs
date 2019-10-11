using MV.Common;
using MV.WorldObject.KogamaSettings.KogamaSettingsCore.KogamaSettingTypes;
using MV.WorldObject.KogamaSettings.SpecializedSettingsTypes.AttributeSettings.AttributeSettingTypes;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class SkillSettingInt : SkillSettingBase
{
	[SerializeField]
	private Slider slider;

	[SerializeField]
	private InputField inputField;

	private bool isInitialized;

	private float settingValue;

	private AttributeSettingInt skillPercentageSetting;

	public override void Initialize(string skill, SpawnRolesSkillDataManager skillDataManager, int skillCost, int spawnRoleCost, GamePassTier spawnRoleTier, KogamaSettingValueWrapperBase skillSetting, UnityAction<KogamaSettingValueWrapperBase> removeSkillCallback, UnityAction<KogamaSettingValueWrapperBase> updateSkillCallback, UnityAction cantUpdateSkillCallback, UnityAction cantRemoveSkillCallback)
	{
		skillPercentageSetting = (AttributeSettingInt)skillSetting;
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
			int numericValue = skillPercentageSetting.NumericValue;
			skillPercentageSetting.NumericValue = Mathf.FloorToInt((float)newValue);
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
		inputField.text = Mathf.FloorToInt(slider.value).ToString();
	}

	private void UpdateSliderValueWithInputFieldText()
	{
		float.TryParse(inputField.text, out var result);
		slider.value = result;
	}

	public void SliderValueChanged()
	{
		UpdateTextInputFieldWithSliderValue();
		UpdateSkillData(slider.value);
	}

	public void InputFieldChange()
	{
		UpdateSliderValueWithInputFieldText();
		UpdateTextInputFieldWithSliderValue();
		UpdateSkillData(slider.value);
	}
}
