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
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Expected Obj, but got Unknown
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Expected Obj, but got Unknown
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		MeshRenderer val = ((Component)this).gameObject.AddComponent<MeshRenderer>();
		((Renderer)val).material = (Material)Resources.Load("Materials/UX/ListMenu/MouseOver");
		((Component)this).gameObject.AddComponent<MeshFilter>();
		itemRoot = new GameObject("MenuItemRoot");
		itemRoot.transform.parent = ((Component)this).transform;
		itemRoot.transform.localPosition = Vector3.zero;
		itemRoot.transform.localScale = Vector3.one;
		itemRoot.transform.localRotation = Quaternion.identity;
	}

	public void AddListMenuTextItem(string text, OnMenuItemClickDelegate OnMenuItemClick)
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Expected Obj, but got Unknown
		GameObject val = new GameObject("TextMenuItem");
		TextListMenuItem textListMenuItem = val.AddComponent<TextListMenuItem>();
		textListMenuItem.SetText(text);
		AddListMenuItem(textListMenuItem, OnMenuItemClick);
	}

	public void AddListMenuDivider()
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Expected Obj, but got Unknown
		GameObject val = new GameObject("MenuDivider");
		DividerListMenuItem menuItem = val.AddComponent<DividerListMenuItem>();
		AddListMenuItem(menuItem);
	}

	private void AddListMenuItem(ListMenuItem menuItem, OnMenuItemClickDelegate OnMenuItemClick = null)
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		menuItem.BuildMenuItem(MenuWidth, OnMenuItemClick);
		((Component)menuItem).transform.parent = itemRoot.transform;
		((Component)menuItem).transform.localScale = Vector3.one;
		((Component)menuItem).transform.localRotation = Quaternion.identity;
		menuItems.Add(menuItem);
		PlaceMenuItems();
		((Component)this).gameObject.GetComponent<MeshFilter>().mesh = UXUtils.BuildPlaneMesh(MenuWidth, GetTotalMenuHeight(), "ListMenuBackground");
	}

	private void PlaceMenuItems()
	{
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		float num = GetTotalMenuHeight() / 2f;
		float num2 = 0f;
		foreach (ListMenuItem menuItem in menuItems)
		{
			float height = menuItem.GetHeight();
			float num3 = num - height / 2f - num2;
			((Component)menuItem).transform.localPosition = new Vector3(0f, num3, 0f);
			num2 += menuItem.GetHeight();
		}
	}

	private Vector3 GetNextMenuItemPosition()
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
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
