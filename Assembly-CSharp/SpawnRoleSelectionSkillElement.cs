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
		SpawnRoleSkillIconController imageClone = skillDataManager.GetImageClone(skill, iconColor, iconBackgroundColor, iconWidth, iconHeight);
		imageClone.transform.SetParent(skillIconContainer, worldPositionStays: false);
		imageClone.HandleNegativeState(attributeValue);
		object obj = 0;
		float fillAmount = 1f;
		float zeroValue = skillDataManager.GetZeroValue(skill);
		if (skillSetting is AttributeSettingInt)
		{
			obj = ((AttributeSettingInt)skillSetting).NumericValue;
			float max = ((AttributeSettingInt)skillSetting).KogamaSettingNumeric.RangeValidator.max;
			float min = ((AttributeSettingInt)skillSetting).KogamaSettingNumeric.RangeValidator.min;
			fillAmount = CalculateSkillPowerPercentage((int)obj, max, min, zeroValue);
		}
		else if (skillSetting is AttributeSettingFloat)
		{
			obj = ((AttributeSettingFloat)skillSetting).NumericValue;
			float max2 = ((AttributeSettingFloat)skillSetting).KogamaSettingNumeric.RangeValidator.max;
			float min2 = ((AttributeSettingFloat)skillSetting).KogamaSettingNumeric.RangeValidator.min;
			fillAmount = CalculateSkillPowerPercentage((float)obj, max2, min2, zeroValue);
		}
		descriptionText.text = skillDataManager.GetSkillDescription(skill, obj, attributeValue);
		bool flag = attributeValue < 0;
		powerFillImage.fillAmount = fillAmount;
		negativePowerFillImage.fillAmount = fillAmount;
		powerFillImage.gameObject.SetActive(!flag);
		negativePowerFillImage.gameObject.SetActive(flag);
	}

	private float CalculateSkillPowerPercentage(float value, float max, float min, float zeroValue)
	{
		if (zeroValue < value)
		{
			float num = min;
			if (min < zeroValue)
			{
				num = zeroValue;
			}
			if (max > 0f)
			{
				return (value - num) / (max - num);
			}
			return (value - max) / (min - max);
		}
		return 1f - (value - min) / (zeroValue - min);
	}
}
