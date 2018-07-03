using System;
using UnityEngine;

public class AccessoryAttacher
{
	private ConfirmationPopup attachConfirmationPopup;

	private MVBody avatarBody;

	private Action OnFinishedCallback;

	public void AttachAccessory(int streamingAssetsId, MVBody currentBody, float offset, float scale, Action OnFinishedCallback)
	{
		Debug.LogWarning("Don't understand ordering of this. Would expect server call first the accessory loading second.");
		Debug.LogWarning("Reimplement: AttachAccessory");
		this.OnFinishedCallback = OnFinishedCallback;
		avatarBody = currentBody;
		MVNetworkGame game = MVGameControllerBase.Game;
		game.OnSetAvatarAccessoryResponse = (Action<bool>)Delegate.Combine(game.OnSetAvatarAccessoryResponse, new Action<bool>(Game_OnSetAvatarAccessorySlotResponseEquipHandler));
		MVGameControllerBase.OperationRequests.SetAvatarAccessorySlot(avatarBody.Id, streamingAssetsId, offset, scale);
	}

	private void Game_OnSetAvatarAccessorySlotResponseEquipHandler(bool setSlotSuccess)
	{
		MVNetworkGame game = MVGameControllerBase.Game;
		game.OnSetAvatarAccessoryResponse = (Action<bool>)Delegate.Remove(game.OnSetAvatarAccessoryResponse, new Action<bool>(Game_OnSetAvatarAccessorySlotResponseEquipHandler));
		OnFinishedCallback();
	}
}
