using UnityEngine;

public abstract class TabMenuBase : MonoBehaviour
{
	public abstract void AddTabMenuButton(int categoryIndex, string categoryName);

	public abstract void SelectTab(int tab, int currentPage, int maxPages);
}
