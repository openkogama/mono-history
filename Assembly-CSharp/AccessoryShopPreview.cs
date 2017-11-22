using Assets.Scripts.WorldObjectTypes.Avatar.Accessories;
using MV.Common;
using UnityEngine;
using UnityEngine.EventSystems;

public class AccessoryShopPreview : MonoBehaviour, IPreviewAccessoryShopItem, IEventSystemHandler
{
	private StreamingAssetInfo waitingToBeAttached;

	private StreamingAssetInfo previewStreamingAssetInfo;

	private AvatarAccessory previewAvatarAccessory;

	private AccessoryLoader accessoryLoader = new AccessoryLoader();

	protected MVBody AvatarBody;

	private void SetCurrentBody(MVBody body)
	{
		AvatarBody = body;
	}

	public void PreviewAccessoryShopItem(StreamingAssetInfo viewItem)
	{
		if (MVGameControllerBase.GameMode == MVGameMode.CharacterEditor)
		{
			ExecuteEvents.ExecuteHierarchy(gameObject, null, (IGetCurrentBody x, BaseEventData y) =>
			{
				x.GetCurrentBody(SetCurrentBody);
			});
		}
		else
		{
			AvatarBody = MVGameControllerBase.Game.LocalPlayer.Avatar.Body;
		}
		if (viewItem == null)
		{
			ResetAttachedAvatarItem();
			return;
		}
		StreamingAssetInfo streamingAssetInfo = viewItem;
		if (previewStreamingAssetInfo == streamingAssetInfo)
		{
			ResetAttachedAvatarItem();
			return;
		}
		waitingToBeAttached = streamingAssetInfo;
		accessoryLoader.LoadAccessory(streamingAssetInfo, (AvatarAccessory avatarAccessory) =>
		{
			OnPreviewAvatarAccessoryLoaded(viewItem, avatarAccessory);
		});
	}

	private void OnDestroy()
	{
		accessoryLoader.Destroy();
		accessoryLoader = null;
		ResetAttachedAvatarItem();
	}

	private void DetachAndDestroyPreviewItem()
	{
		AvatarBody.DetachAccessory(previewAvatarAccessory);
		Object.Destroy(previewAvatarAccessory.gameObject);
		previewStreamingAssetInfo = null;
		previewAvatarAccessory = null;
	}

	private void ResetAttachedAvatarItem()
	{
		if (AvatarBody == null)
		{
			if (MVGameControllerBase.GameMode == MVGameMode.CharacterEditor)
			{
				ExecuteEvents.ExecuteHierarchy(gameObject, null, (IGetCurrentBody x, BaseEventData y) =>
				{
					x.GetCurrentBody(SetCurrentBody);
				});
			}
			else
			{
				AvatarBody = MVGameControllerBase.Game.LocalPlayer.Avatar.Body;
			}
		}
		if (previewAvatarAccessory != null)
		{
			DetachAndDestroyPreviewItem();
		}
		foreach (AvatarAccessory accessory in AvatarBody.GetAccessories())
		{
			accessory.Visible = true;
		}
	}

	private void OnPreviewAvatarAccessoryLoaded(StreamingAssetInfo viewItem, AvatarAccessory avatarAccessory)
	{
		if (waitingToBeAttached != viewItem)
		{
			Object.Destroy(avatarAccessory.gameObject);
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
		previewStreamingAssetInfo = viewItem;
		previewAvatarAccessory = avatarAccessory;
	}
}
