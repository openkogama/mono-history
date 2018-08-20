using MV.WorldObject.ThemesData;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class ThemeMenuController : MonoBehaviour, ThemeMenuButton.IClickHandler, IEventSystemHandler
{
	[SerializeField]
	private ThemeSelection selectionPrefab;

	[SerializeField]
	private ThemeSettingsMenu settingsPrefab;

	[SerializeField]
	private ThemePreviewSettingsMenu previewSettingsPrefab;

	public void OpenThemesMenu()
	{
		if (ThemeRepository.Instance.ThemeIsActive)
		{
			OpenSettings(ThemeRepository.Instance.CurrentThemeVisualization);
			return;
		}
		ThemeSelection menu = Object.Instantiate(selectionPrefab);
		menu.Initialize(this);
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
		{
			x.Push(menu.gameObject, UIPushOption.Blocking, () =>
			{
			}, UIGroupFlags.InventoryUI);
		});
	}

	public void OpenSelection()
	{
		ThemeSelection menu = Object.Instantiate(selectionPrefab);
		menu.InitializeWithBackButton(this);
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
		{
			x.Push(menu.gameObject, UIPushOption.Blocking, () =>
			{
			}, UIGroupFlags.InventoryUI);
		});
	}

	public void OpenSettings(Theme theme)
	{
		ThemeSettingsMenu settings = Object.Instantiate(settingsPrefab);
		settings.Initialize(this, theme);
		UnityAction commitChanges = () =>
		{
			theme.Settings.CommitChanges();
		};
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
		{
			x.Push(settings.gameObject, UIPushOption.HideAll | UIPushOption.InvisibleBlocker, commitChanges, UIGroupFlags.InventoryUI);
		});
	}

	public void OpenSettingsForPreview(Theme theme, ThemeData entry)
	{
		ThemePreviewSettingsMenu settings = Object.Instantiate(previewSettingsPrefab);
		settings.Initialize(theme, entry, this);
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
		{
			x.Push(settings.gameObject, UIPushOption.HideAll | UIPushOption.InvisibleBlocker, () =>
			{
				ThemeRepository.Instance.DestroyTemporary(theme);
			}, UIGroupFlags.InventoryUI);
		});
	}
}
