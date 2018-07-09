using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TabMenuAccessoryShop : TabMenuBase
{
	[Serializable]
	private class TabMenuButtonNonLayoutDef
	{
		public AccessoryCategoryClient category = AccessoryCategoryClient.Bundles;

		public TabMenuButtonBase tabMenuButton;
	}

	private Dictionary<int, TabMenuButtonBase> buttons = new Dictionary<int, TabMenuButtonBase>();

	[SerializeField]
	private Text pages;

	[SerializeField]
	private TabMenuButtonBase tabMenuButtonPrefab;

	[SerializeField]
	private List<GameObject> pageButtons;

	[SerializeField]
	private List<TabMenuButtonNonLayoutDef> nonLayoutTabButtons;

	public TabMenuButtonBase GetTabMenuButton(AccessoryCategoryClient category)
	{
		if (!buttons.ContainsKey((int)category))
		{
			return null;
		}
		return buttons[(int)category];
	}

	public void DestroyTab(AccessoryCategoryClient category)
	{
		if (buttons.ContainsKey((int)category))
		{
			UnityEngine.Object.Destroy(buttons[(int)category].gameObject);
			buttons.Remove((int)category);
		}
	}

	public override void AddTabMenuButton(int categoryIndex, string categoryName)
	{
		TabMenuButtonBase tabMenuButtonBase = null;
		for (int i = 0; i < nonLayoutTabButtons.Count; i++)
		{
			if (nonLayoutTabButtons[i].category == (AccessoryCategoryClient)categoryIndex)
			{
				tabMenuButtonBase = UnityEngine.Object.Instantiate(nonLayoutTabButtons[i].tabMenuButton);
				break;
			}
		}
		if (tabMenuButtonBase == null)
		{
			tabMenuButtonBase = UnityEngine.Object.Instantiate(tabMenuButtonPrefab);
		}
		tabMenuButtonBase.Initialize(categoryIndex, categoryName);
		tabMenuButtonBase.transform.SetParent(transform, worldPositionStays: false);
		buttons.Add(categoryIndex, tabMenuButtonBase);
	}

	public override void SelectTab(int tab, int currentPage, int maxPages)
	{
		pages.text = $"{currentPage}/{maxPages}";
		for (int i = 0; i < pageButtons.Count; i++)
		{
			pageButtons[i].SetActive(maxPages > 1);
		}
		foreach (KeyValuePair<int, TabMenuButtonBase> button in buttons)
		{
			button.Value.SetAsDeselected();
		}
		buttons[tab].SetAsSelected();
	}
}
