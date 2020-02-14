using System;
using MV.Common;
using MV.WorldObject.GamePassSystem.GamePassEarnings;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GameSetupMenu : MonoBehaviour
{
	[SerializeField]
	private GameObject activeGameTierButton;

	[SerializeField]
	private GameObject inActiveGameTierButton;

	[SerializeField]
	private Text earningsAmountText;

	[SerializeField]
	private GamePassesShopDetails gamePassesShopDetailsPrefab;

	[SerializeField]
	private BoostEditMenu boosterEditMenuPrefab;

	[SerializeField]
	private GameEarningsMenu earningsMenuPrefab;

	[SerializeField]
	private GameObject crystalPopupPrefab;

	[SerializeField]
	private GameSetupOptions optionsMenuPrefab;

	private void Start()
	{
		UpdateTierButtonVisibility();
		ProjectEarningsReport projectEarningReport = GamePassesProjectEarningsManager.ProjectEarningReport;
		if (projectEarningReport != null)
		{
			UpdateEarningText(projectEarningReport);
		}
		else
		{
			GamePassesProjectEarningsManager.OnEarningsDataUpdated = (Action<ProjectEarningsReport>)Delegate.Combine(GamePassesProjectEarningsManager.OnEarningsDataUpdated, new Action<ProjectEarningsReport>(OnRecieveEarningsReport));
		}
	}

	private void OnDestroy()
	{
		GamePassesProjectEarningsManager.OnEarningsDataUpdated = (Action<ProjectEarningsReport>)Delegate.Remove(GamePassesProjectEarningsManager.OnEarningsDataUpdated, new Action<ProjectEarningsReport>(OnRecieveEarningsReport));
	}

	private void UpdateTierButtonVisibility()
	{
		if (MVClientSettings.IsFlagSet(ClientSettingFlags.GamePassSilentReleaseEnabled))
		{
			if (activeGameTierButton.activeSelf)
			{
				activeGameTierButton.SetActive(value: false);
			}
			if (inActiveGameTierButton.activeSelf)
			{
				inActiveGameTierButton.SetActive(value: false);
			}
			return;
		}
		bool isProgressionEnabled = GamePassProgressionController.IsProgressionEnabled;
		if (activeGameTierButton.activeSelf != isProgressionEnabled)
		{
			activeGameTierButton.SetActive(isProgressionEnabled);
		}
		if (inActiveGameTierButton.activeSelf == isProgressionEnabled)
		{
			inActiveGameTierButton.SetActive(!isProgressionEnabled);
		}
	}

	private void OnRecieveEarningsReport(ProjectEarningsReport projectEarningsReport)
	{
		GamePassesProjectEarningsManager.OnEarningsDataUpdated = (Action<ProjectEarningsReport>)Delegate.Remove(GamePassesProjectEarningsManager.OnEarningsDataUpdated, new Action<ProjectEarningsReport>(OnRecieveEarningsReport));
		UpdateEarningText(projectEarningsReport);
	}

	private void UpdateEarningText(ProjectEarningsReport projectEarningsReport)
	{
		int profileID = MVGameControllerBase.Game.LocalPlayer.ProfileID;
		if (!projectEarningsReport.projectMemberEarningsReports.ContainsKey(profileID))
		{
			earningsAmountText.text = "0";
		}
		else
		{
			earningsAmountText.text = projectEarningsReport.projectMemberEarningsReports[profileID].earningsReport.TotalEarningsGold.ToString("N0").Replace(",", ".");
		}
	}

	public void ShowGamePassesShopDetails()
	{
		GamePassesShopDetails gameShopDetails = UnityEngine.Object.Instantiate(gamePassesShopDetailsPrefab);
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
		{
			x.Push(gameShopDetails.gameObject, UIPushOption.HideAll | UIPushOption.InvisibleBlocker, null, UIGroupFlags.InventoryUI);
		});
	}

	public void ShowBoostEditMenu()
	{
		BoostEditMenu boostEditMenu = UnityEngine.Object.Instantiate(boosterEditMenuPrefab);
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
		{
			x.Push(boostEditMenu.gameObject, UIPushOption.HideAll | UIPushOption.InvisibleBlocker, null, UIGroupFlags.InventoryUI);
		});
	}

	public void OnInactiveTierButtonPressed()
	{
		GameObject popup = UnityEngine.Object.Instantiate(crystalPopupPrefab);
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
		{
			x.Push(popup, UIPushOption.Blocking, null, UIGroupFlags.Popup);
		});
	}

	public void ShowGameEarnings()
	{
		GameEarningsMenu earningsMenu = UnityEngine.Object.Instantiate(earningsMenuPrefab);
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
		{
			x.Push(earningsMenu.gameObject, UIPushOption.HideAll | UIPushOption.InvisibleBlocker, null, UIGroupFlags.InventoryUI);
		});
	}

	public void ShowMiscOptions()
	{
		GameSetupOptions optionsMenu = UnityEngine.Object.Instantiate(optionsMenuPrefab);
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
		{
			x.Push(optionsMenu.gameObject, UIPushOption.HideAll | UIPushOption.InvisibleBlocker, null, UIGroupFlags.InventoryUI);
		});
	}
}
