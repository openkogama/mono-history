using UnityEngine;

public class ThemeSettingsMenu : ThemeSettingsMenuBase
{
	[SerializeField]
	private TextButton browseThemesButton;

	private ThemeSettingsSideBar sideBar;

	public void Initialize(ThemeMenuController menuController, Theme theme)
	{
		sideBar = Object.Instantiate(sideBarPrefab);
		sideBar.transform.SetParent(settingsArea, worldPositionStays: false);
		Initialize(theme, sideBar.Content);
		browseThemesButton = Object.Instantiate(browseThemesButton);
		browseThemesButton.transform.SetParent(settingsArea, worldPositionStays: false);
		browseThemesButton.Button.onClick.AddListener(() =>
		{
			menuController.OpenSelection();
		});
	}
}
