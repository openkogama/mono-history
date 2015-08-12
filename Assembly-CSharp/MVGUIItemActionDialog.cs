using System;
using MV.WorldObject;
using UnityEngine;

public class MVGUIItemActionDialog : UXCustomDialogBox
{
	private const int INFO_TAB = 0;

	private const int SELL_TAB = 1;

	private const int MANAGE_TAB = 2;

	public UXTabWindow tabWindow;

	private MVItem mvItem;

	private UXView parentView;

	private bool _isInitialized;

	public override void OnShowDialog()
	{
		if (!mvItem.resellable)
		{
			tabWindow.GetTab(1).SetVisible(visible: false);
		}
		if (mvItem.itemCategoryID != 1)
		{
			int num = MVGameController.Game.PlayerRepository.CountItemsWithOriginalID(mvItem.originalItemID);
			tabWindow.GetTab(2).SetVisible(num > 1);
		}
		if (_isInitialized)
		{
			return;
		}
		foreach (UXTabPane tabPane in tabWindow.TabPanes)
		{
			if (tabPane.Visible)
			{
				MVGUIItemAction component = tabPane.TabGroup.GetComponent<MVGUIItemAction>();
				component.UpdateItemAction(mvItem);
				component.OnActionCompleted = (MVGUIItemAction.OnActionCompletedDelegate)Delegate.Combine(component.OnActionCompleted, new MVGUIItemAction.OnActionCompletedDelegate(OnActionCompleted));
			}
		}
		UXTabWindow uXTabWindow = tabWindow;
		uXTabWindow.OnTabSelect = (UXTabWindow.OnTabSelectedDelegate)Delegate.Combine(uXTabWindow.OnTabSelect, new UXTabWindow.OnTabSelectedDelegate(OnTabSelected));
		parentView = (UXView)UXUtils.FindComponentInParents(typeof(UXView), transform.parent);
		_isInitialized = true;
	}

	private void OnTabSelected(int tabId)
	{
		tabWindow.SetHeaderText(tabWindow.GetTab(tabId).GetHeaderText());
		parentView.ReleaseFocus();
	}

	public void SetMVItem(MVItem item)
	{
		mvItem = item;
	}

	private void OnActionCompleted()
	{
		OnPositiveClose();
		UXUtils.UXDialogFactory.CloseDialog();
	}

	public override Vector2 GetSize()
	{
		return tabWindow.Size;
	}
}
