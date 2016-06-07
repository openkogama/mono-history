using System;
using MV.Common;
using UnityEngine;
using UnityEngine.EventSystems;
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
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IModalPopupCreator x, BaseEventData y) =>
		{
			x.Create();
		});
		int accessoryID = AvatarBody.GetAccessoryID(avatarAccessorySlot);
		MVGameControllerBase.OperationRequests.SetAvatarAccessorySlot(AvatarBody.Id, accessoryID, AvatarAccessorySlot.Undefined, 0f);
		MVNetworkGame game = MVGameControllerBase.Game;
		game.OnSetAvatarAccessoryResponse = (Action<bool>)Delegate.Combine(game.OnSetAvatarAccessoryResponse, new Action<bool>(OnUnequipPop));
	}

	private void OnUnequipPop(bool setSlotSuccess)
	{
		MVNetworkGame game = MVGameControllerBase.Game;
		game.OnSetAvatarAccessoryResponse = (Action<bool>)Delegate.Remove(game.OnSetAvatarAccessoryResponse, new Action<bool>(OnUnequipPop));
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
		{
			x.Pop();
		});
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
