using System;
using System.Collections.Generic;
using MV.Common;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class AndroidChatController : MonoBehaviour
{
	private string adminMessageFormat = "<color=#{0}>{1}</color>";

	private string chatMessageFromFriend = "<color=#{0}><b>[{1}]: </b></color><color=#{2}>{3}</color>";

	private string chatMessageFormat = "<color=#{0}>[{1}]: </color><color=#{2}>{3}</color>";

	private const string teamMessageFormat = "<color=#{0}>[{1}] </color><color=#{2}>{3}: </color><color=#{4}>{5}</color>";

	private const string sayMessageFormat = "<color=#{0}>[{1}] </color><color=#{2}>{3}: </color><color=#{4}>{5}</color>";

	private string warningMessageFormat = "<color=#{0}>{1}</color>";

	private const int maxLineCount = 50;

	private Queue<Text> lines = new Queue<Text>();

	private bool promptRegisterForChat = true;

	private const float sayHearingDistance = 15f;

	[SerializeField]
	private ChatConsoleModes chatConsoleModes;

	[SerializeField]
	private RectTransform rectTransform;

	[SerializeField]
	private ConsoleDragAndTapHandler enterChatButton;

	[SerializeField]
	private ScrollRect scrollRect;

	[SerializeField]
	private RectTransform inputAreaRoot;

	[SerializeField]
	private RectTransform expandChat;

	[SerializeField]
	private RectTransform minimizeChat;

	[SerializeField]
	private SendMessageControl messageController;

	[SerializeField]
	private Text consoleLinePrefab;

	[SerializeField]
	private RectTransform contentPanel;

	[SerializeField]
	private Color systemMessageColor;

	[SerializeField]
	private Color chatMessageColor;

	[SerializeField]
	private Color chatMessageDefaultNameColor;

	[SerializeField]
	private Color warningMessageColor = Color.red;

	[SerializeField]
	private Color sayColor;

	public void Initialize()
	{
		MVGameControllerBase.OnReceivedGameMsg = (MVGameControllerBase.OnReceivedGameMsgDelegate)Delegate.Combine(MVGameControllerBase.OnReceivedGameMsg, new MVGameControllerBase.OnReceivedGameMsgDelegate(ReceiveMessage));
		ConsoleDragAndTapHandler consoleDragAndTapHandler = enterChatButton;
		consoleDragAndTapHandler.OnClick = (UnityAction)Delegate.Combine(consoleDragAndTapHandler.OnClick, new UnityAction(OnChatModeTapped));
		SayChatBubbleVisibilityManager.OnSayChatMessageHeard = (Action<Dictionary<object, object>>)Delegate.Combine(SayChatBubbleVisibilityManager.OnSayChatMessageHeard, new Action<Dictionary<object, object>>(OnSayChatMessageHeard));
		chatConsoleModes = UnityEngine.Object.Instantiate(chatConsoleModes);
		chatConsoleModes.transform.SetParent(transform.parent, worldPositionStays: false);
		messageController.SayChatColor = sayColor;
		transform.SetAsLastSibling();
		enterChatButton.gameObject.SetActive(value: true);
		chatConsoleModes.Set(ChatConsoleMode.ChatLobbyMode, ref rectTransform);
		inputAreaRoot.gameObject.SetActive(!MVGameControllerBase.IsTouristSession);
		SetMode(ChatConsoleMode.ChatLobbyMode);
	}

	public void OnChatModeTapped()
	{
		if (chatConsoleModes.ChatConsoleMode == ChatConsoleMode.ChatPlayMode)
		{
			SetMode(ChatConsoleMode.PlayMode);
		}
		else if (chatConsoleModes.ChatConsoleMode == ChatConsoleMode.PlayMode)
		{
			SetMode(ChatConsoleMode.ChatPlayMode);
		}
	}

	public void OnLobbyStateChange(bool inLobbyState)
	{
		if (chatConsoleModes.ChatConsoleMode == ChatConsoleMode.ChatLobbyMode && !inLobbyState)
		{
			SetMode(ChatConsoleMode.PlayMode);
		}
		else if (chatConsoleModes.ChatConsoleMode == ChatConsoleMode.PlayMode && inLobbyState)
		{
			SetMode(ChatConsoleMode.ChatLobbyMode);
		}
		else
		{
			SetMode(ChatConsoleMode.ChatLobbyMode);
		}
	}

	private void SetMode(ChatConsoleMode chatConsoleMode)
	{
		switch (chatConsoleMode)
		{
		case ChatConsoleMode.ChatLobbyMode:
			enterChatButton.SetScrollingEnabled(scrollEnabled: true);
			inputAreaRoot.gameObject.SetActive(!MVGameControllerBase.IsTouristSession);
			minimizeChat.gameObject.SetActive(value: false);
			expandChat.gameObject.SetActive(value: false);
			break;
		case ChatConsoleMode.ChatPlayMode:
			enterChatButton.SetScrollingEnabled(scrollEnabled: true);
			if (promptRegisterForChat && MVGameControllerBase.IsTouristSession)
			{
				promptRegisterForChat = false;
				NotificationController.PushNotification(NotificationType.RegisterToChat);
			}
			inputAreaRoot.gameObject.SetActive(!MVGameControllerBase.IsTouristSession);
			minimizeChat.gameObject.SetActive(value: true);
			expandChat.gameObject.SetActive(value: false);
			break;
		case ChatConsoleMode.PlayMode:
			enterChatButton.SetScrollingEnabled(scrollEnabled: false);
			inputAreaRoot.gameObject.SetActive(value: false);
			minimizeChat.gameObject.SetActive(value: false);
			expandChat.gameObject.SetActive(value: true);
			break;
		}
		chatConsoleModes.Set(chatConsoleMode, ref rectTransform);
		scrollRect.verticalNormalizedPosition = 0f;
	}

	private void ReceiveMessage(MVGameMsgType msgType, Dictionary<object, object> message)
	{
		switch (msgType)
		{
		case MVGameMsgType.Chat:
			AddChatLine(message);
			break;
		case MVGameMsgType.TeamChat:
			HandleTeamChatMessage(message);
			break;
		case MVGameMsgType.SayChat:
			HandleSayChatMessage(message);
			break;
		case MVGameMsgType.AdminMsg:
			AddAdminMessage(message);
			break;
		case MVGameMsgType.Warning:
			AddWarningMessage(message);
			break;
		}
	}

	private void AddAdminMessage(Dictionary<object, object> data)
	{
		string arg = (string)data[(byte)5];
		arg = string.Format(adminMessageFormat, Styles.ColorToHex(systemMessageColor), arg);
		AddLine(arg);
	}

	private void AddWarningMessage(Dictionary<object, object> data)
	{
		string text = (string)data[(byte)5];
		if (text.Length > 1536)
		{
			text.Substring(0, 1536);
		}
		text = string.Format(warningMessageFormat, Styles.ColorToHex(warningMessageColor), text);
		AddLine(text);
	}

	private void AddChatLine(Dictionary<object, object> data)
	{
		string text = (string)data[(byte)5];
		int actorNumber = (int)data[(byte)0];
		Color teamColor = chatMessageDefaultNameColor;
		MVPlayer mVPlayer = MVGameControllerBase.Game.MVPlayerContainer[actorNumber];
		if (MVGameControllerBase.Game.TeamManager.TeamCount() > 1)
		{
			teamColor = Styles.GetTeamColor(mVPlayer.Team);
		}
		bool flag = MVGameControllerBase.Game.Friends.IsFriend(mVPlayer.ProfileID);
		string format = chatMessageFormat;
		if (flag)
		{
			format = chatMessageFromFriend;
		}
		string text2 = string.Format(format, Styles.ColorToHex(teamColor), mVPlayer.UserProfileData.UserName, Styles.ColorToHex(chatMessageColor), text);
		AddLine(text2);
	}

	private void AddLine(string text)
	{
		Text text2 = ((lines.Count >= 50) ? ReuseLine() : InstantiateNewLine());
		text2.text = text;
	}

	private Text InstantiateNewLine()
	{
		Text text = UnityEngine.Object.Instantiate(consoleLinePrefab);
		lines.Enqueue(text);
		text.transform.SetParent(contentPanel, worldPositionStays: false);
		return text;
	}

	private Text ReuseLine()
	{
		Text text = lines.Dequeue();
		lines.Enqueue(text);
		text.transform.SetParent(contentPanel, worldPositionStays: false);
		text.transform.SetAsLastSibling();
		return text;
	}

	private void HandleTeamChatMessage(Dictionary<object, object> data)
	{
		int actorNr = (int)data[(byte)0];
		MVPlayer playerUnsafe = MVGameControllerBase.Game.MVPlayerContainer.GetPlayerUnsafe(actorNr);
		if (playerUnsafe.IsOnSameTeam(MVGameControllerBase.Game.LocalPlayer))
		{
			AddLine(FormatTeamChatMessage(data));
		}
	}

	private string FormatTeamChatMessage(Dictionary<object, object> data)
	{
		string text = (string)data[(byte)5];
		int actorNumber = (int)data[(byte)0];
		string text2 = "(Team)";
		Color teamColor = chatMessageDefaultNameColor;
		MVPlayer mVPlayer = MVGameControllerBase.Game.MVPlayerContainer[actorNumber];
		if (MVGameControllerBase.Game.TeamManager.TeamCount() > 1)
		{
			teamColor = Styles.GetTeamColor(mVPlayer.Team);
		}
		Color color = teamColor;
		return $"<color=#{Styles.ColorToHex(color)}>[{mVPlayer.UserProfileData.UserName}] </color><color=#{Styles.ColorToHex(teamColor)}>{text2}: </color><color=#{Styles.ColorToHex(chatMessageColor)}>{text}</color>";
	}

	private void HandleSayChatMessage(Dictionary<object, object> data)
	{
		int num = (int)data[(byte)0];
		if (MVGameControllerBase.Game.LocalPlayer.ActorNr == num)
		{
			OnSayChatMessageHeard(data);
		}
		if (MVGameControllerBase.Game.LocalPlayer.IsReady && SayChatBubbleVisibilityManager.OnSayChatMessageRecieved != null)
		{
			SayChatBubbleVisibilityManager.OnSayChatMessageRecieved(num, data);
		}
	}

	private void OnSayChatMessageHeard(Dictionary<object, object> data)
	{
		AddLine(FormatSayChatMessage(data));
	}

	private string FormatSayChatMessage(Dictionary<object, object> data)
	{
		string text = (string)data[(byte)5];
		int actorNumber = (int)data[(byte)0];
		string text2 = "(says)";
		Color teamColor = chatMessageDefaultNameColor;
		MVPlayer mVPlayer = MVGameControllerBase.Game.MVPlayerContainer[actorNumber];
		if (MVGameControllerBase.Game.TeamManager.TeamCount() > 1)
		{
			teamColor = Styles.GetTeamColor(mVPlayer.Team);
		}
		Color color = teamColor;
		return $"<color=#{Styles.ColorToHex(color)}>[{mVPlayer.UserProfileData.UserName}] </color><color=#{Styles.ColorToHex(sayColor)}>{text2}: </color><color=#{Styles.ColorToHex(chatMessageColor)}>{text}</color>";
	}

	private void OnDestroy()
	{
		SayChatBubbleVisibilityManager.OnSayChatMessageHeard = (Action<Dictionary<object, object>>)Delegate.Remove(SayChatBubbleVisibilityManager.OnSayChatMessageHeard, new Action<Dictionary<object, object>>(OnSayChatMessageHeard));
	}
}
