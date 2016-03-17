using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TabMenu : MonoBehaviour
{
	private Dictionary<int, TabMenuButton> buttons = new Dictionary<int, TabMenuButton>();

	[SerializeField]
	private Text pages;

	[SerializeField]
	private TabMenuButton tabMenuButtonPrefab;

	public void AddTabMenuButton(int categoryIndex, string categoryName)
	{
		TabMenuButton tabMenuButton = Object.Instantiate(tabMenuButtonPrefab);
		tabMenuButton.Initialize(categoryIndex, categoryName);
		tabMenuButton.transform.SetParent(transform, worldPositionStays: false);
		buttons.Add(categoryIndex, tabMenuButton);
	}

	public void SelectTab(int tab, int currentPage, int maxPages)
	{
		pages.text = $"{currentPage} / {maxPages}";
		foreach (KeyValuePair<int, TabMenuButton> button in buttons)
		{
			button.Value.SetAsDeselected();
		}
		buttons[tab].SetAsSelected();
	}
}
