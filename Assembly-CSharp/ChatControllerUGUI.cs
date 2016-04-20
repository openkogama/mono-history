using System;
using System.Collections.Generic;
using MV.Common;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.UI;

public class ChatControllerUGUI : MonoBehaviour
{
	private const string joinLeaveMessageFormat = "<color=#{0}>{1}</color> <color=#{2}>{3}</color>";

	private const string killMessageFormat = "<color=#{0}>{1}</color>";

	private const string joinStatusMessageFormat = "<color=#{0}>{1}</color>";

	private const string chatMessageFromFriend = "<color=#{0}><b>[{1}]: </b></color><color=#{2}>{3}</color>";

	private const string chatMessageFormat = "<color=#{0}>[{1}]: </color><color=#{2}>{3}</color>";

	private const float timeBeforeFade = 10f;

	private const float fadeTime = 1f;

	private const int maxLineCount = 50;

	private bool shouldUpdateFade = true;

	private float startTime;

	private float currFade;

	private bool currentlyInLobbyState;

	private Queue<Text> lines = new Queue<Text>();

	[SerializeField]
	private ScrollRect scrollRect;

	[SerializeField]
	private RectTransform inputAreaRoot;

	[SerializeField]
	private RectTransform inputAreaDeactivated;

	[SerializeField]
	private InputField inputField;

	[SerializeField]
	private ConsoleDragAndTapHandler enterChatButton;

	[SerializeField]
	private CanvasGroup canvasGroup;

	[SerializeField]
	private SendMessageControl messageController;

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

	[SerializeField]
	private VerticalLayoutGroup textGroup;

	private void Start()
	{
		inputAreaRoot.gameObject.SetActive(value: false);
		inputAreaDeactivated.gameObject.SetActive(value: true);
		startTime = Time.time;
		enterChatButton.gameObject.SetActive(value: false);
		if (MVGameControllerBase.GameMode != MVGameMode.CharacterEditor)
		{
			ExecuteEvents.ExecuteHierarchy(gameObject, null, (IShortcutKeyRegister x, BaseEventData y) =>
			{
				x.RegisterShortcutKey(KogamaControls.ShowChat, KeyState.Down, ChatHotkeyPressed);
			});
		}
		SendMessageControl sendMessageControl = messageController;
		sendMessageControl.DoSend = (UnityAction<bool>)Delegate.Combine(sendMessageControl.DoSend, new UnityAction<bool>(ChatFocusChanged));
		SendMessageControl sendMessageControl2 = messageController;
		sendMessageControl2.SpamWarning = (UnityAction)Delegate.Combine(sendMessageControl2.SpamWarning, new UnityAction(WarnForSpam));
		scrollRect.onValueChanged.AddListener(ScrollbarChanged);
	}

	public void SubscribeToMessages()
	{
		MVGameControllerBase.OnReceivedGameMsg = (MVGameControllerBase.OnReceivedGameMsgDelegate)Delegate.Combine(MVGameControllerBase.OnReceivedGameMsg, new MVGameControllerBase.OnReceivedGameMsgDelegate(ReceiveMessage));
	}

	private void WarnForSpam()
	{
		AddLine(TM._("Warning: You are sending too many messages, please wait a few moments before sending again."));
	}

	public void Initialize()
	{
		transform.SetAsLastSibling();
		ChatFocusChanged(MVGameControllerBase.GameMode != MVGameMode.Edit);
		if (MVGameControllerBase.GameMode == MVGameMode.Play)
		{
			shouldUpdateFade = false;
		}
		enterChatButton.gameObject.SetActive(value: true);
		currentlyInLobbyState = true;
		if (MVGameControllerBase.GameMode == MVGameMode.Edit)
		{
			inputField.DeactivateInputField();
		}
	}

	private void Update()
	{
		if (inputField.isFocused)
		{
			UpdateFadeTime();
		}
		if (shouldUpdateFade && Time.time - startTime >= 10f)
		{
			currFade += Time.deltaTime;
			canvasGroup.alpha = Mathf.Lerp(1f, 0f, currFade);
			if (currFade >= 1f)
			{
				shouldUpdateFade = false;
				canvasGroup.blocksRaycasts = false;
			}
		}
	}

	private void ChatHotkeyPressed()
	{
		if (!inputField.isFocused)
		{
			ChatFocusChanged(enterChatMode: true);
		}
	}

