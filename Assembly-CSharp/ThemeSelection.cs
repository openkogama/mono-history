using System;
using MV.WorldObject.ThemesData;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.UI;

public class ThemeSelection : MonoBehaviour
{
	public static class CallbackHandler
	{
		public static Action<ThemeData[]> OnThemeDataReceived;
	}

	public const string menuDescription = "_(\"Hover over a theme to know more about it. Click it to see it in game.\")";

	[SerializeField]
	private ThemeRepository themeRepo;

	[SerializeField]
	private ThemeSelectionButton buttonPrefab;

	[SerializeField]
	private Button themeRemovalButtonPrefab;

	[SerializeField]
	private RectTransform themeButtonContainer;

	[SerializeField]
	private Text themeDescription;

	[SerializeField]
	private Image close;

	[SerializeField]
	private Image back;

	public string currentDescription = "_(\"Hover over a theme to know more about it. Click it to see it in game.\")";

	private ThemeMenuController menuController;

	private Theme previewTheme;

	public string ThemeDescription
	{
		set
		{
			currentDescription = value;
			LocalizeAndSetDescriptionText();
		}
	}

	private Theme PreviewTheme
	{
		get
		{
			return previewTheme;
		}
		set
		{
			if (previewTheme != null)
			{
				UnityEngine.Object.Destroy(previewTheme.gameObject);
			}
			previewTheme = value;
		}
	}

	public void InitializeWithBackButton(ThemeMenuController menuController)
	{
		close.gameObject.SetActive(value: false);
		back.gameObject.SetActive(value: true);
		Initialize(menuController);
	}

	public void Initialize(ThemeMenuController menuController)
	{
		this.menuController = menuController;
	}

	protected void Awake()
	{
		CallbackHandler.OnThemeDataReceived = CreateThemeButtons;
		MVGameControllerBase.OperationRequests.GetThemesData();
		LocalizeAndSetDescriptionText();
		TM.LanguageChanged(LocalizeAndSetDescriptionText);
	}

	protected void OnDestroy()
	{
		CallbackHandler.OnThemeDataReceived = null;
	}

	private void CreateThemeButtons(ThemeData[] data)
	{
		if (themeRepo.ThemeIsActive)
		{
			Button button = UnityEngine.Object.Instantiate(themeRemovalButtonPrefab);
			button.transform.SetParent(themeButtonContainer, worldPositionStays: false);
			button.onClick.AddListener(() =>
			{
				ShowThemeRemovalWarning();
			});
		}
		foreach (ThemeData themeData in data)
		{
			ThemeSelectionButton b = UnityEngine.Object.Instantiate(buttonPrefab);
			b.transform.SetParent(themeButtonContainer, worldPositionStays: false);
			if (themeRepo.CurrentThemeIdentifier == themeData.themeIdentifier)
			{
				UnityAction buttonClickedCB = () =>
				{
					GoBackToSettings();
				};
				b.Initialize(this, themeRepo.GetThemePrefab(themeData.themeIdentifier), buttonClickedCB, themeData.priceGold, themeData.levelRequirement, alreadyInUse: true);
				continue;
			}
			string identifier = themeData.themeIdentifier;
			ThemeData copy = themeData;
			UnityAction buttonClickedCB2 = () =>
			{
				b.EventTriggerCB_MouseExit();
				LoadTheme(identifier, copy);
			};
			b.Initialize(this, themeRepo.GetThemePrefab(themeData.themeIdentifier), buttonClickedCB2, themeData.priceGold, themeData.levelRequirement, alreadyInUse: false);
		}
	}

	private void LoadTheme(string identifier, ThemeData data)
	{
		PreviewTheme = themeRepo.CreateTemporaryThemeVisualization(identifier);
		menuController.OpenSettingsForPreview(PreviewTheme, data);
	}

	private void ShowThemeRemovalWarning()
	{
		string msg = string.Format("{0}\n{1}", TM._("This will remove the current theme. Your theme settings will be permanently lost.)"), TM._("Are you sure you want to do this?"));
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IModalPopupCreator x, BaseEventData y) =>
		{
			x.Create(msg, OnThemeRemovalWarningResolved, TM._("Theme removal"));
		});
	}

	private void OnThemeRemovalWarningResolved(bool answer, ConfirmationPopup popup)
	{
		if (answer)
		{
			ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
			{
				x.PopGroups(UIGroupFlags.InventoryUI);
			});
			MVGameControllerBase.OperationRequests.UnregisterWorldObject(ThemeRepository.Instance.CurrentThemeWoid);
		}
		popup.Pop();
	}

	private void GoBackToSettings()
	{
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
		{
			x.Pop();
		});
	}

	private void LocalizeAndSetDescriptionText()
	{
		themeDescription.text = TM._(currentDescription);
	}
}
