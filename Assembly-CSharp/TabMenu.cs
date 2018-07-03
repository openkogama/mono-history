using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TabMenu : TabMenuBase
{
	private Dictionary<int, TabMenuButtonBase> buttons = new Dictionary<int, TabMenuButtonBase>();

	[SerializeField]
	private Text pages;

	[SerializeField]
	private TabMenuButtonBase tabMenuButtonPrefab;

	public override void AddTabMenuButton(int categoryIndex, string categoryName)
	{
		TabMenuButtonBase tabMenuButtonBase = Object.Instantiate(tabMenuButtonPrefab);
		tabMenuButtonBase.Initialize(categoryIndex, categoryName);
		tabMenuButtonBase.transform.SetParent(transform, worldPositionStays: false);
		buttons.Add(categoryIndex, tabMenuButtonBase);
	}

	public override void SelectTab(int tab, int currentPage, int maxPages)
	{
		pages.text = string.Empty;
		if (maxPages > 1)
		{
			pages.text = $"{currentPage}/{maxPages}";
		}
		foreach (KeyValuePair<int, TabMenuButtonBase> button in buttons)
		{
			button.Value.SetAsDeselected();
		}
		buttons[tab].SetAsSelected();
	}
}