	public void ChatFocusChanged(bool enterChatMode)
	{
		if (enterChatMode && MVGameControllerBase.GameMode != MVGameMode.CharacterEditor)
		{
			inputAreaRoot.gameObject.SetActive(!MVGameControllerBase.IsTouristSession);
			inputAreaDeactivated.gameObject.SetActive(MVGameControllerBase.IsTouristSession);
			inputField.ActivateInputField();
		}
		else
		{
			bool flag = MVGameControllerBase.IEditModeUI != null && !MVGameControllerBase.IEditModeUI.IsInPlayInEditMode;
			if (!currentlyInLobbyState || flag)
			{
				inputField.DeactivateInputField();
				inputAreaDeactivated.gameObject.SetActive(!flag);
				inputAreaRoot.gameObject.SetActive(flag);
			}
		}
		shouldUpdateFade = true;
		UpdateFadeTime();
		if (MVGameControllerBase.GameMode == MVGameMode.Play)
		{
			shouldUpdateFade = !currentlyInLobbyState;
		}
	}

	public void UpdateFadeTime()
	{
		canvasGroup.alpha = 1f;
		currFade = 0f;
		startTime = Time.time;
		canvasGroup.blocksRaycasts = true;
	}

	private void ScrollbarChanged(Vector2 value)
	{
		UpdateFadeTime();
	}

	public void OnLobbyStateChange(bool cursorLocked)
	{
		currentlyInLobbyState = !cursorLocked;
		UpdateFadeTime();
		shouldUpdateFade = cursorLocked;
		ChatFocusChanged(enterChatMode: false);
		bool flag = !MVGameControllerBase.IsTouristSession && !cursorLocked;
		inputAreaRoot.gameObject.SetActive(flag);
		inputAreaDeactivated.gameObject.SetActive(!flag);
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
			LeaveMessage(message);
			break;
		case MVGameMsgType.CollectiblePickedUp:
			Debug.LogWarning("Not showing CollectiblePickedUp line");
			break;
		case MVGameMsgType.AchievementUnlocked:
			break;
		case MVGameMsgType.CheckpointReached:
			Debug.LogWarning("Not showing CheckpointReached line");
			break;
		case MVGameMsgType.Chat:
			AddChatLine(message);
			break;
		case MVGameMsgType.AdminMsg:
			AddAdminMessage(message);
			break;
		default:
			Debug.Log("GameMsg of type " + msgType.ToString() + " received...");
			break;
		}
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
			string text = string.Format("<color=#{0}>{1}</color> <color=#{2}>{3}</color>", Styles.ColorToHex(color), mVPlayer.Username, Styles.ColorToHex(chatMessageColor), TM._("joined the game"));
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
			string text2 = string.Format("<color=#{0}>{1}</color> <color=#{2}>{3}</color>", Styles.ColorToHex(color), text, Styles.ColorToHex(chatMessageColor), TM._("left the game"));
			AddLine(text2);
		}
	}

	private void AddKillMessage(Dictionary<object, object> data)
	{
		if (GameMessagesRepository.ShowKilledMessage(data))
		{
			string arg = GameMessagesRepository.CreateKilledMessage(data);
			arg = $"<color=#{Styles.ColorToHex(killMessageColor)}>{arg}</color>";
			AddLine(arg);
		}
	}

	private void AddJoinFlowStatusLine(Dictionary<object, object> data)
	{
		string arg = (string)data[(byte)5];
		arg = $"<color=#{Styles.ColorToHex(systemMessageColor)}>{arg}</color>";
		AddLine(arg);
	}

	private void AddAdminMessage(Dictionary<object, object> data)
	{
		string msg = (string)data[(byte)5];
		AddAdminMessage(msg);
	}

	private void AddAdminMessage(string msg)
	{
		msg = $"<color=#{Styles.ColorToHex(systemMessageColor)}>{msg}</color>";
		AddLine(msg);
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
		string format = "<color=#{0}>[{1}]: </color><color=#{2}>{3}</color>";
		if (flag)
		{
			format = "<color=#{0}><b>[{1}]: </b></color><color=#{2}>{3}</color>";
			teamColor = friendNameColor;
		}
		string text2 = string.Format(format, Styles.ColorToHex(teamColor), mVPlayer.Username, Styles.ColorToHex(chatMessageColor), text);
		AddLine(text2);
	}

	private void AddLine(string text)
	{
		Text text2 = ((lines.Count >= 50) ? ReuseLine() : InstantiateNewLine());
		text2.text = text;
		if (scrollRect.verticalNormalizedPosition != 0f)
		{
			textGroup.gameObject.SetActive(value: false);
			textGroup.gameObject.SetActive(value: true);
		}
		UpdateFadeTime();
		if (MVGameControllerBase.GameMode == MVGameMode.Play)
		{
			shouldUpdateFade = !currentlyInLobbyState;
		}
	}

	private Text InstantiateNewLine()
	{
		Text text = UnityEngine.Object.Instantiate(consoleLinePrefab);
		lines.Enqueue(text);
		text.transform.SetParent(contentPanel, worldPositionStays: false);
		text.transform.SetAsLastSibling();
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
