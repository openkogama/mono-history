using System;
using MV.Common;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GamePassesHighScoreElement : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IEventSystemHandler
{
	[SerializeField]
	private Text userRankText;

	[SerializeField]
	private Text userNameText;

	[SerializeField]
	private Text userAmountOfGamePointText;

	[SerializeField]
	private Image backgroundImage;

	[SerializeField]
	private GameObject topBorderGameObject;

	[SerializeField]
	private GameObject subscriberUI;

	[SerializeField]
	private Button buttonElement;

	[SerializeField]
	private PlayerSocialPopup playerSocialPopup;

	[SerializeField]
	private EmbeddedPlayerConfig embeddedPlayerConfig;

	private int profileId;

	private bool isLocalPlayerElement;

	private bool subscriber;

	public void Initialize(int userRank, string userName, int amountOfGamePoints, int profileID, bool isSubscriber)
	{
		isLocalPlayerElement = profileID == MVGameControllerBase.Game.LocalPlayer.ProfileID;
		profileId = profileID;
		userRankText.text = userRank.ToString();
		userNameText.text = userName;
		userAmountOfGamePointText.text = amountOfGamePoints.ToString();
		subscriber = isSubscriber;
		if (isLocalPlayerElement)
		{
			backgroundImage.color = Styles.GetColor(ColorStyle.OffGray);
			buttonElement.interactable = false;
			userRankText.color = Styles.GetColor(ColorStyle.Gray);
			userNameText.color = Styles.GetColor(ColorStyle.Gray);
			userAmountOfGamePointText.color = Styles.GetColor(ColorStyle.Gray);
		}
		else
		{
			Friend friendByProfileID = MVGameControllerBase.Game.Friends.GetFriendByProfileID(profileID);
			if (friendByProfileID != null && friendByProfileID.status == FriendStatus.Accepted)
			{
				userNameText.color = Styles.GetColor(ColorStyle.FriendGreen);
			}
			EmbeddedSiteConfigData currentSiteData = embeddedPlayerConfig.GetCurrentSiteData();
			if (!currentSiteData.allowsOpenInNewTab && !currentSiteData.allowsRedirectToWebpage)
			{
				buttonElement.interactable = false;
			}
		}
		if (isSubscriber)
		{
			subscriberUI.SetActive(value: true);
		}
		FriendList friends = MVGameControllerBase.Game.Friends;
		friends.OnFriendRequestAccepted = (FriendList.OnFriendRequestUpdated)Delegate.Combine(friends.OnFriendRequestAccepted, new FriendList.OnFriendRequestUpdated(FriendRequestAccepted));
	}

	private void FriendRequestAccepted(Friend friend)
	{
		if (friend.profileID == profileId)
		{
			userNameText.color = Styles.GetColor(ColorStyle.FriendGreen);
		}
	}

	private void OnDestroy()
	{
		FriendList friends = MVGameControllerBase.Game.Friends;
		friends.OnFriendRequestAccepted = (FriendList.OnFriendRequestUpdated)Delegate.Remove(friends.OnFriendRequestAccepted, new FriendList.OnFriendRequestUpdated(FriendRequestAccepted));
	}

	public void OnClick()
	{
		PlayerSocialPopup popup = UnityEngine.Object.Instantiate(playerSocialPopup);
		popup.Initialize(profileId, userNameText.text, subscriber);
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
		{
			x.Push(popup.gameObject, UIPushOption.InvisibleBlocker, null, UIGroupFlags.Popup);
		});
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		if (!isLocalPlayerElement)
		{
			backgroundImage.color = Styles.GetColor(ColorStyle.White);
		}
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		if (!isLocalPlayerElement)
		{
			backgroundImage.color = Styles.GetColor(ColorStyle.OffWhite);
		}
	}

	public void DeactivateTopBorder()
	{
		topBorderGameObject.gameObject.SetActive(value: false);
	}
}
