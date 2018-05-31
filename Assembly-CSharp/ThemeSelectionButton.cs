using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ThemeSelectionButton : TextButton
{
	[SerializeField]
	private RawImage background;

	[SerializeField]
	private GameObject nameLabel;

	[SerializeField]
	private RectTransform previewImageArea;

	[SerializeField]
	[Header("Configuration")]
	private Color selectedColor;

	private Color normalColor;

	private ThemeSelection selectionMenu;

	private Theme themePrefab;

	private bool themeInUse;

	private string themeName;

	public void Initialize(ThemeSelection selectionMenu, Theme themePrefab, UnityAction buttonClickedCB, int price, int levelReq, bool alreadyInUse)
	{
		this.selectionMenu = selectionMenu;
		this.themePrefab = themePrefab;
		Object.Instantiate(themePrefab.ThemeButtonImagePrefab).SetParent(previewImageArea, worldPositionStays: false);
		themeName = themePrefab.DisplayName;
		LocalizeAndSetThemeName();
		TM.LanguageChanged(LocalizeAndSetThemeName);
		normalColor = background.color;
		if (alreadyInUse)
		{
			background.color = selectedColor;
		}
		Button.onClick.AddListener(buttonClickedCB);
		themeInUse = alreadyInUse;
		nameLabel.SetActive(themeInUse);
	}

	public void EventTriggerCB_MouseEnter()
	{
		selectionMenu.ThemeDescription = themePrefab.Description;
		if (!themeInUse)
		{
			nameLabel.SetActive(value: true);
			background.color = selectedColor;
		}
	}

	public void EventTriggerCB_MouseExit()
	{
		selectionMenu.ThemeDescription = "_(\"Hover over a theme to know more about it. Click it to see it in game.\")";
		if (!themeInUse)
		{
			nameLabel.SetActive(value: false);
			background.color = normalColor;
		}
	}

	private void LocalizeAndSetThemeName()
	{
		Text.text = TM._(themeName);
	}
}
