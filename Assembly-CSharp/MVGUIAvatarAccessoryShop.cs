using System;
using System.Collections.Generic;
using MV.Common;
using UnityEngine;

public class MVGUIAvatarAccessoryShop : MVGUIAvatarAccessoryBasicView
{
	private bool fromShown;

	public List<MVGUIAvatarAccessoryShopGroup> avatarAccessoryGroups;

	[SerializeField]
	private AvatarAccessoryController avatarAccessoryController;

	private StreamingAssetInfo waitingToBeAttached;

	private AvatarAccessoryShopViewItem previewViewItem;

	private StreamingAssetInfo previewStreamingAssetInfo;

	private AvatarAccessory previewAvatarAccessory;

	public override void OnHide()
	{
		base.OnHide();
		if (AvatarBody != null && fromShown)
		{
			ResetAttachedAvatarItem();
		}
		fromShown = false;
	}

	public override void OnShow()
	{
		base.OnShow();
		fromShown = true;
	}

	protected override void DoInitialize()
	{
		foreach (MVGUIAvatarAccessoryShopGroup avatarAccessoryGroup in avatarAccessoryGroups)
		{
			avatarAccessoryGroup.Initialize(avatarAccessoryController);
			avatarAccessoryGroup.OnItemPreview = (MVGUIAvatarAccessoryShopGroup.OnItemPreviewDelegate)Delegate.Combine(avatarAccessoryGroup.OnItemPreview, new MVGUIAvatarAccessoryShopGroup.OnItemPreviewDelegate(PreviewAvatarItem));
		}
	}

	private void ResetAttachedAvatarItem()
	{
		if (previewAvatarAccessory != null)
		{
			DetachAndDestroyPreviewItem();
		}
		foreach (AvatarAccessory accessory in AvatarBody.GetAccessories())
		{
			accessory.Visible = true;
		}
	}

	private void PreviewAvatarItem(AvatarAccessoryShopViewItem viewItem)
	{
		if (viewItem == null)
		{
			ResetAttachedAvatarItem();
			return;
		}
		StreamingAssetInfo streamingAssetInfo = (StreamingAssetInfo)viewItem.Item.Object;
		if (previewStreamingAssetInfo != streamingAssetInfo)
		{
			waitingToBeAttached = streamingAssetInfo;
			AvatarAccessory.Create(streamingAssetInfo, (AvatarAccessory avatarAccessory) =>
			{
				OnPreviewAvatarAccessoryLoaded(viewItem, avatarAccessory);
			});
		}
	}

	private void OnPreviewAvatarAccessoryLoaded(AvatarAccessoryShopViewItem viewItem, AvatarAccessory avatarAccessory)
	{
		StreamingAssetInfo streamingAssetInfo = (StreamingAssetInfo)viewItem.Item.Object;
		if (waitingToBeAttached != streamingAssetInfo)
		{
			UnityEngine.Object.Destroy(avatarAccessory.gameObject);
			return;
		}
		waitingToBeAttached = null;
		ResetAttachedAvatarItem();
		AvatarAccessorySlot defaultSlot = avatarAccessory.AccessorySettings.DefaultSlot;
		foreach (AvatarAccessory accessory in AvatarBody.GetAccessories(defaultSlot))
		{
			accessory.Visible = false;
		}
		AvatarBody.AttachAccessory(avatarAccessory, defaultSlot, avatarAccessory.AccessorySettings.DefaultOffset);
		previewViewItem = viewItem;
		previewStreamingAssetInfo = streamingAssetInfo;
		previewAvatarAccessory = avatarAccessory;
	}

	private void DetachAndDestroyPreviewItem()
	{
		AvatarBody.DetachAccessory(previewAvatarAccessory);
		UnityEngine.Object.Destroy(previewAvatarAccessory.gameObject);
		previewViewItem.previewButton.SetToggleState(toggle: false);
		previewStreamingAssetInfo = null;
		previewAvatarAccessory = null;
		previewViewItem = null;
	}
}
