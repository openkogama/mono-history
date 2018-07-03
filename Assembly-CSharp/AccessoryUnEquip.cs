using System;
using MV.Common;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class AccessoryUnEquip : MonoBehaviour
{
	private AccessorySlotType avatarAccessorySlot;

	public UnityAction OnUnequipFinished;

	private MVBody AvatarBody;

	public void Initialize(AccessorySlotType avatarAccessorySlot, MVBody body)
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
		MVGameControllerBase.OperationRequests.UnEquipAccessory(AvatarBody.Id, avatarAccessorySlot);
		Debug.LogWarning("Implement callback. Investigate if something smart is possible.");
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
