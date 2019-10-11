using System.Collections.Generic;
using MV.Common;
using MV.WorldObject.KogamaSettings.KogamaSettingsCore.KogamaSettingTypes;
using MV.WorldObject.KogamaSettings.SpecializedSettingsTypes.AttributeSettings;
using MV.WorldObject.KogamaSettings.SpecializedSettingsTypes.AttributeSettings.AttributeSettingTypes;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.UI;

public class SpawnRoleSelectionElement : DefaultSpawnRoleSelectionElement
{
	[SerializeField]
	private Text spawnRoleCostAmount;

	[SerializeField]
	private GameObject spawnRoleCostObject;

	[SerializeField]
	private NotificationFade spawnRoleCostFader;

	[SerializeField]
	private GameObject moreInfoButton;

	[SerializeField]
	private NotificationFade moreInfoButtonFader;

	[SerializeField]
	private SpawnRoleSelectionSkillMenu skillMenuPrefab;

	[SerializeField]
	private GameObject backgroundTier1;

	[SerializeField]
	private GameObject backgroundTier2;

	[SerializeField]
	private GameObject backgroundTier3;

	private GamePassTier tierRequirement;

	public override GamePassTier Tier => tierRequirement;

	private void ChangeBackground(GamePassTier tier)
	{
		bool flag = tier == GamePassTier.Tier1;
		bool flag2 = tier == GamePassTier.Tier2;
		bool flag3 = tier == GamePassTier.Tier3;
		if (backgroundTier1.activeSelf != flag)
		{
			backgroundTier1.SetActive(flag);
		}
		if (backgroundTier2.activeSelf != flag2)
		{
			backgroundTier2.SetActive(flag2);
		}
		if (backgroundTier3.activeSelf != flag3)
		{
			backgroundTier3.SetActive(flag3);
		}
	}

	public override void Initialize(int spawnRoleIndex, int woId, GamePassTier tierRequirement, UnityAction<int> onSelectedCallback, UnityAction<int> onActivatedCallback)
	{
		base.Initialize(spawnRoleIndex, woId, tierRequirement, onSelectedCallback, onActivatedCallback);
		this.tierRequirement = tierRequirement;
		int skillCost = CalculateTotalSpawnRoleCost(woId);
		spawnRoleCostAmount.text = skillCost.ToString();
		spawnRoleCostAmount.color = SpawnRolesSkillDataManager.GetCostColor(skillCost);
		spawnRoleCostObject.SetActive(value: false);
		moreInfoButton.SetActive(value: false);
		spawnRoleCostFader.ShouldHideWhenDone = false;
		moreInfoButtonFader.ShouldHideWhenDone = false;
		ChangeBackground(tierRequirement);
	}

	public void ShowSkillMenu()
	{
		OnShowSkillMenu();
	}

	public void OnShowSkillMenu()
	{
		SpawnRoleSelectionSkillMenu skillMenu = Object.Instantiate(skillMenuPrefab);
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
		{
			x.Push(skillMenu.gameObject, UIPushOption.InvisibleBlocker, null, UIGroupFlags.InventoryUI);
		});
		skillMenu.Initialize(WOID, tierRequirement, spawnRolePreviewObject);
	}

	private int CalculateTotalSpawnRoleCost(int spawnRoleId)
	{
		MVAvatarSpawnRoleCreator mVAvatarSpawnRoleCreator = (MVAvatarSpawnRoleCreator)MVGameControllerBase.WOCM.GetWorldObject(spawnRoleId);
		AttributeSettingsManager attributeSettingsManagerAvatar = mVAvatarSpawnRoleCreator.AttributeSettingsManagerAvatar;
		KogamaSettingsCollectionBase kogamaSettingsCollectionBase = (KogamaSettingsCollectionBase)attributeSettingsManagerAvatar.Settings;
		if (kogamaSettingsCollectionBase == null)
		{
			return 0;
		}
		int num = 0;
		foreach (KeyValuePair<string, KogamaSettingWrapperBase> child in kogamaSettingsCollectionBase.Children)
		{
			num += ((IAttributeSetting)child.Value).AttributeValue;
		}
		return num;
	}

	public override void OnSelctionHighlight()
	{
		base.OnSelctionHighlight();
	}

	public override void OnSelected()
	{
		base.OnSelected();
		spawnRoleCostObject.SetActive(value: true);
		moreInfoButton.SetActive(value: true);
		spawnRoleCostFader.Activate();
		moreInfoButtonFader.Activate();
	}

	public override void OnUnSelected()
	{
		base.OnUnSelected();
		spawnRoleCostObject.SetActive(value: false);
		moreInfoButton.SetActive(value: false);
	}
}
