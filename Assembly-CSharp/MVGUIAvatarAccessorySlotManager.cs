using System;
using System.Linq;
using MV.Common;
using UnityEngine;

public class MVGUIAvatarAccessorySlotManager : MonoBehaviour
{
	private UXGroup Group => ((Component)this).GetComponent<UXGroup>();

	private MVGUIAvatarAccessorySlot[] AccessorySlots => ((Component)this).GetComponentsInChildren<MVGUIAvatarAccessorySlot>();

	public void ShowSlots()
	{
		Group.Show();
		RefreshAvatarBodySlots();
	}

	public void HideSlots()
	{
		Group.Hide();
	}

	public void Initialize()
	{
		StreamingAssetInventory streamingAssetInventory = MVGameController.Instance.Game.StreamingAssetInventory;
		streamingAssetInventory.OnProductInventoryChange = (ProductInventory<StreamingAssetInfo>.OnProductInventoryChangeDelegate)Delegate.Combine(streamingAssetInventory.OnProductInventoryChange, (ProductInventory<StreamingAssetInfo>.OnProductInventoryChangeDelegate)((ProductInventory<StreamingAssetInfo> inv) =>
		{
			RefreshAvatarBodySlots();
		}));
	}

	public void RefreshAvatarBodySlots()
	{
		MVGUIAvatarAccessorySlot[] accessorySlots = AccessorySlots;
		foreach (MVGUIAvatarAccessorySlot mVGUIAvatarAccessorySlot in accessorySlots)
		{
			mVGUIAvatarAccessorySlot.RefreshEquippedState();
		}
	}

	public void HighlightSlots(AvatarAccessorySlot[] slots)
	{
		MVGUIAvatarAccessorySlot[] accessorySlots = AccessorySlots;
		foreach (MVGUIAvatarAccessorySlot mVGUIAvatarAccessorySlot in accessorySlots)
		{
			bool validSlot = Enumerable.Contains(slots, mVGUIAvatarAccessorySlot.avatarAccessorySlot);
			mVGUIAvatarAccessorySlot.HightlightSlot(validSlot);
		}
	}

	public void StopHightlightSlots()
	{
		MVGUIAvatarAccessorySlot[] accessorySlots = AccessorySlots;
		foreach (MVGUIAvatarAccessorySlot mVGUIAvatarAccessorySlot in accessorySlots)
		{
			mVGUIAvatarAccessorySlot.StopSlotHighlight();
		}
	}

	public void SetDragItem(AvatarAccessoryInventoryViewItem dragItem)
	{
		MVGUIAvatarAccessorySlot[] accessorySlots = AccessorySlots;
		foreach (MVGUIAvatarAccessorySlot mVGUIAvatarAccessorySlot in accessorySlots)
		{
			mVGUIAvatarAccessorySlot.SetDragItem(dragItem);
		}
	}

	public void StopDragAvatarAccessory()
	{
		StopHightlightSlots();
		MVGUIAvatarAccessorySlot[] accessorySlots = AccessorySlots;
		foreach (MVGUIAvatarAccessorySlot mVGUIAvatarAccessorySlot in accessorySlots)
		{
			mVGUIAvatarAccessorySlot.SetStopDragItem();
			mVGUIAvatarAccessorySlot.SetDragItem(null);
		}
	}
}
