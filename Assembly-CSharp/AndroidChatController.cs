using System;
using System.Collections.Generic;
using MV.Common;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class AndroidChatController : MonoBehaviour
{
	private const int maxLineCount = 50;

	private string joinLeaveMessageFormat = "<color=#{0}>{1}</color> <color=#{2}>{3}</color>";

	private string killMessageFormat = "<color=#{0}>{1}</color>";

	private string joinStatusMessageFormat = "<color=#{0}>{1}</color>";

	private string chatMessageFromFriend = "<color=#{0}><b>[{1}]: </b></color><color=#{2}>{3}</color>";

	private string chatMessageFormat = "<color=#{0}>[{1}]: </color><color=#{2}>{3}</color>";

	private Queue<Text> lines = new Queue<Text>();

	[SerializeField]
	private ChatConsoleModes chatConsoleModes;

	[SerializeField]
	private RectTransform rectTransform;

	[SerializeField]
	private ConsoleDragAndTapHandler enterChatButton;

	[SerializeField]
	private ScrollRect scrollRect;

	[SerializeField]
	private InputField inputField;

	[SerializeField]
	private RectTransform inputAreaRoot;

	[SerializeField]
	private Text consoleLinePrefab;

	[SerializeField]
	private RectTransform contentPanel;

	[SerializeField]
	private Color systemMessageColor;

	[SerializeField]
	private Color killMessageColor;

	[SerializeField]
	private Color chatMessageColor;

	[SerializeField]
	private Color chatMessageDefaultNameColor;

	[SerializeField]
	private Color friendNameColor;

	private void Start()
	{
		MVGameControllerBase.OnReceivedGameMsg = (MVGameControllerBase.OnReceivedGameMsgDelegate)Delegate.Combine(MVGameControllerBase.OnReceivedGameMsg, new MVGameControllerBase.OnReceivedGameMsgDelegate(ReceiveMessage));
		ConsoleDragAndTapHandler consoleDragAndTapHandler = enterChatButton;
		consoleDragAndTapHandler.OnClick = (UnityAction)Delegate.Combine(consoleDragAndTapHandler.OnClick, new UnityAction(OnChatModeTapped));
		chatConsoleModes = UnityEngine.Object.Instantiate(chatConsoleModes);
		chatConsoleModes.transform.SetParent(transform.parent, worldPositionStays: false);
		chatConsoleModes.Set(ChatConsoleMode.PlayMode, ref rectTransform);
		enterChatButton.gameObject.SetActive(value: false);
		inputAreaRoot.gameObject.SetActive(value: false);
	}

	public void Initialize()
	{
		transform.SetAsLastSibling();
		enterChatButton.gameObject.SetActive(value: true);
		chatConsoleModes.Set(ChatConsoleMode.ChatLobbyMode, ref rectTransform);
		inputAreaRoot.gameObject.SetActive(!MVGameControllerBase.IsTouristSession);
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
	}

	private void SetMode(ChatConsoleMode chatConsoleMode)
	{
		switch (chatConsoleMode)
		{
		case ChatConsoleMode.ChatLobbyMode:
		case ChatConsoleMode.ChatPlayMode:
			enterChatButton.SetScrollingEnabled(scrollEnabled: true);
			inputAreaRoot.gameObject.SetActive(!MVGameControllerBase.IsTouristSession);
			break;
		case ChatConsoleMode.PlayMode:
			enterChatButton.SetScrollingEnabled(scrollEnabled: false);
			inputAreaRoot.gameObject.SetActive(value: false);
			break;
		}
		chatConsoleModes.Set(chatConsoleMode, ref rectTransform);
		scrollRect.verticalNormalizedPosition = 0f;
	}

	private void ReceiveMessage(MVGameMsgType msgType, Dictionary<object, object> message)
	{
		switch (msgType)
		{
		case MVGameMsgType.JoinFlowStatus:
			AddJoinFlowStatusLine(message);
			break;
		case MVGameMsgType.AvatarKilled:
			break;
		case MVGameMsgType.UserJoined:
			break;
		case MVGameMsgType.UserLeft:
			break;
		case MVGameMsgType.CollectiblePickedUp:
			break;
		case MVGameMsgType.AchievementUnlocked:
			break;
		case MVGameMsgType.CheckpointReached:
			break;
		case MVGameMsgType.Chat:
			AddChatLine(message);
			break;
		case MVGameMsgType.AdminMsg:
			AddAdminMessage(message);
			break;
		}
	}

	private void AddAdminMessage(Dictionary<object, object> data)
	{
		string msg = (string)data[(byte)5];
		AddAdminMessage(msg);
	}

	private void AddAdminMessage(string msg)
	{
		msg = string.Format(joinStatusMessageFormat, Styles.ColorToHex(systemMessageColor), msg);
		AddLine(msg);
	}

	private void JoinMessage(Dictionary<object, object> data)
	{
		int key = (int)data[(byte)0];
		MVPlayer mVPlayer = MVGameControllerBase.Game.Players[key];
		if (!mVPlayer.IsAnonymous)
		{
			Color color = chatMessageDefaultNameColor;
			if (MVGameControllerBase.Game.Friends.IsFriend(mVPlayer.ProfileID))
			{
				color = friendNameColor;
			}
			string text = string.Format(joinLeaveMessageFormat, Styles.ColorToHex(color), mVPlayer.Username, Styles.ColorToHex(chatMessageColor), TM._("joined the game"));
			AddLine(text);
		}
	}

	private void LeaveMessage(Dictionary<object, object> data)
	{
		bool flag = (bool)data[(byte)6];
		if (flag)
		{
			string text = (string)data[(byte)3];
			Color color = chatMessageDefaultNameColor;
			if (flag)
			{
				color = friendNameColor;
			}
			string text2 = string.Format(joinLeaveMessageFormat, Styles.ColorToHex(color), text, Styles.ColorToHex(chatMessageColor), TM._("left the game"));
			AddLine(text2);
		}
	}

	private void AddKillMessage(Dictionary<object, object> data)
	{
		if (GameMessagesRepository.ShowKilledMessage(data))
		{
			string arg = GameMessagesRepository.CreateKilledMessage(data);
			arg = string.Format(killMessageFormat, Styles.ColorToHex(killMessageColor), arg);
			AddLine(arg);
		}
	}

	private void AddJoinFlowStatusLine(Dictionary<object, object> data)
	{
		string arg = (string)data[(byte)5];
		arg = string.Format(joinStatusMessageFormat, Styles.ColorToHex(systemMessageColor), arg);
		AddLine(arg);
	}

	private void AddChatLine(Dictionary<object, object> data)
	{
		string text = (string)data[(byte)5];
		int key = (int)data[(byte)0];
		Color teamColor = chatMessageDefaultNameColor;
		MVPlayer mVPlayer = MVGameControllerBase.Game.Players[key];
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
		string text2 = string.Format(format, Styles.ColorToHex(teamColor), mVPlayer.Username, Styles.ColorToHex(chatMessageColor), text);
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
}
