using System;
using System.Collections.Generic;

public class MVGUIAggregateInventory : UXViewScript
{
	public UXTabWindow tabs;

	public List<MVGUIInventoryGroup> inventoryGroups;

	private bool _isInitialized;

	private bool _beenReset;

	private int currentTab;

	public void InitializeInventoryGroups()
	{
		foreach (MVGUIInventoryGroup inventoryGroup in inventoryGroups)
		{
			InitializeGroup(inventoryGroup);
		}
		UXTabWindow uXTabWindow = tabs;
		uXTabWindow.OnExitButtonClick = (UXWindow.OnExitButtonClickDelegate)Delegate.Combine(uXTabWindow.OnExitButtonClick, new UXWindow.OnExitButtonClickDelegate(HideAggregateInventory));
	}

	private void InitializeGroup(MVGUIInventoryGroup inventoryGroup)
	{
		inventoryGroup.NotifyItemSelection = (MVGUIInventoryGroup.OnItemSelectionDelegate)Delegate.Combine(inventoryGroup.NotifyItemSelection, new MVGUIInventoryGroup.OnItemSelectionDelegate(HideAggregateInventory));
		inventoryGroup.Initialize();
	}

	private void HideAggregateInventory()
	{
		View.Hide();
	}

	public void ResetCollectionViews()
	{
		foreach (MVGUIInventoryGroup inventoryGroup in inventoryGroups)
		{
			inventoryGroup.ResetInventoryGroup();
		}
		_beenReset = true;
	}

	public override void OnShow()
	{
		base.OnShow();
		if (!_isInitialized)
		{
			InitializeInventoryGroups();
			UXTabWindow uXTabWindow = tabs;
			uXTabWindow.OnTabSelect = (UXTabWindow.OnTabSelectedDelegate)Delegate.Combine(uXTabWindow.OnTabSelect, new UXTabWindow.OnTabSelectedDelegate(OnTabsChange));
			_isInitialized = true;
		}
		if (_beenReset)
		{
			foreach (MVGUIInventoryGroup inventoryGroup in inventoryGroups)
			{
				inventoryGroup.InitializeAfterReset();
			}
			_beenReset = false;
		}
		tabs.SelectTab(currentTab);
		UXFullscreenColliderBox.Instance.AddBlockingObject(this);
	}

	private void OnTabsChange(int tab)
	{
		currentTab = tab;
	}

	public override void OnHide()
	{
		base.OnHide();
		UXFullscreenColliderBox.Instance.RemoveBlockingObject(this);
	}
}
