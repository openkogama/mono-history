using System;
using MV.Common;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PlayerElementState : MonoBehaviour
{
	[SerializeField]
	private Button requestFriendship;

	[SerializeField]
	private Button pendingFriendship;

	[SerializeField]
	private Button acceptFriendRequest;

	[SerializeField]
	private Button cancel;

	[SerializeField]
	private Button manageUserButton;

	[SerializeField]
	private AdminToolController adminToolsPrefab;

	[SerializeField]
	private OwnerToolController ownerToolsPrefab;

	[SerializeField]
	private Text playerName;

	public void Initialize(MVPlayer player, Friend friend)
	{
		SetupButtons(player, friend);
		SetButtonVisibility(player, friend);
	}

	public void OpenUserManagement()
	{
		MVLocalPlayer localPlayer = MVGameControllerBase.Game.LocalPlayer;
		if (localPlayer.ProfileID > 0 && localPlayer.IsAdmin)
		{
			AdminToolController adminTools = UnityEngine.Object.Instantiate(adminToolsPrefab);
			adminTools.Initialize(playerName.text);
			ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
			{
				x.Push(adminTools.gameObject, UIPushOption.Blocking, null, UIGroupFlags.GameObjectUI);
			});
		}
		else if (MVGameControllerBase.GameMode == MVGameMode.Edit && localPlayer.PlanetOwnership == MVLocalPlayer.PlanetOwnershipType.Owner)
		{
			OwnerToolController ownerTools = UnityEngine.Object.Instantiate(ownerToolsPrefab);
			ownerTools.Initialize(playerName.text);
			ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
			{
				x.Push(ownerTools.gameObject, UIPushOption.Blocking, null, UIGroupFlags.GameObjectUI);
			});
		}
	}

	private void SetButtonVisibility(MVPlayer player, Friend friend)
	{
		pendingFriendship.gameObject.SetActive(value: false);
		cancel.gameObject.SetActive(value: false);
		acceptFriendRequest.gameObject.SetActive(value: false);
		requestFriendship.gameObject.SetActive(value: false);
		manageUserButton.gameObject.SetActive(value: false);
		MVLocalPlayer localPlayer = MVGameControllerBase.Game.LocalPlayer;
		bool flag = player.Avatar == localPlayer.Avatar;
		bool flag2 = MVGameControllerBase.GameMode == MVGameMode.Edit && localPlayer.PlanetOwnership == MVLocalPlayer.PlanetOwnershipType.Owner;
		manageUserButton.gameObject.SetActive(!localPlayer.IsTourist && (localPlayer.IsAdmin || flag2) && !flag);
		if (!player.IsTourist && !localPlayer.IsTourist && !flag)
		{
			requestFriendship.gameObject.SetActive(friend == null);
			if (friend != null && friend.status == FriendStatus.Pending)
			{
				bool flag3 = MVGameControllerBase.Game.Friends.Friends.ContainsValue(friend);
				pendingFriendship.gameObject.SetActive(flag3);
				cancel.gameObject.SetActive(!flag3);
				acceptFriendRequest.gameObject.SetActive(!flag3);
			}
		}
	}

	private void SetupButtons(MVPlayer player, Friend friend)
	{
		requestFriendship.onClick.AddListener(() =>
		{
			try
			{
				ValidateFriendRequest();
				string errorText = string.Empty;
				if (!MVGameControllerBase.OperationRequests.RequestFriendShipByID(player.ProfileID, ref errorText))
				{
					ExecuteEvents.ExecuteHierarchy(gameObject, null, (IModalPopupCreator x, BaseEventData y) =>
					{
						x.Create(errorText, "Error: ");
					});
				}
			}
			catch (Exception ex)
			{
				Debug.Log(ex.Message);
				Debug.LogWarning("Remember to hook up errors to console");
			}
		});
		acceptFriendRequest.onClick.AddListener(() =>
		{
			try
			{
				ValidateFriendRequest();
				MVGameControllerBase.OperationRequests.RequestAcceptFriendShip(friend.friendID);
			}
			catch (Exception ex)
			{
				Debug.Log(ex.Message);
				Debug.LogWarning("Remember to hook up errors to console");
			}
		});
		cancel.onClick.AddListener(() =>
		{
			MVGameControllerBase.OperationRequests.RequestRejectFriendShip(friend.friendID);
		});
		pendingFriendship.onClick.AddListener(() =>
		{
			MVGameControllerBase.OperationRequests.RequestRejectFriendShip(friend.friendID);
		});
	}

	private void ValidateFriendRequest()
	{
		int level = MVGameControllerBase.Game.LocalPlayer.Level;
		int friendsLimit = BadgeManager.GetFriendsLimit(level);
		int count = MVGameControllerBase.Game.Friends.Friends.Count;
		if (count < friendsLimit)
		{
			return;
		}
		int num = level + 1;
		int friendsLimit2 = BadgeManager.GetFriendsLimit(num);
		string format = TM._("You can only have {0} friends at level {1}. Get to level {2} and you can have {3} friends.");
		format = string.Format(format, friendsLimit, level, num, friendsLimit2);
		MVGameControllerBase.PostGameMsg(MVGameMsgType.AdminMsg, TM._("Your friendlist is full"));
		throw new Exception(format);
	}
}
