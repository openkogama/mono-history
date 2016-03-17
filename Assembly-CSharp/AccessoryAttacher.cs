using System;
using System.Linq;
using MV.Common;
using UnityEngine;

public class AccessoryAttacher
{
	private int purchasedInventoryID;

	private ConfirmationPopup attachConfirmationPopup;

	private AvatarAccessory accessoryToBeEquipped;

	private MVBody avatarBody;

	private Action OnFinishedCallback;

	public void AttachAccessory(int purchasedInventoryID, MVBody currentBody, Action OnFinishedCallback)
	{
		this.OnFinishedCallback = OnFinishedCallback;
		avatarBody = currentBody;
		this.purchasedInventoryID = purchasedInventoryID;
		ProductInventoryInfo invInfo = MVGameControllerBase.Game.StreamingAssetInventory.Get(purchasedInventoryID);
		AvatarAccessory.Create(invInfo, AvatarAccessoryCreateHandler);
	}

	private void AvatarAccessoryCreateHandler(AvatarAccessory avatarAccessory)
	{
		accessoryToBeEquipped = avatarAccessory;
		AvatarAccessory avatarAccessory2 = avatarBody.GetAccessories(avatarAccessory.AccessorySettings.DefaultSlot).FirstOrDefault();
		if (avatarAccessory2 != null)
		{
			Unequip(avatarAccessory2);
		}
		else
		{
			Equip();
		}
	}

	private void Unequip(AvatarAccessory avatarAccessory)
	{
		MVGameControllerBase.Game.SetAvatarAccessorySlot(avatarBody.Id, avatarAccessory.InventoryID, AvatarAccessorySlot.Undefined, 0f);
		MVNetworkGame game = MVGameControllerBase.Game;
		game.OnSetAvatarAccessoryResponse = (Action<bool>)Delegate.Combine(game.OnSetAvatarAccessoryResponse, new Action<bool>(Game_OnSetAvatarAccessorySlotResponseUnequipHandler));
	}

	private void Game_OnSetAvatarAccessorySlotResponseUnequipHandler(bool setSlotSuccess)
	{
		MVNetworkGame game = MVGameControllerBase.Game;
		game.OnSetAvatarAccessoryResponse = (Action<bool>)Delegate.Remove(game.OnSetAvatarAccessoryResponse, new Action<bool>(Game_OnSetAvatarAccessorySlotResponseUnequipHandler));
		if (setSlotSuccess)
		{
			Equip();
			return;
		}
		UnityEngine.Object.Destroy(accessoryToBeEquipped.gameObject);
		accessoryToBeEquipped = null;
	}

	private void Equip()
	{
		AvatarAccessory avatarAccessory = accessoryToBeEquipped;
		MVNetworkGame game = MVGameControllerBase.Game;
		game.OnSetAvatarAccessoryResponse = (Action<bool>)Delegate.Combine(game.OnSetAvatarAccessoryResponse, new Action<bool>(Game_OnSetAvatarAccessorySlotResponseEquipHandler));
		MVGameControllerBase.Game.SetAvatarAccessorySlot(avatarBody.Id, avatarAccessory.InventoryID, avatarAccessory.AccessorySettings.DefaultSlot, avatarAccessory.AccessorySettings.DefaultOffset);
		avatarBody.AttachAccessory(avatarAccessory, accessoryToBeEquipped.AccessorySettings.DefaultSlot, accessoryToBeEquipped.AccessorySettings.DefaultOffset);
	}

	private void Game_OnSetAvatarAccessorySlotResponseEquipHandler(bool setSlotSuccess)
	{
		MVNetworkGame game = MVGameControllerBase.Game;
		game.OnSetAvatarAccessoryResponse = (Action<bool>)Delegate.Remove(game.OnSetAvatarAccessoryResponse, new Action<bool>(Game_OnSetAvatarAccessorySlotResponseEquipHandler));
		if (!setSlotSuccess)
		{
			avatarBody.DestroyAccessory(purchasedInventoryID);
		}
		accessoryToBeEquipped = null;
		purchasedInventoryID = -1;
		OnFinishedCallback();
	}
}
