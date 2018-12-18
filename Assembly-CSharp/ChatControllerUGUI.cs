using System;
using System.Collections.Generic;
using MV.Common;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.UI;

public class ChatControllerUGUI : MonoBehaviour
{
	private bool waitForLocalPlayerReady;

	private const string adminMessageFormat = "<color=#{0}>{1}</color>";

	private const string chatMessageFromFriend = "<color=#{0}>[{1}]: </color><color=#{2}>{3}</color>";

	private const string chatMessageFormat = "<color=#{0}>[{1}]: </color><color=#{2}>{3}</color>";

	private const string teamMessageFormat = "<color=#{0}>[{1}] </color><color=#{2}>{3}: </color><color=#{4}>{5}</color>";

	private const string sayMessageFormat = "<color=#{0}>[{1}] </color><color=#{2}>{3}: </color><color=#{4}>{5}</color>";

	private const string warningMessageFormat = "<color=#{0}>{1}</color>";

	private bool shouldUpdateFade;

	private float startTime;

	private const float timeBeforeFade = 10f;

	private const float fadeTime = 1f;

	private float currFade;

	private bool currentlyInLobbyState;

	private const float sayHearingDistance = 15f;

	private const int maxLineCount = 50;

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
	private Color warningColor = Color.red;

	[SerializeField]
	private Color killMessageColor;

	[SerializeField]
	private Color chatMessageColor;

	[SerializeField]
	private Color chatMessageDefaultNameColor;

	[SerializeField]
	private Color friendNameColor;

	[SerializeField]
	private Color sayColor;

	[SerializeField]
	private VerticalLayoutGroup textGroup;

	private bool promptRegisterForChat = true;

	private void Awake()
	{
		inputAreaRoot.gameObject.SetActive(value: false);
		inputAreaDeactivated.gameObject.SetActive(value: true);
		startTime = Time.time;
		enterChatButton.gameObject.SetActive(value: false);
	}

