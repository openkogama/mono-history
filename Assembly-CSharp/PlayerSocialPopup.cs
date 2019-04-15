using System;
using MV.Common;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.UI;

public class PlayerSocialPopup : MonoBehaviour
{
	[SerializeField]
	private Text playerName;

	[SerializeField]
	private GameObject subscriberFrame;

	[SerializeField]
	private GameObject viewProfileButton;

	[SerializeField]
	private GameObject addFriendButton;

	[SerializeField]
	private GameObject pendingFriendship;

	[SerializeField]
	private GameObject cancel;

	[SerializeField]
	private GameObject acceptFriendRequest;

	[SerializeField]
	private GameObject friendButtonsGameObject;

	[SerializeField]
	private GameObject isFriendTextObject;

	private int profileId;

	private Friend friend;

	public void Initialize(int remotePlayerProfileId, string name, bool isSubscriber)
	{
		profileId = remotePlayerProfileId;
		playerName.text = name;
		subscriberFrame.SetActive(isSubscriber);
		Color color = Styles.GetColor(ColorStyle.Gray);
		if (isSubscriber)
		{
			color = Styles.GetColor(ColorStyle.OffWhite);
		}
		playerName.color = color;
		if (!MVGameControllerBase.IsTouristSession)
		{
			SetupFriendButtons();
			FriendList friends = MVGameControllerBase.Game.Friends;
			friends.OnFriendRequestReceived = (UnityAction)Delegate.Combine(friends.OnFriendRequestReceived, new UnityAction(FriendRequestReceived));
			FriendList friends2 = MVGameControllerBase.Game.Friends;
			friends2.OnPendingCountChanged = (UnityAction<int>)Delegate.Combine(friends2.OnPendingCountChanged, new UnityAction<int>(PendingCountChanged));
			FriendList friends3 = MVGameControllerBase.Game.Friends;
			friends3.OnFriendListUpdated = (FriendList.OnFriendListUpdatedDelegate)Delegate.Combine(friends3.OnFriendListUpdated, new FriendList.OnFriendListUpdatedDelegate(SetupFriendButtons));
			FriendList friends4 = MVGameControllerBase.Game.Friends;
			friends4.OnFriendRequestAccepted = (FriendList.OnFriendRequestUpdated)Delegate.Combine(friends4.OnFriendRequestAccepted, new FriendList.OnFriendRequestUpdated(OnFriendRequestAccepted));
		}
		else
		{
			friendButtonsGameObject.SetActive(value: false);
		}
	}

	public void OnFriendRequestAccepted(Friend friend)
	{
		string error = string.Empty;
		if (ValidateFriendRequest(ref error))
		{
			MVGameControllerBase.OperationRequests.RequestAcceptFriendShip(friend.friendID);
		}
		else
		{
			PostErrorPopup(error);
		}
	}

	public void OnFriendRequestCanceled()
	{
		Friend friendByProfileID = MVGameControllerBase.Game.Friends.GetFriendByProfileID(profileId);
		MVGameControllerBase.OperationRequests.RequestRejectFriendShip(friendByProfileID.friendID);
		SetupFriendButtons();
	}

	public void OnAddFriendClicked()
	{
		string error = string.Empty;
		if (!ValidateFriendRequest(ref error) || !MVGameControllerBase.OperationRequests.RequestFriendShipByID(profileId, ref error))
		{
			PostErrorPopup(error);
		}
		else
		{
			SetupFriendButtons();
		}
	}

	public void OnViewProfileClicked()
	{
		Debug.Log("Goto profile of player: " + profileId);
		BrowserComm.ToJavaScript.ExternalCall("gotoPlayerProfile", profileId);
	}

	private void FriendRequestReceived()
	{
		SetupFriendButtons();
	}

	private void PendingCountChanged(int count)
	{
		SetupFriendButtons();
	}

	private void OnDestroy()
	{
		FriendList friends = MVGameControllerBase.Game.Friends;
		friends.OnFriendRequestReceived = (UnityAction)Delegate.Remove(friends.OnFriendRequestReceived, new UnityAction(FriendRequestReceived));
		FriendList friends2 = MVGameControllerBase.Game.Friends;
		friends2.OnPendingCountChanged = (UnityAction<int>)Delegate.Remove(friends2.OnPendingCountChanged, new UnityAction<int>(PendingCountChanged));
		FriendList friends3 = MVGameControllerBase.Game.Friends;
		friends3.OnFriendListUpdated = (FriendList.OnFriendListUpdatedDelegate)Delegate.Remove(friends3.OnFriendListUpdated, new FriendList.OnFriendListUpdatedDelegate(SetupFriendButtons));
		FriendList friends4 = MVGameControllerBase.Game.Friends;
		friends4.OnFriendRequestAccepted = (FriendList.OnFriendRequestUpdated)Delegate.Remove(friends4.OnFriendRequestAccepted, new FriendList.OnFriendRequestUpdated(OnFriendRequestAccepted));
	}

	private void SetupFriendButtons()
	{
		pendingFriendship.gameObject.SetActive(value: false);
		cancel.gameObject.SetActive(value: false);
		acceptFriendRequest.gameObject.SetActive(value: false);
		friend = MVGameControllerBase.Game.Friends.GetFriendByProfileID(profileId);
		bool flag = friend != null;
		addFriendButton.SetActive(!flag);
		if (flag)
		{
			if (friend.status == FriendStatus.Pending)
			{
				bool flag2 = MVGameControllerBase.Game.Friends.Friends.ContainsValue(friend);
				pendingFriendship.gameObject.SetActive(flag2);
				cancel.gameObject.SetActive(!flag2);
				acceptFriendRequest.gameObject.SetActive(!flag2);
			}
			bool flag3 = friend.status == FriendStatus.Accepted;
			if (flag3)
			{
				playerName.color = Styles.GetColor(ColorStyle.FriendGreen);
			}
			friendButtonsGameObject.SetActive(!flag3);
			isFriendTextObject.SetActive(flag3);
		}
	}

	private void PostErrorPopup(string error)
	{
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IModalPopupCreator x, BaseEventData y) =>
		{
			x.Create(error, "Error: ");
		});
	}

	private bool ValidateFriendRequest(ref string error)
	{
		int level = MVGameControllerBase.Game.LocalPlayer.Level;
		int friendsLimit = BadgeManager.GetFriendsLimit(level);
		int count = MVGameControllerBase.Game.Friends.Friends.Count;
		if (count < friendsLimit)
		{
			return true;
		}
		int num = level + 1;
		int friendsLimit2 = BadgeManager.GetFriendsLimit(num);
		string format = TM._("You can only have {0} friends at level {1}. Get to level {2} and you can have {3} friends.");
		format = string.Format(format, friendsLimit, level, num, friendsLimit2);
		MVGameControllerBase.PostGameMsg(MVGameMsgType.AdminMsg, TM._("Your friendlist is full"));
		return false;
	}
}
