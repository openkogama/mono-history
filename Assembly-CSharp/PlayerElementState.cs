using System;
using MV.Common;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PlayerElementState : MonoBehaviour
{
	private bool showAcceptButton;

	private bool showRequestButton;

	private bool showCancelButton;

	private bool showPendingButton;

	private bool showLocalPlayerImage;

	[SerializeField]
	private Button requestFriendship;

	[SerializeField]
	private Button pendingFriendship;

	[SerializeField]
	private Button acceptFriendRequest;

	[SerializeField]
	private Button cancel;

	[SerializeField]
	private Image localPlayerImage;

	public void Initialize(MVPlayer player, Friend friend)
	{
		Debug.LogWarning("Remember to hook up errors to console");
		SetupButtons(player, friend);
		SetButtonVisibility(player, friend);
	}

	private void SetButtonVisibility(MVPlayer player, Friend friend)
	{
		if (player.IsAnonymous || MVGameControllerBase.Game.LocalPlayer.IsAnonymous)
		{
			return;
		}
		if (friend == null && player != MVGameControllerBase.Game.LocalPlayer)
		{
			showRequestButton = true;
		}
		else if (friend != null && friend.status == FriendStatus.Pending)
		{
			if (MVGameControllerBase.Game.Friends.Friends.ContainsValue(friend))
			{
				showPendingButton = true;
			}
			else
			{
				showAcceptButton = true;
				showCancelButton = true;
			}
		}
		if (player.ProfileID == MVGameControllerBase.Game.LocalPlayer.ProfileID)
		{
			showLocalPlayerImage = true;
		}
		requestFriendship.gameObject.SetActive(showRequestButton);
		pendingFriendship.gameObject.SetActive(showPendingButton);
		acceptFriendRequest.gameObject.SetActive(showAcceptButton);
		cancel.gameObject.SetActive(showCancelButton);
		localPlayerImage.gameObject.SetActive(showLocalPlayerImage);
	}

	private void SetupButtons(MVPlayer player, Friend friend)
	{
		requestFriendship.onClick.AddListener(() =>
		{
			try
			{
				Debug.Log("Request friendship");
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
