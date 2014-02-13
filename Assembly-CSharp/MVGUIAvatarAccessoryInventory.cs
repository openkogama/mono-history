using System;
using System.Collections.Generic;
using UnityEngine;

public class MVGUIAvatarAccessoryInventory : MVGUIAvatarAccessoryBasicView
{
	public List<MVGUIAvatarAccessoryInventoryGroup> avatarAccessoryGroups;

	public MVGUIAvatarAccessorySlotManager avatarAccessorySlots;

	public UXGroup inventoryUpdatedGroup;

	public override void OnShow()
	{
		base.OnShow();
		avatarAccessorySlots.ShowSlots();
		if (inventoryUpdatedGroup.Visible)
		{
			HideInventoryUpdated();
		}
	}

	public override void OnHide()
	{
		base.OnHide();
		avatarAccessorySlots.HideSlots();
	}

	protected override void DoInitialize()
	{
		foreach (MVGUIAvatarAccessoryInventoryGroup avatarAccessoryGroup in avatarAccessoryGroups)
		{
			avatarAccessoryGroup.Initialize();
			avatarAccessoryGroup.OnViewItemDragStart = (MVGUIAvatarAccessoryInventoryGroup.OnViewItemActionDelegate)Delegate.Combine(avatarAccessoryGroup.OnViewItemDragStart, new MVGUIAvatarAccessoryInventoryGroup.OnViewItemActionDelegate(OnStartDragAvatarAccessory));
			avatarAccessoryGroup.OnViewItemDragEnd = (MVGUIAvatarAccessoryInventoryGroup.OnViewItemActionDelegate)Delegate.Combine(avatarAccessoryGroup.OnViewItemDragEnd, new MVGUIAvatarAccessoryInventoryGroup.OnViewItemActionDelegate(OnStopDragAvatarAccessory));
			avatarAccessoryGroup.OnViewItemMouseOverEnter = (MVGUIAvatarAccessoryInventoryGroup.OnViewItemActionDelegate)Delegate.Combine(avatarAccessoryGroup.OnViewItemMouseOverEnter, new MVGUIAvatarAccessoryInventoryGroup.OnViewItemActionDelegate(OnViewItemMouseOverEnter));
			avatarAccessoryGroup.OnViewItemMouseOverExit = (MVGUIAvatarAccessoryInventoryGroup.OnViewItemActionDelegate)Delegate.Combine(avatarAccessoryGroup.OnViewItemMouseOverExit, new MVGUIAvatarAccessoryInventoryGroup.OnViewItemActionDelegate(OnViewItemMouseOverExit));
		}
		avatarAccessorySlots.Initialize();
	}

	public void ShowInventoryUpdated()
	{
		if (!View.isVisible)
		{
			inventoryUpdatedGroup.SetVisible(visible: true);
		}
	}

	public void HideInventoryUpdated()
	{
		inventoryUpdatedGroup.SetVisible(visible: false);
	}

	private void OnStartDragAvatarAccessory(AvatarAccessoryInventoryViewItem viewItem)
	{
		avatarAccessorySlots.HighlightSlots(viewItem.AvatarAccessory.ValidSlots);
		avatarAccessorySlots.SetDragItem(viewItem);
	}

	private void OnStopDragAvatarAccessory(AvatarAccessoryInventoryViewItem viewItem)
	{
		avatarAccessorySlots.StopHightlightSlots();
		avatarAccessorySlots.StopDragAvatarAccessory();
	}

	private void OnViewItemMouseOverEnter(AvatarAccessoryInventoryViewItem viewItem)
	{
		if ((Object)(object)viewItem != (Object)null && (Object)(object)viewItem.AvatarAccessory != (Object)null)
		{
			avatarAccessorySlots.HighlightSlots(viewItem.AvatarAccessory.ValidSlots);
		}
		else
		{
			avatarAccessorySlots.StopHightlightSlots();
		}
	}

	private void OnViewItemMouseOverExit(AvatarAccessoryInventoryViewItem viewItem)
	{
		avatarAccessorySlots.StopHightlightSlots();
	}
}
