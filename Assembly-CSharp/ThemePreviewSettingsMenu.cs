using System;
using System.Collections.Generic;
using MV.Common;
using MV.WorldObject.ThemesData;
using UnityEngine;
using UnityEngine.EventSystems;

public class ThemePreviewSettingsMenu : ThemeSettingsMenuBase
{
	[SerializeField]
	private RectTransform topArea;

	[SerializeField]
	private SwitchThemeButton switchThemeButtonPrefab;

	private Theme previewTheme;

	private int previewID;

	private string previewDisplayName;

	private ThemeMenuController menuController;

	private ConfirmationPopup openPopup;

	private ThemeSettingsSideBar sideBar;

	public void Initialize(Theme theme, ThemeData data, ThemeMenuController menuController)
	{
		sideBar = UnityEngine.Object.Instantiate(sideBarPrefab);
		sideBar.transform.SetParent(settingsArea, worldPositionStays: false);
		sideBar.InitializeForPreview();
		Initialize(theme, sideBar.Content);
		previewTheme = theme;
		previewID = data.id;
		this.menuController = menuController;
		SwitchThemeButton switchThemeButton = UnityEngine.Object.Instantiate(switchThemeButtonPrefab);
		switchThemeButton.transform.SetParent(settingsArea, worldPositionStays: false);
		switchThemeButton.LevelRequirement = data.levelRequirement;
		switchThemeButton.Button.onClick.AddListener(() =>
		{
			SwitchThemeButtonClicked(data.id, data.levelRequirement);
		});
	}

	private void SwitchThemeButtonClicked(int themeID, int levelReq)
	{
		if (MVGameControllerBase.Game.LocalPlayer.Level < levelReq)
		{
			DisplayInsufficientLevelNotification(levelReq);
		}
		else if (previewTheme.OverrideSkyboxManager)
		{
			DisplaySkyboxWarning();
		}
		else
		{
			DisplayThemeSwitchWarning();
		}
	}

	private void DisplayInsufficientLevelNotification(int levelReq)
	{
		string msg = string.Format("{0} {1}.{2} {3}.", TM._("In order to buy this theme you'll need to reach level"), levelReq, TM._("You're currently level"), MVGameControllerBase.Game.LocalPlayer.Level);
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IModalPopupCreator x, BaseEventData y) =>
		{
			x.CreateErrorNotificationPopup(msg);
		});
	}

	private void DisplaySkyboxWarning()
	{
		string msg = TM._("You will not be able to use skyboxes while this theme is in use. Any active skyboxes will be disabled, and you will not be able to place new ones.");
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IModalPopupCreator x, BaseEventData y) =>
		{
			x.Create(msg, OnSkyboxWarningResolved, TM._("Theme purchase"));
		});
	}

	private void OnSkyboxWarningResolved(bool b, ConfirmationPopup popup)
	{
		popup.Pop();
		if (b)
		{
			DisplayThemeSwitchWarning();
		}
	}

	private void DisplayThemeSwitchWarning()
	{
		string msg = TM._("By purchasing a new theme, you are deleting the one currently in use. Your old theme will be permanently lost.");
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IModalPopupCreator x, BaseEventData y) =>
		{
			x.Create(msg, OnThemeSwitchWarningResolved, "Theme purchase");
		});
	}

	private void OnThemeSwitchWarningResolved(bool b, ConfirmationPopup popup)
	{
		if (b)
		{
			openPopup = popup;
			MVNetworkGame game = MVGameControllerBase.Game;
			game.PurchaseProductResponseHandler = (Action<int, Dictionary<object, object>>)Delegate.Combine(game.PurchaseProductResponseHandler, new Action<int, Dictionary<object, object>>(OnPurchaseResponse));
			previewTheme.Purchase(previewID);
		}
		else
		{
			popup.Pop();
		}
	}

	private void OnPurchaseResponse(int returnCode, Dictionary<object, object> purchaseResponseData)
	{
		MVNetworkGame game = MVGameControllerBase.Game;
		game.PurchaseProductResponseHandler = (Action<int, Dictionary<object, object>>)Delegate.Remove(game.PurchaseProductResponseHandler, new Action<int, Dictionary<object, object>>(OnPurchaseResponse));
		openPopup.Pop();
		switch ((MVPurchaseReturnCode)returnCode)
		{
		case MVPurchaseReturnCode.Success:
			ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
			{
				x.PopGroups(UIGroupFlags.InventoryUI);
			});
			menuController.OpenSettings(ThemeRepository.Instance.CurrentlySerializedTheme);
			ExecuteEvents.ExecuteHierarchy(gameObject, null, (IModalPopupCreator x, BaseEventData y) =>
			{
				x.Create(TM._("Purchase successful!"), TM._("Theme purchase"));
			});
			break;
		case MVPurchaseReturnCode.InsufficientFunds:
			ExecuteEvents.ExecuteHierarchy(gameObject, null, (IModalPopupCreator x, BaseEventData y) =>
			{
				x.Create(TM._("You do not have enough gold to buy this theme."), TM._("Theme purchase"));
			});
			break;
		case MVPurchaseReturnCode.InsufficientLevel:
			break;
		case MVPurchaseReturnCode.AlreadyPurchased:
		case MVPurchaseReturnCode.ProductSubtypeNotSold:
		case MVPurchaseReturnCode.Failed:
			ExecuteEvents.ExecuteHierarchy(gameObject, null, (IModalPopupCreator x, BaseEventData y) =>
			{
				x.Create(TM._("Something went wrong. The purchase could not be made. Please try again later."), TM._("Theme purchase"));
			});
			break;
		default:
			Debug.LogError("Unexpected purchase response.");
			break;
		}
	}
}
