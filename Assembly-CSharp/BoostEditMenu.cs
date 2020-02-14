using System;
using System.Collections.Generic;
using MV.WorldObject.GamePassSystem.GamePassEarnings;
using MV.WorldObject.KogamaSettings.SpecializedSettingsTypes.GameBoosterSettings.GameBoosterSettingTypes;
using UnityEngine;
using UnityEngine.UI;

public class BoostEditMenu : MonoBehaviour
{
	[SerializeField]
	private BoostEditMenuItem boostPrefab;

	[SerializeField]
	private RectTransform boostItemsScrollRect;

	[SerializeField]
	private RectTransform boostItemsContent;

	private List<BoostEditMenuItem> boostItems = new List<BoostEditMenuItem>();

	public void Start()
	{
		MVGameOptionDataObject singletonWorldObject = MVGameControllerBase.WOCM.GetSingletonWorldObject<MVGameOptionDataObject>();
		BoostController boostController = MVGameControllerBase.Game.LocalPlayer.BoostController;
		Dictionary<BoostType, Boost>.ValueCollection allBoosts = boostController.GetAllBoosts();
		foreach (Boost item in allBoosts)
		{
			List<GameBoosterSettingWithGoldSetting> activeSettingsList = singletonWorldObject.GameBoosterSettingsManager.ActiveSettingsList;
			activeSettingsList.AddRange(singletonWorldObject.GameBoosterSettingsManager.InactiveGameBoosterSettingsList);
			for (int i = 0; i < activeSettingsList.Count; i++)
			{
				if (item.BoostKey == activeSettingsList[i].Key)
				{
					BoostEditMenuItem boostEditMenuItem = UnityEngine.Object.Instantiate(boostPrefab);
					boostEditMenuItem.transform.SetParent(boostItemsContent, worldPositionStays: false);
					boostEditMenuItem.Initialize(item);
					boostItems.Add(boostEditMenuItem);
				}
			}
		}
		LayoutRebuilder.ForceRebuildLayoutImmediate(boostItemsContent);
		if (boostItemsScrollRect.rect.width < boostItemsContent.rect.width)
		{
			boostItemsContent.pivot = new Vector2(0f, 0.5f);
		}
		ProjectEarningsReport projectEarningReport = GamePassesProjectEarningsManager.ProjectEarningReport;
		if (projectEarningReport != null)
		{
			UpdateEarningsData(projectEarningReport);
		}
		else
		{
			GamePassesProjectEarningsManager.OnEarningsDataUpdated = (Action<ProjectEarningsReport>)Delegate.Combine(GamePassesProjectEarningsManager.OnEarningsDataUpdated, new Action<ProjectEarningsReport>(OnProjectEarningsUpdatedCallback));
		}
	}

	private void UpdateEarningsData(ProjectEarningsReport projectEarningsReport)
	{
		for (int i = 0; i < boostItems.Count; i++)
		{
			boostItems[i].UpdateEarningsText(projectEarningsReport);
		}
	}

	private void OnProjectEarningsUpdatedCallback(ProjectEarningsReport projectEarningsReport)
	{
		GamePassesProjectEarningsManager.OnEarningsDataUpdated = (Action<ProjectEarningsReport>)Delegate.Remove(GamePassesProjectEarningsManager.OnEarningsDataUpdated, new Action<ProjectEarningsReport>(OnProjectEarningsUpdatedCallback));
		UpdateEarningsData(projectEarningsReport);
	}

	private void OnDestroy()
	{
		GamePassesProjectEarningsManager.OnEarningsDataUpdated = (Action<ProjectEarningsReport>)Delegate.Remove(GamePassesProjectEarningsManager.OnEarningsDataUpdated, new Action<ProjectEarningsReport>(OnProjectEarningsUpdatedCallback));
	}
}
