using ThemeSettings;
using UnityEngine;

public abstract class ThemeSettingsMenuBase : MonoBehaviour, IMenu
{
	[SerializeField]
	protected ThemeSettingsSideBar sideBarPrefab;

	[SerializeField]
	protected RectTransform settingsArea;

	[SerializeField]
	protected RectTransform controllerArea;

	private Theme theme;

	private RectTransform content;

	protected void Initialize(Theme theme, RectTransform content)
	{
		this.theme = theme;
		this.content = content;
		theme.Settings.SubscribeToSettingsUI(this);
	}

	public void Refresh()
	{
		foreach (RectTransform item in content)
		{
			Object.Destroy(item.gameObject);
		}
		RectTransform[] settingsUI = theme.Settings.SettingsUI;
		foreach (RectTransform rectTransform2 in settingsUI)
		{
			rectTransform2.SetParent(content, worldPositionStays: false);
		}
		foreach (RectTransform item2 in controllerArea)
		{
			Object.Destroy(item2.gameObject);
		}
		foreach (RectTransform controller in theme.Controllers)
		{
			controller.SetParent(controllerArea, worldPositionStays: false);
		}
	}

	protected void OnDestroy()
	{
		theme.Settings.UnsubscribeToSettingsUI();
	}
}
