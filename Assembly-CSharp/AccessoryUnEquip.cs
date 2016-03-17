using System;
using MV.Common;
using UnityEngine;
using UnityEngine.Events;

public class AccessoryUnEquip : MonoBehaviour
{
	private AvatarAccessorySlot avatarAccessorySlot;

	public UnityAction OnUnequipFinished;

	private MVBody AvatarBody;

	public void Initialize(AvatarAccessorySlot avatarAccessorySlot, MVBody body)
	{
		AvatarBody = body;
		this.avatarAccessorySlot = avatarAccessorySlot;
		MVNetworkGame game = MVGameControllerBase.Game;
		game.OnSetAvatarAccessoryResponse = (Action<bool>)Delegate.Combine(game.OnSetAvatarAccessoryResponse, new Action<bool>(Game_OnSetAvatarAccessorySlotResponseUnequipHandler));
	}

	public void UnEquip()
	{
		int accessoryID = AvatarBody.GetAccessoryID(avatarAccessorySlot);
		MVGameControllerBase.Game.SetAvatarAccessorySlot(AvatarBody.Id, accessoryID, AvatarAccessorySlot.Undefined, 0f);
	}

	private void OnDestroy()
	{
		OnUnequipFinished = null;
		MVNetworkGame game = MVGameControllerBase.Game;
		game.OnSetAvatarAccessoryResponse = (Action<bool>)Delegate.Remove(game.OnSetAvatarAccessoryResponse, new Action<bool>(Game_OnSetAvatarAccessorySlotResponseUnequipHandler));
	}

	private void Game_OnSetAvatarAccessorySlotResponseUnequipHandler(bool setSlotSuccess)
	{
		Debug.Log("UnEquip result: " + setSlotSuccess);
		MVNetworkGame game = MVGameControllerBase.Game;
		game.OnSetAvatarAccessoryResponse = (Action<bool>)Delegate.Remove(game.OnSetAvatarAccessoryResponse, new Action<bool>(Game_OnSetAvatarAccessorySlotResponseUnequipHandler));
		if (OnUnequipFinished != null)
		{
			OnUnequipFinished();
		}
	}
}
