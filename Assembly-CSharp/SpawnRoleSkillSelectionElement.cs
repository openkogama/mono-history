using MV.Common;
using MV.WorldObject.KogamaSettings.KogamaSettingsCore.KogamaSettingTypes;
using MV.WorldObject.KogamaSettings.SpecializedSettingsTypes.AttributeSettings.AttributeSettingTypes;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.UI;

public class SpawnRoleSkillSelectionElement : MonoBehaviour
{
	[SerializeField]
	private Transform imageContainer;

	[SerializeField]
	private Text nameText;

	[SerializeField]
	private Text spawnRoleCostText;

	[SerializeField]
	private SpawnRoleSkillInfoButton infoButton;

	[SerializeField]
	private GameObject cogWheelIcon;

	[SerializeField]
	private float iconHeight;

	[SerializeField]
	private float iconWidth;

	[SerializeField]
	private ColorStyle iconColor;

	[SerializeField]
	private ColorStyle iconBackgroundColor;

	private int skillCost;

	private int spawnRoleCost;

	private GamePassTier spawnRoleTier;

	private KogamaSettingValueWrapperBase skillSetting;

	private UnityAction<KogamaSettingValueWrapperBase> addSkillCallback;

	private UnityAction cantAddSkillCallback;

	public void Initialize(string skill, SpawnRolesSkillDataManager skillDataManager, int skillCost, int spawnRoleCost, GamePassTier spawnRoleTier, KogamaSettingValueWrapperBase skillSetting, UnityAction<KogamaSettingValueWrapperBase> addSkillCallback, UnityAction cantAddSkillCallback)
	{
		this.skillCost = skillCost;
		this.spawnRoleCost = spawnRoleCost;
		this.spawnRoleTier = spawnRoleTier;
		this.skillSetting = skillSetting;
		this.addSkillCallback = addSkillCallback;
		this.cantAddSkillCallback = cantAddSkillCallback;
		nameText.text = skillDataManager.GetNameText(skill);
		spawnRoleCostText.text = skillCost.ToString();
		spawnRoleCostText.color = SpawnRolesSkillDataManager.GetCostColor(skillCost);
		SpawnRoleSkillIconController spawnRoleSkillIconController = Object.Instantiate(skillDataManager.GetImageClone(skill, iconColor, iconBackgroundColor, iconWidth, iconHeight));
		spawnRoleSkillIconController.transform.SetParent(imageContainer, worldPositionStays: false);
		spawnRoleSkillIconController.HandleNegativeState(skillCost);
		cogWheelIcon.gameObject.SetActive(!(skillSetting is KogamaSettingBoolBase));
		InitializeInfoButton(skill, skillDataManager);
	}

	public void AddSkill()
	{
		if (CanAddSkill())
		{
			addSkillCallback(skillSetting);
		}
		else
		{
			cantAddSkillCallback();
		}
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
		{
			x.Pop();
		});
	}

	private void InitializeInfoButton(string skill, SpawnRolesSkillDataManager skillDataManager)
	{
		object skillValue = 0;
		if (skillSetting is AttributeSettingInt)
		{
			skillValue = ((AttributeSettingInt)skillSetting).NumericValue;
		}
		else if (skillSetting is AttributeSettingFloat)
		{
			skillValue = ((AttributeSettingFloat)skillSetting).NumericValue;
		}
		infoButton.Initialize(skill, skillValue, skillCost, skillDataManager);
	}

	private bool CanAddSkill()
	{
		if (spawnRoleTier != GamePassTier.Tier0)
		{
			return true;
		}
		int num = 100;
		int num2 = spawnRoleCost + skillCost;
		if (num2 <= num)
		{
			return true;
		}
		return false;
	}
}
