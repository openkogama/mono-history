using MV.WorldObject.KogamaSettings.KogamaSettingsCore.KogamaSettingTypes;
using MV.WorldObject.KogamaSettings.SpecializedSettingsTypes.AttributeSettings.AttributeSettingTypes;
using UnityEngine;
using UnityEngine.UI;

public class SpawnRoleSelectionSkillElement : MonoBehaviour
{
	[SerializeField]
	private Transform skillIconContainer;

	[SerializeField]
	private Image powerFillImage;

	[SerializeField]
	private Image negativePowerFillImage;

	[SerializeField]
	private Text nameText;

	[SerializeField]
	private Text descriptionText;

	[SerializeField]
	private float iconHeight;

	[SerializeField]
	private float iconWidth;

	[SerializeField]
	private ColorStyle iconColor;

	[SerializeField]
	private ColorStyle iconBackgroundColor;

	public void Initialize(string skill, SpawnRolesSkillDataManager skillDataManager, KogamaSettingValueWrapperBase skillSetting)
	{
		nameText.text = skillDataManager.GetNameText(skill);
		int attributeValue = ((IAttributeSetting)skillSetting).AttributeValue;
		SpawnRoleSkillIconController spawnRoleSkillIconController = Object.Instantiate(skillDataManager.GetImageClone(skill, iconColor, iconBackgroundColor, iconWidth, iconHeight));
		spawnRoleSkillIconController.transform.SetParent(skillIconContainer, worldPositionStays: false);
		spawnRoleSkillIconController.HandleNegativeState(attributeValue);
		object obj = 0;
		float fillAmount = 1f;
		float zeroValue = skillDataManager.GetZeroValue(skill);
		if (skillSetting is AttributeSettingInt)
		{
			obj = ((AttributeSettingInt)skillSetting).NumericValue;
			float max = ((AttributeSettingInt)skillSetting).KogamaSettingNumeric.RangeValidator.max;
			float min = ((AttributeSettingInt)skillSetting).KogamaSettingNumeric.RangeValidator.min;
			fillAmount = CalculateSkillPowerPercentage((float)(int)obj - zeroValue, max, min);
		}
		else if (skillSetting is AttributeSettingFloat)
		{
			obj = ((AttributeSettingFloat)skillSetting).NumericValue;
			float max2 = ((AttributeSettingFloat)skillSetting).KogamaSettingNumeric.RangeValidator.max;
			float min2 = ((AttributeSettingFloat)skillSetting).KogamaSettingNumeric.RangeValidator.min;
			fillAmount = CalculateSkillPowerPercentage((float)obj - zeroValue, max2, min2);
		}
		descriptionText.text = skillDataManager.GetSkillDescription(skill, obj, attributeValue);
		bool flag = attributeValue < 0;
		powerFillImage.fillAmount = fillAmount;
		negativePowerFillImage.fillAmount = fillAmount;
		powerFillImage.gameObject.SetActive(!flag);
		negativePowerFillImage.gameObject.SetActive(flag);
	}

	private float CalculateSkillPowerPercentage(float value, float max, float min)
	{
		float num = ((!(max > 0f)) ? ((value - max) / (min - max)) : ((value - min) / (max - min)));
		if (num < 0f)
		{
			num *= -1f;
		}
		return num;
	}
}
