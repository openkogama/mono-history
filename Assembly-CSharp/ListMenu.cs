using System.Collections.Generic;
using UnityEngine;

public class ListMenu : MonoBehaviour
{
	public delegate void OnMenuItemClickDelegate();

	private List<ListMenuItem> menuItems = new List<ListMenuItem>();

	private GameObject itemRoot;

	public float MenuWidth = 10f;

	public float MenuHeight;

	public bool FitToItemHeight = true;

	public void Awake()
	{
		MeshRenderer meshRenderer = gameObject.AddComponent<MeshRenderer>();
		meshRenderer.material = PrefabPool.Instance.MouseOverMaterial;
		gameObject.AddComponent<MeshFilter>();
		itemRoot = new GameObject("MenuItemRoot");
		itemRoot.transform.parent = transform;
		itemRoot.transform.localPosition = Vector3.zero;
		itemRoot.transform.localScale = Vector3.one;
		itemRoot.transform.localRotation = Quaternion.identity;
	}

	public void AddListMenuTextItem(string text, OnMenuItemClickDelegate OnMenuItemClick)
	{
		GameObject gameObject = new GameObject("TextMenuItem");
		TextListMenuItem textListMenuItem = gameObject.AddComponent<TextListMenuItem>();
		textListMenuItem.SetText(text);
		AddListMenuItem(textListMenuItem, OnMenuItemClick);
	}

	public void AddListMenuDivider()
	{
		GameObject gameObject = new GameObject("MenuDivider");
		DividerListMenuItem menuItem = gameObject.AddComponent<DividerListMenuItem>();
		AddListMenuItem(menuItem);
	}

	private void AddListMenuItem(ListMenuItem menuItem, OnMenuItemClickDelegate OnMenuItemClick = null)
	{
		menuItem.BuildMenuItem(MenuWidth, OnMenuItemClick);
		menuItem.transform.parent = itemRoot.transform;
		menuItem.transform.localScale = Vector3.one;
		menuItem.transform.localRotation = Quaternion.identity;
		menuItems.Add(menuItem);
		PlaceMenuItems();
		UXUtils.BuildPlaneMesh(gameObject.GetComponent<MeshFilter>().mesh, MenuWidth, GetTotalMenuHeight(), "ListMenuBackground");
	}

	private void PlaceMenuItems()
	{
		float num = GetTotalMenuHeight() / 2f;
		float num2 = 0f;
		foreach (ListMenuItem menuItem in menuItems)
		{
			float height = menuItem.GetHeight();
			float y = num - height / 2f - num2;
			menuItem.transform.localPosition = new Vector3(0f, y, 0f);
			num2 += menuItem.GetHeight();
		}
	}

	private Vector3 GetNextMenuItemPosition()
	{
		return new Vector3(0f, GetTotalMenuHeight(), 0f);
	}

	private float GetTotalMenuHeight()
	{
		if (!FitToItemHeight)
		{
			return MenuHeight;
		}
		float num = 0f;
		foreach (ListMenuItem menuItem in menuItems)
		{
			num += menuItem.GetHeight();
		}
		return num;
	}
}
