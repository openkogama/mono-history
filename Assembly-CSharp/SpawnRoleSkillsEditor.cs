using System.Collections.Generic;
using MV.Common;
using MV.WorldObject.KogamaSettings.KogamaSettingsCore.KogamaSettingTypes;
using MV.WorldObject.KogamaSettings.SpecializedSettingsTypes.AttributeSettings;
using MV.WorldObject.KogamaSettings.SpecializedSettingsTypes.AttributeSettings.AttributeSettingTypes;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class SpawnRoleSkillsEditor : MonoBehaviour
{
	[SerializeField]
	private Transform defenceSkillsContainer;

	[SerializeField]
	private Transform offenceSkillsContainer;

	[SerializeField]
	private Transform tacticalSkillsContainer;

	[SerializeField]
	private GamePassesTextBubble cantAddSkillInfoTextBubble;

	[SerializeField]
	private SpawnRolesSkillDataManager skillDataManagerPrefab;

	[SerializeField]
	private SpawnRoleSkillSelectionMenu skillSelectionMenuPrefab;

	private AttributeSettingsManager attributeSettingsManager;

	private int spawnRoleCost;

	private GamePassTier spawnRoleTier;

	private List<SkillSettingBase> skillSettingList = new List<SkillSettingBase>();

	private UnityAction updateSkillCostCallback;

	private void OnDestroy()
	{
		attributeSettingsManager.Submit();
	}

	public void Initialize(int spawnRoleCost, GamePassTier spawnRoleTier, AttributeSettingsManager spawnRoleAttributeSettingsManager, UnityAction updateSkillCost)
	{
		this.spawnRoleCost = spawnRoleCost;
		this.spawnRoleTier = spawnRoleTier;
		attributeSettingsManager = spawnRoleAttributeSettingsManager;
		updateSkillCostCallback = updateSkillCost;
		CreateSkillSettings();
	}

	public void UpdateSpawnRoleTier(GamePassTier newSpawnRoleTier)
	{
		spawnRoleTier = newSpawnRoleTier;
		for (int i = 0; i < skillSettingList.Count; i++)
		{
			skillSettingList[i].UpdateSpawnRoleTier(newSpawnRoleTier);
		}
	}

	public void UpdateSpawnRoleCost(int newSpawnRoleCost)
	{
		spawnRoleCost = newSpawnRoleCost;
		for (int i = 0; i < skillSettingList.Count; i++)
		{
			skillSettingList[i].UpdateSpawnRoleCost(newSpawnRoleCost);
		}
	}

	private void CreateSkillSettings()
	{
		KogamaSettingsCollectionBase kogamaSettingsCollectionBase = (KogamaSettingsCollectionBase)attributeSettingsManager.Settings;
		if (kogamaSettingsCollectionBase == null)
		{
			return;
		}
		foreach (KeyValuePair<string, KogamaSettingWrapperBase> child in kogamaSettingsCollectionBase.Children)
		{
			CreateSkillSetting(child.Key, child.Value);
		}
	}

	private void CreateSkillSetting(string skillKey, KogamaSettingWrapperBase skillSettingData)
	{
		SkillSettingBase skillsSettingsClone = skillDataManagerPrefab.GetSkillsSettingsClone(skillKey);
		skillsSettingsClone.Initialize(skillKey, skillDataManagerPrefab, ((IAttributeSetting)skillSettingData).AttributeValue, spawnRoleCost, spawnRoleTier, (KogamaSettingValueWrapperBase)skillSettingData, RemoveSkillCallback, UpdateSkillCallback, CantUpdateSkillCallback, CantRemoveSkillCallback);
		skillSettingList.Add(skillsSettingsClone);
		switch (skillDataManagerPrefab.GetSkillsCategory(skillKey))
		{
		case SkillCategory.Defence:
			skillsSettingsClone.transform.SetParent(defenceSkillsContainer, worldPositionStays: false);
			break;
		case SkillCategory.Offence:
			skillsSettingsClone.transform.SetParent(offenceSkillsContainer, worldPositionStays: false);
			break;
		case SkillCategory.Tactical:
			skillsSettingsClone.transform.SetParent(tacticalSkillsContainer, worldPositionStays: false);
			break;
		}
	}

	public void OnAddDefenceSkillPressed()
	{
		SpawnRoleSkillSelectionMenu skillSelectionMenu = Object.Instantiate(skillSelectionMenuPrefab);
		skillSelectionMenu.Initialize(skillDataManagerPrefab, attributeSettingsManager.AvailableAttributeSettings, SkillCategory.Defence, spawnRoleCost, spawnRoleTier, AddSkillCallback, CantAddSkillCallback);
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
		{
			x.Push(skillSelectionMenu.gameObject, UIPushOption.InvisibleBlocker, null, UIGroupFlags.InventoryUI);
		});
	}

	public void OnAddOffenceSkillPressed()
	{
		SpawnRoleSkillSelectionMenu skillSelectionMenu = Object.Instantiate(skillSelectionMenuPrefab);
		skillSelectionMenu.Initialize(skillDataManagerPrefab, attributeSettingsManager.AvailableAttributeSettings, SkillCategory.Offence, spawnRoleCost, spawnRoleTier, AddSkillCallback, CantAddSkillCallback);
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
		{
			x.Push(skillSelectionMenu.gameObject, UIPushOption.InvisibleBlocker, null, UIGroupFlags.InventoryUI);
		});
	}

	public void OnAddTacticalSkillPressed()
	{
		SpawnRoleSkillSelectionMenu skillSelectionMenu = Object.Instantiate(skillSelectionMenuPrefab);
		skillSelectionMenu.Initialize(skillDataManagerPrefab, attributeSettingsManager.AvailableAttributeSettings, SkillCategory.Tactical, spawnRoleCost, spawnRoleTier, AddSkillCallback, CantAddSkillCallback);
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
		{
			x.Push(skillSelectionMenu.gameObject, UIPushOption.InvisibleBlocker, null, UIGroupFlags.InventoryUI);
		});
	}

	private void AddSkillCallback(KogamaSettingValueWrapperBase attributeSetting)
	{
		attributeSettingsManager.UpdateSetting(attributeSetting);
		attributeSettingsManager.Submit();
		CreateSkillSetting(attributeSetting.Key, attributeSetting);
		updateSkillCostCallback();
	}

	private void CantAddSkillCallback()
	{
		ShowPowerErrorTipBubble();
	}

	private void CantUpdateSkillCallback()
	{
		ShowPowerErrorTipBubble();
	}

	private void CantRemoveSkillCallback()
	{
		ShowPowerErrorTipBubble();
	}

	private void ShowPowerErrorTipBubble()
	{
		cantAddSkillInfoTextBubble.Activate("The Power of the Class will be too high. Upgrade to Tier 1 or higher to proceed.");
	}

	private void RemoveSkillCallback(KogamaSettingValueWrapperBase attributeSetting)
	{
		attributeSettingsManager.RemoveAvatarSetting(attributeSetting);
		attributeSettingsManager.Submit();
		updateSkillCostCallback();
	}

	private void UpdateSkillCallback(KogamaSettingValueWrapperBase attributeSetting)
	{
		attributeSettingsManager.UpdateSetting(attributeSetting);
		updateSkillCostCallback();
	}
}
