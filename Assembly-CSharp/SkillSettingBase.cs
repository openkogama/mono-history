using MV.Common;
using MV.WorldObject.KogamaSettings.KogamaSettingsCore.KogamaSettingTypes;
using MV.WorldObject.KogamaSettings.SpecializedSettingsTypes.AttributeSettings.AttributeSettingTypes;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class SkillSettingBase : MonoBehaviour
{
	[SerializeField]
	private Transform imageContainer;

	[SerializeField]
	private Text nameText;

	[SerializeField]
	private Text skillCostText;

	[SerializeField]
	protected SpawnRoleSkillInfoButton infoButton;

	[SerializeField]
	private float iconHeight;

	[SerializeField]
	private float iconWidth;

	[SerializeField]
	private ColorStyle iconColor;

	[SerializeField]
	private ColorStyle iconBackgroundColor;

	private UnityAction<KogamaSettingValueWrapperBase> removeSkillCallback;

	protected KogamaSettingValueWrapperBase skillSetting;

	protected SpawnRolesSkillDataManager skillDataManager;

	protected SpawnRoleSkillIconController skillIcon;

	protected UnityAction<KogamaSettingValueWrapperBase> updateSkillCallback;

	protected UnityAction cantUpdateSkillCallback;

	protected UnityAction cantRemoveSkillCallback;

	protected int currentSkillCost;

	private int spawnRoleCost;

	private GamePassTier spawnRoleTier;

	public virtual void Initialize(string skill, SpawnRolesSkillDataManager skillDataManager, int skillCost, int spawnRoleCost, GamePassTier spawnRoleTier, KogamaSettingValueWrapperBase skillSetting, UnityAction<KogamaSettingValueWrapperBase> removeSkillCallback, UnityAction<KogamaSettingValueWrapperBase> updateSkillCallback, UnityAction cantUpdateSkillCallback, UnityAction cantRemoveSkillCallback)
	{
		this.skillDataManager = skillDataManager;
		currentSkillCost = skillCost;
		this.spawnRoleCost = spawnRoleCost;
		this.spawnRoleTier = spawnRoleTier;
		this.skillSetting = skillSetting;
		this.removeSkillCallback = removeSkillCallback;
		this.updateSkillCallback = updateSkillCallback;
		this.cantUpdateSkillCallback = cantUpdateSkillCallback;
		this.cantRemoveSkillCallback = cantRemoveSkillCallback;
		nameText.text = skillDataManager.GetNameText(skill);
		skillCostText.text = skillCost.ToString();
		skillCostText.color = SpawnRolesSkillDataManager.GetCostColor(skillCost);
		skillIcon = Object.Instantiate(skillDataManager.GetImageClone(skill, iconColor, iconBackgroundColor, iconWidth, iconHeight));
		skillIcon.transform.SetParent(imageContainer, worldPositionStays: false);
		skillIcon.HandleNegativeState(skillCost);
		InitializeInfoButton(skill, skillCost, skillDataManager);
	}

	public void RemoveSkill()
	{
		if (CanRemoveSkill())
		{
			removeSkillCallback(skillSetting);
			Object.Destroy(gameObject);
		}
		else
		{
			cantRemoveSkillCallback();
		}
	}

	public void UpdateSpawnRoleCost(int newSpawnRoleCost)
	{
		spawnRoleCost = newSpawnRoleCost;
	}

	public void UpdateSpawnRoleTier(GamePassTier newTier)
	{
		spawnRoleTier = newTier;
	}

	protected virtual void UpdateSkillData(object newValue)
	{
		infoButton.UpdateSkillValue(newValue);
	}

	protected void UpdateSkillCost()
	{
		currentSkillCost = ((IAttributeSetting)skillSetting).AttributeValue;
		skillCostText.text = currentSkillCost.ToString();
		skillIcon.HandleNegativeState(currentSkillCost);
		infoButton.UpdateSkillCost(currentSkillCost);
		skillCostText.color = SpawnRolesSkillDataManager.GetCostColor(currentSkillCost);
	}

	protected bool CanUpdateSkill(int newSkillCost)
	{
		if (spawnRoleTier != GamePassTier.Tier0)
		{
			return true;
		}
		int num = 100;
		int num2 = newSkillCost - currentSkillCost;
		int num3 = spawnRoleCost + num2;
		if (num3 <= num)
		{
			return true;
		}
		return false;
	}

	protected bool CanRemoveSkill()
	{
		if (spawnRoleTier != GamePassTier.Tier0)
		{
			return true;
		}
		int num = 100;
		int num2 = spawnRoleCost - currentSkillCost;
		if (num2 <= num)
		{
			return true;
		}
		return false;
	}

	protected virtual void InitializeInfoButton(string skill, int skillCost, SpawnRolesSkillDataManager skillDataManager)
	{
		infoButton.Initialize(skill, 0, skillCost, skillDataManager);
	}
}