	private void Start()
	{
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IShortcutKeyRegister x, BaseEventData y) =>
		{
			x.RegisterShortcutKey(KogamaControls.ShowChat, KeyState.Down, ChatHotkeyPressed);
		});
		SendMessageControl sendMessageControl = messageController;
		sendMessageControl.DoSend = (UnityAction<bool>)Delegate.Combine(sendMessageControl.DoSend, new UnityAction<bool>(ChatFocusChanged));
		SendMessageControl sendMessageControl2 = messageController;
		sendMessageControl2.SpamWarning = (UnityAction)Delegate.Combine(sendMessageControl2.SpamWarning, new UnityAction(WarnForSpam));
		messageController.SayChatColor = sayColor;
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
		waitForLocalPlayerReady = true;
		if (MVGameControllerBase.Game.LocalPlayer.IsReady)
		{
			InitializeReady();
		}
	}

	private void InitializeReady()
	{
		waitForLocalPlayerReady = false;
		transform.SetAsLastSibling();
		ChatFocusChanged(MVGameControllerBase.GameMode != MVGameMode.Edit);
		if (MVGameControllerBase.GameMode == MVGameMode.Play)
		{
			shouldUpdateFade = false;
			inputField.ActivateInputField();
		}
		enterChatButton.gameObject.SetActive(!MVGameControllerBase.IsTouristSession);
		inputAreaRoot.gameObject.SetActive(!MVGameControllerBase.IsTouristSession);
		inputAreaDeactivated.gameObject.SetActive(MVGameControllerBase.IsTouristSession);
		currentlyInLobbyState = true;
		if (MVGameControllerBase.GameMode == MVGameMode.Edit)
		{
			inputField.DeactivateInputField();
			messageController.OnInputFocusChange(isFocused: false);
		}
	}

	private void Update()
	{
		if (waitForLocalPlayerReady)
		{
			if (MVGameControllerBase.Game.LocalPlayer.IsReady)
			{
				InitializeReady();
			}
			return;
		}
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
			if (promptRegisterForChat && MVGameControllerBase.IsTouristSession)
			{
				promptRegisterForChat = false;
				NotificationController.PushNotification(NotificationType.RegisterToChat);
			}
			ChatFocusChanged(enterChatMode: true);
		}
	}

	public void ChatFocusChanged(bool enterChatMode)
	{
		if (enterChatMode)
		{
			inputAreaRoot.gameObject.SetActive(!MVGameControllerBase.IsTouristSession);
			inputAreaDeactivated.gameObject.SetActive(MVGameControllerBase.IsTouristSession);
			inputField.ActivateInputField();
			messageController.OnInputFocusChange(isFocused: true);
		}
		else
		{
			bool flag = MVGameControllerBase.EditModeUI != null && !MVGameControllerBase.EditModeUI.IsInPlayInEditMode;
			messageController.OnInputFocusChange(isFocused: false);
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
		if (MVGameControllerBase.JoinState == MVJoinState.Playing)
		{
			canvasGroup.alpha = 1f;
			currFade = 0f;
			startTime = Time.time;
			canvasGroup.blocksRaycasts = true;
		}
	}

	private void ScrollbarChanged(Vector2 value)
	{
		shouldUpdateFade = true;
		if (MVGameControllerBase.GameMode == MVGameMode.Play)
		{
			shouldUpdateFade = !currentlyInLobbyState;
		}
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
		arg = $"<color=#{Styles.ColorToHex(systemMessageColor)}>{arg}</color>";
		AddLine(arg);
	}

	private void AddWarningMessage(Dictionary<object, object> data)
	{
		string text = (string)data[(byte)5];
		if (text.Length > 1536)
		{
			text.Substring(0, 1536);
		}
		text = $"<color=#{Styles.ColorToHex(warningColor)}>{text}</color>";
		AddLine(text);
	}

	private void HandleTeamChatMessage(Dictionary<object, object> data)
	{
		int actorNr = (int)data[(byte)0];
		MVPlayer playerUnsafe = MVGameControllerBase.Game.MVPlayerContainer.GetPlayerUnsafe(actorNr);
		MVWorldObjectClient worldObjectClient = MVGameControllerBase.WOCM.GetWorldObjectClient(playerUnsafe.Avatar.Id);
		if (worldObjectClient != null && MVGameControllerBase.Game.TeamManager.IsOnSameTeam(MVGameControllerBase.WOCM.AvatarLocal, worldObjectClient))
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
		if (MVGameControllerBase.Game.Friends.IsFriend(mVPlayer.ProfileID))
		{
			color = friendNameColor;
		}
		return $"<color=#{Styles.ColorToHex(color)}>[{mVPlayer.Username}] </color><color=#{Styles.ColorToHex(teamColor)}>{text2}: </color><color=#{Styles.ColorToHex(chatMessageColor)}>{text}</color>";
	}

	private void HandleSayChatMessage(Dictionary<object, object> data)
	{
		if (MVGameControllerBase.Game.LocalPlayer.IsReady)
		{
			int actorNumber = (int)data[(byte)0];
			Vector3 position = MVGameControllerBase.WOCM.AvatarLocal.Avatar.transform.position;
			MVAvatar avatar = MVGameControllerBase.Game.MVPlayerContainer[actorNumber].Avatar;
			Vector3 position2 = avatar.Transform.position;
			if ((position - position2).magnitude <= 15f)
			{
				AddLine(FormatSayChatMessage(data));
				string text = (string)data[(byte)5];
				ChatBubbleManager.ShowChatBubble(text, avatar.Id, avatar.Avatar.AvatarUIHandler.ChatBubbleAnchor);
			}
		}
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
		if (MVGameControllerBase.Game.Friends.IsFriend(mVPlayer.ProfileID))
		{
			color = friendNameColor;
		}
		return $"<color=#{Styles.ColorToHex(color)}>[{mVPlayer.Username}] </color><color=#{Styles.ColorToHex(sayColor)}>{text2}: </color><color=#{Styles.ColorToHex(chatMessageColor)}>{text}</color>";
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
		string format = "<color=#{0}>[{1}]: </color><color=#{2}>{3}</color>";
		if (flag)
		{
			format = "<color=#{0}>[{1}]: </color><color=#{2}>{3}</color>";
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

	private void OnEnable()
	{
		shouldUpdateFade = true;
		UpdateFadeTime();
	}
}
