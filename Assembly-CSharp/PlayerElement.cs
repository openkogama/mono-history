using System.Collections.Generic;
using MV.Common;
using MV.WorldObject.Subscription;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PlayerElement : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IEventSystemHandler
{
	[SerializeField]
	private Text playerName;

	[SerializeField]
	private Text rank;

	[SerializeField]
	private Text score;

	[SerializeField]
	private GameObject memberUI;

	[SerializeField]
	private Text memberRank;

	[SerializeField]
	private Image nonMemberUI;

	[SerializeField]
	private Button playerNameAndScoreButton;

	[SerializeField]
	private List<Image> backgrounds;

	[SerializeField]
	private PlayerSocialPopup playerSocialPopupPrefab;

	[SerializeField]
	private Button dotsImage;

	[SerializeField]
	private Button incomingFriendRequestRedDot;

	[SerializeField]
	private Image underline;

	private string realPlayerName;

	private bool subscriber;

	private int profileId;

	public string PlayerName => playerName.text;

	public void Initialize(MVPlayer player, GameStatCounterType typeToDisplay, int scoreValue)
	{
		Friend friendByProfileID = MVGameControllerBase.Game.Friends.GetFriendByProfileID(player.ProfileID);
		underline.gameObject.SetActive(value: false);
		dotsImage.gameObject.SetActive(value: true);
		incomingFriendRequestRedDot.gameObject.SetActive(value: false);
		bool flag = friendByProfileID != null && friendByProfileID.status == FriendStatus.Accepted;
		if (friendByProfileID != null && friendByProfileID.status == FriendStatus.Pending)
		{
			bool flag2 = MVGameControllerBase.Game.Friends.Friends.ContainsValue(friendByProfileID);
			dotsImage.gameObject.SetActive(flag2);
			incomingFriendRequestRedDot.gameObject.SetActive(!flag2);
		}
		if (player == MVGameControllerBase.Game.LocalPlayer)
		{
			for (int i = 0; i < backgrounds.Count; i++)
			{
				backgrounds[i].color = Styles.GetColor(ColorStyle.LocalPlayerBackground);
			}
		}
		if (player.SubscriptionRules.HasBenefit(SubscriptionBenefit.XPBoost))
		{
			ActivateSubscriberUI(flag);
		}
		else
		{
			memberUI.SetActive(value: false);
		}
		if (flag)
		{
			playerName.color = Styles.GetColor(ColorStyle.FriendGreen);
		}
		realPlayerName = player.UserProfileData.UserName;
		playerName.text = realPlayerName;
		if (typeToDisplay == GameStatCounterType.None)
		{
			score.gameObject.SetActive(value: false);
		}
		else
		{
			score.gameObject.SetActive(value: true);
			score.text = WinningConditionControl.MakeIntoScoreText(scoreValue, typeToDisplay);
		}
		profileId = player.ProfileID;
		if (profileId == 0 || player == MVGameControllerBase.Game.LocalPlayer)
		{
			playerNameAndScoreButton.interactable = false;
			dotsImage.gameObject.SetActive(value: false);
			incomingFriendRequestRedDot.gameObject.SetActive(value: false);
		}
	}

	public void OnPlayerClicked()
	{
		PlayerSocialPopup popup = Object.Instantiate(playerSocialPopupPrefab);
		popup.Initialize(profileId, realPlayerName, subscriber);
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
		{
			x.Push(popup.gameObject, UIPushOption.InvisibleBlocker, null, UIGroupFlags.Popup);
		});
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		underline.gameObject.SetActive(value: true);
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		underline.gameObject.SetActive(value: false);
	}

	public void UpdateScoreIndex()
	{
		string text = (transform.GetSiblingIndex() + 1).ToString();
		if (!memberUI.activeSelf)
		{
			rank.text = text;
		}
		memberRank.text = text;
	}

	private void ActivateSubscriberUI(bool isFriend)
	{
		subscriber = true;
		memberUI.SetActive(value: true);
		nonMemberUI.enabled = false;
		rank.text = string.Empty;
		if (!isFriend)
		{
			playerName.color = Styles.GetColor(ColorStyle.OffWhite);
		}
		score.color = Styles.GetColor(ColorStyle.OffWhite);
		for (int i = 0; i < backgrounds.Count; i++)
		{
			backgrounds[i].color = Styles.GetColor(ColorStyle.Gray);
		}
	}
}
