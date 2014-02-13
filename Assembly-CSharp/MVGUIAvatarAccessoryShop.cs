using System;
using System.Collections.Generic;
using MV.Common;
using UnityEngine;

public class MVGUIAvatarAccessoryShop : MVGUIAvatarAccessoryBasicView
{
	public List<MVGUIAvatarAccessoryShopGroup> avatarAccessoryGroups;

	private StreamingAssetInfo waitingToBeAttached;

	private AvatarAccessoryShopViewItem previewViewItem;

	private StreamingAssetInfo previewStreamingAssetInfo;

	private AvatarAccessory previewAvatarAccessory;

	public override void OnHide()
	{
		base.OnHide();
		if (AvatarBody != null)
		{
			ResetAttachedAvatarItem();
		}
	}

	protected override void DoInitialize()
	{
		foreach (MVGUIAvatarAccessoryShopGroup avatarAccessoryGroup in avatarAccessoryGroups)
		{
			avatarAccessoryGroup.Initialize();
			avatarAccessoryGroup.OnItemPreview = (MVGUIAvatarAccessoryShopGroup.OnItemPreviewDelegate)Delegate.Combine(avatarAccessoryGroup.OnItemPreview, new MVGUIAvatarAccessoryShopGroup.OnItemPreviewDelegate(PreviewAvatarItem));
		}
	}

	private void ResetAttachedAvatarItem()
	{
		if ((Object)(object)previewAvatarAccessory != (Object)null)
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
		if ((Object)(object)viewItem == (Object)null)
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
			Object.Destroy((Object)(object)((Component)avatarAccessory).gameObject);
			return;
		}
		waitingToBeAttached = null;
		ResetAttachedAvatarItem();
		AvatarAccessorySlot defaultSlot = avatarAccessory.DefaultSlot;
		foreach (AvatarAccessory accessory in AvatarBody.GetAccessories(defaultSlot))
		{
			accessory.Visible = false;
		}
		AvatarBody.AttachAccessory(avatarAccessory, defaultSlot, avatarAccessory.DefaultOffset);
		previewViewItem = viewItem;
		previewStreamingAssetInfo = streamingAssetInfo;
		previewAvatarAccessory = avatarAccessory;
	}

	private void DetachAndDestroyPreviewItem()
	{
		AvatarBody.DetachAccessory(previewAvatarAccessory);
		Object.Destroy((Object)(object)((Component)previewAvatarAccessory).gameObject);
		previewViewItem.previewButton.SetToggleState(toggle: false);
		previewStreamingAssetInfo = null;
		previewAvatarAccessory = null;
		previewViewItem = null;
	}
}
