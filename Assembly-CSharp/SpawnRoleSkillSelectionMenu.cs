using System.Collections.Generic;
using MV.Common;
using MV.WorldObject.KogamaSettings.KogamaSettingsCore.KogamaSettingTypes;
using MV.WorldObject.KogamaSettings.SpecializedSettingsTypes.AttributeSettings.AttributeSettingTypes;
using UnityEngine;
using UnityEngine.Events;

public class SpawnRoleSkillSelectionMenu : MonoBehaviour
{
	[SerializeField]
	private Transform skillSelectionElementContainer;

	[SerializeField]
	private SpawnRoleSkillSelectionElement skillSelectionElementPrefab;

	private UnityAction<KogamaSettingValueWrapperBase> addSkillCallback;

	private UnityAction cantAddSkillCallback;

	private SpawnRolesSkillDataManager skillDataManager;

	private int spawnRoleCost;

	private GamePassTier spawnRoleTier;

	public void Initialize(SpawnRolesSkillDataManager skillDataManager, KogamaSettingWrapperBase notAppliedSettings, SkillCategory skillCategory, int spawnRoleCost, GamePassTier spawnRoleTier, UnityAction<KogamaSettingValueWrapperBase> addSkillCallback, UnityAction cantAddSkillCallback)
	{
		if (notAppliedSettings == null)
		{
			return;
		}
		this.spawnRoleCost = spawnRoleCost;
		this.spawnRoleTier = spawnRoleTier;
		this.skillDataManager = skillDataManager;
		this.addSkillCallback = addSkillCallback;
		this.cantAddSkillCallback = cantAddSkillCallback;
		KogamaSettingsCollectionBase kogamaSettingsCollectionBase = (KogamaSettingsCollectionBase)notAppliedSettings;
		foreach (KeyValuePair<string, KogamaSettingWrapperBase> child in kogamaSettingsCollectionBase.Children)
		{
			if (skillDataManager.GetSkillsCategory(child.Key) == skillCategory)
			{
				CreateSkillSelectionElement(child.Key, child.Value);
			}
		}
	}

	private void CreateSkillSelectionElement(string skillKey, KogamaSettingWrapperBase skillSetting)
	{
		SpawnRoleSkillSelectionElement spawnRoleSkillSelectionElement = Object.Instantiate(skillSelectionElementPrefab);
		spawnRoleSkillSelectionElement.Initialize(skillKey, skillDataManager, ((IAttributeSetting)skillSetting).AttributeValue, spawnRoleCost, spawnRoleTier, (KogamaSettingValueWrapperBase)skillSetting, addSkillCallback, cantAddSkillCallback);
		spawnRoleSkillSelectionElement.transform.SetParent(skillSelectionElementContainer, worldPositionStays: false);
	}
}
