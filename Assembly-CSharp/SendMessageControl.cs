using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using MV.Common;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.UI;

public class SendMessageControl : MonoBehaviour
{
	public UnityAction<bool> DoSend;

	public UnityAction SpamWarning;

	private const string showPerGameData = "/gp";

	private const string resetPlayerPlanetData = "/rgp";

	private const string helpString = "/h";

	private const string fps = "/f";

	private const string resolution = "/r";

	private const string mathTest = "/m";

	private const string chatCommands = "/c";

	private const string removeUI = "/ru";

	private const string switchAvatarTest = "/sat";

	private const string enableHD = "/hd";

	private const string buildInformation = "/build";

	private const string exportTool = "/export";

	private const string exportSelfTool = "/exportself";

	private const string chatChangeCommandAll = "/all";

	private const string chatChangeCommandTeam = "/team";

	private const string chatChangeCommandSay = "/say";

	private const string allChat = "[ All ]";

	private const string teamChat = "[ Team ]";

	private const string sayChat = "[ Say ]";

	private const string startHeadShake = "/no";

	private const string startNod = "/yes";

	private const string startWave = "/wave";

	private const string fyberTestSuite = "/fyber";

	private const string showAd = "/ad";

	[SerializeField]
	private Text currentChat;

	[SerializeField]
	private InputField inputField;

	[SerializeField]
	private float intervalForMessages = 5f;

	[SerializeField]
	private int maxMessagesPerInterval = 5;

	private Regex whiteSpaceCheck;

	private int frameCountSent;

	private const float sendMessageDelay = 0.01f;

	private float sendMessageCooldownTime;

	private List<float> spamList = new List<float>();

	private MVGameMsgType selectedChat = MVGameMsgType.Chat;

	private bool isSayChatIconVisible;

	private Color sayChatColor;

	public Color SayChatColor
	{
		set
		{
			sayChatColor = value;
		}
	}

	private void Awake()
	{
		whiteSpaceCheck = new Regex("\\S");
		MVTeamManager teamManager = MVGameControllerBase.Game.TeamManager;
		teamManager.OnTeamsUpdated = (MVTeamManager.OnTeamsUpdatedDelegate)Delegate.Combine(teamManager.OnTeamsUpdated, new MVTeamManager.OnTeamsUpdatedDelegate(ChangeTeamChatColor));
		MVPlayerContainer mVPlayerContainer = MVGameControllerBase.Game.MVPlayerContainer;
		mVPlayerContainer.OnPlayerListChanged = (Action)Delegate.Combine(mVPlayerContainer.OnPlayerListChanged, new Action(ChangeTeamChatColor));
	}

	private void OnDestroy()
	{
		if (MVGameControllerBase.IsAlive && MVGameControllerBase.Game != null)
		{
			MVTeamManager teamManager = MVGameControllerBase.Game.TeamManager;
			teamManager.OnTeamsUpdated = (MVTeamManager.OnTeamsUpdatedDelegate)Delegate.Remove(teamManager.OnTeamsUpdated, new MVTeamManager.OnTeamsUpdatedDelegate(ChangeTeamChatColor));
			MVPlayerContainer mVPlayerContainer = MVGameControllerBase.Game.MVPlayerContainer;
			mVPlayerContainer.OnPlayerListChanged = (Action)Delegate.Remove(mVPlayerContainer.OnPlayerListChanged, new Action(ChangeTeamChatColor));
		}
	}

	private void Send()
	{
		if (sendMessageCooldownTime > Time.time)
		{
			EnforceCharacterLimit();
			return;
		}
		sendMessageCooldownTime = Time.time + 0.01f;
		string text = inputField.text;
		text = Regex.Replace(text, "\\r\\n?|\\n", string.Empty);
		SanitizeMessage(ref text, "size");
		inputField.text = string.Empty;
		if (whiteSpaceCheck.Match(text).Length <= 0)
		{
			if (DoSend != null)
			{
				DoSend(arg0: false);
			}
			return;
		}
		for (int i = 0; i < spamList.Count; i++)
		{
			if (Time.timeSinceLevelLoad - spamList[i] > intervalForMessages)
			{
				spamList.Remove(spamList[i]);
			}
		}
		if (spamList.Count > maxMessagesPerInterval)
		{
			if (SpamWarning != null)
			{
				SpamWarning();
			}
		}
		else
		{
			spamList.Add(Time.timeSinceLevelLoad);
			SendChatMessage(text);
		}
	}

	public void OnInputFieldChange()
	{
		string text = inputField.text;
		if (text != string.Empty && text.Substring(text.Length - 1, 1) == " " && HandleChatSwapCommand(text.Substring(0, text.Length - 1)))
		{
			return;
		}
		if (text.IndexOf("\n") >= 0)
		{
			frameCountSent = Time.frameCount;
			Send();
			return;
		}
		if (text.IndexOf("\t") >= 0)
		{
			text = text.Substring(0, text.Length - 1);
			inputField.text = text;
			SetToNextChat();
			return;
		}
		EnforceCharacterLimit();
		if (selectedChat == MVGameMsgType.SayChat && !isSayChatIconVisible)
		{
			MVGameControllerBase.OperationRequests.SetSayChatBubbleVisible(shouldShow: true);
			isSayChatIconVisible = true;
		}
		else if (selectedChat != MVGameMsgType.SayChat && isSayChatIconVisible)
		{
			MVGameControllerBase.OperationRequests.SetSayChatBubbleVisible(shouldShow: false);
			isSayChatIconVisible = false;
		}
		if (frameCountSent == Time.frameCount)
		{
			inputField.text = string.Empty;
		}
	}

	private void EnforceCharacterLimit()
	{
		string text = inputField.text;
		if (text.Length >= inputField.characterLimit)
		{
			text = text.Substring(0, inputField.characterLimit - 1);
			inputField.text = text;
		}
	}

	private void SendChatMessage(string chatMsg)
	{
		if (chatMsg == string.Empty)
		{
			return;
		}
		if (chatMsg.Length > 256)
		{
			chatMsg = chatMsg.Substring(0, 256);
		}
		if (HandleChatSwapCommand(chatMsg))
		{
			return;
		}
		if (HandleChatCommands(chatMsg))
		{
			if (selectedChat == MVGameMsgType.SayChat)
			{
				MVGameControllerBase.OperationRequests.SetSayChatBubbleVisible(shouldShow: false);
				isSayChatIconVisible = false;
			}
			MVGameControllerBase.OperationRequests.PostChatMsg(new Dictionary<object, object>
			{
				{
					(byte)0,
					MVGameControllerBase.Game.LocalPlayer.ActorNr
				},
				{
					(byte)5,
					chatMsg
				}
			}, selectedChat);
		}
		if (DoSend != null)
		{
			DoSend(arg0: false);
		}
	}

	private bool HandleChatCommands(string chatMsg)
	{
		bool result = false;
		switch (chatMsg)
		{
		case "/h":
			MVGameControllerBase.PostGameMsg(MVGameMsgType.AdminMsg, CreateHelpTxt());
			break;
		case "/f":
			ExecuteEvents.ExecuteHierarchy(gameObject, null, (IToggleFps x, BaseEventData y) =>
			{
				x.ToggleFps();
			});
			break;
		case "/r":
			MVGameControllerBase.PostGameMsg(MVGameMsgType.AdminMsg, $"{Screen.width} x {Screen.height}");
			break;
		case "/m":
		{
			Vector3 one = Vector3.one;
			Quaternion identity = Quaternion.identity;
			MVGameControllerBase.PostGameMsg(MVGameMsgType.AdminMsg, "Math validation test");
			MVGameControllerBase.PostGameMsg(MVGameMsgType.AdminMsg, "Vector3.one " + MathFunctions.IsVectorFloatsValid(one));
			one.x = float.PositiveInfinity;
			MVGameControllerBase.PostGameMsg(MVGameMsgType.AdminMsg, "vector3.x = float.PositiveInfinity " + MathFunctions.IsVectorFloatsValid(one));
			one.x = float.NaN;
			MVGameControllerBase.PostGameMsg(MVGameMsgType.AdminMsg, "vector3.x = float.NaN " + MathFunctions.IsVectorFloatsValid(one));
			MVGameControllerBase.PostGameMsg(MVGameMsgType.AdminMsg, "Quaternion.identity " + MathFunctions.IsQuaternionFloatsValid(identity));
			identity.x = float.PositiveInfinity;
			MVGameControllerBase.PostGameMsg(MVGameMsgType.AdminMsg, "q.x = float.PositiveInfinity " + MathFunctions.IsQuaternionFloatsValid(identity));
			identity.x = float.NaN;
			MVGameControllerBase.PostGameMsg(MVGameMsgType.AdminMsg, "q.x = float.NaN " + MathFunctions.IsQuaternionFloatsValid(identity));
			break;
		}
		case "/c":
			MVGameControllerBase.PostGameMsg(MVGameMsgType.AdminMsg, CreateChatCommandsHelpTxt());
			break;
		case "/hd":
			ToggleHD();
			break;
		case "/ru":
			Debug.Log("Remote Playmode Avatar should remove his UI. Build mode avatar should do the same and also unequip the edit mode cube.");
			Debug.LogError("Build and play mode UI not implemented yet.");
			ChatCommandManager.ChatCommandActivated(ChatCommand.HideAllUI);
			break;
		case "/gp":
			GamePassesManager.ShowGamePassDataInConsole = !GamePassesManager.ShowGamePassDataInConsole;
			break;
		case "/rgp":
			MVGameControllerBase.Game.OperationRequestSender.ResetPlayerPlanetData();
			break;
		case "/build":
			ShowBuildInformation();
			break;
		case "/no":
			ChatCommandManager.ChatCommandActivated(ChatCommand.StartShake);
			break;
		case "/yes":
			ChatCommandManager.ChatCommandActivated(ChatCommand.StartNod);
			break;
		case "/wave":
			ChatCommandManager.ChatCommandActivated(ChatCommand.StartWave);
			break;
		case "/ad":
			BrowserComm.ToJavaScript.ExternalCall("showVideoAd", OnAdShownCallback);
			break;
		case "/export":
			ObjExportHandler.InitializePicking();
			break;
		case "/exportself":
			ObjExportHandler.ExportSelfAvatar();
			break;
		default:
			if (chatMsg[0] == '/')
			{
				TextCommand.Resolve(chatMsg);
			}
			else
			{
				result = true;
			}
			break;
		case "/sat":
			break;
		}
		return result;
	}

	private void OnAdShownCallback(bool ok, string json)
	{
		Debug.Log("WebGL Ad shown.");
	}

	private void ShowBuildInformation()
	{
		string message = string.Format(TM._("Version: {0}\nBranch: {1}\nCommitMessage: {2}"), MVGameControllerBase.KoGaMaSettings.VersionString, MVGameControllerBase.KoGaMaSettings.BranchName, MVGameControllerBase.KoGaMaSettings.LatestCommitMessage);
		MVGameControllerBase.PostGameMsg(MVGameMsgType.AdminMsg, message);
	}

	private void ToggleHD()
	{
		int currentLevel = ((MVQualitySettings.CurrentLevel == 0) ? 1 : 0);
		MVQualitySettings.CurrentLevel = currentLevel;
	}

	public static string CreateHelpTxt()
	{
		string text = "\n";
		if (!MVGameControllerBase.IsTouristSession)
		{
			text += TM._("\nType: /c to see available chat commands.\n\n");
		}
		MVGameMode mVGameMode = MVGameControllerBase.GameMode;
		if (MVGameControllerBase.IsPlaying)
		{
			mVGameMode = MVGameMode.Play;
		}
		switch (mVGameMode)
		{
		case MVGameMode.CharacterEditor:
			return string.Empty;
		case MVGameMode.Edit:
			text += TM._("\n<Right mouse> Hold to look\n<WASD> Move\n<Scroll wheel> Move up and down\n<Shift> Hold to move fast\n");
			break;
		case MVGameMode.Play:
			text += TM._("<M> Menu");
			text += TM._("\n<H> Toggle HD Mode\n<WASD> Move\n<Space> Jump\n<K> Respawn\n<Left Mouse> Fire Weapon\n<Q> Holster equipped weapon\n<V> Drop equipped weapon");
			break;
		}
		return text;
	}

	public static string CreateChatCommandsHelpTxt()
	{
		string empty = string.Empty;
		empty += "\nType: /yes to nod your head.";
		empty += "\nType: /no to shake your head.";
		empty += "\nType: /wave to wave your arms.";
		empty += "\n\nType: /all to enter all chat. ";
		empty += "\nType: /team to enter team chat.";
		empty += "\nType: /say to enter say chat.";
		return empty + TM._("\n\nType: /hd to enable HD mode.\n");
	}

	private void SanitizeMessage(ref string message, string tagToSanitize)
	{
		bool flag = false;
		char[] array = message.ToLower().ToCharArray();
		int num = 0;
		int num2 = 0;
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i] == '<' && !flag)
			{
				num = i;
				flag = true;
			}
			if (array[i] == '>' && flag)
			{
				num2 = i + 1;
				flag = false;
				string text = new string(array, num, num2 - num);
				if (array[num + 1] != ' ' && text.Contains(tagToSanitize))
				{
					message = message.Remove(num, num2 - num);
					array = message.ToLower().ToCharArray();
					i -= num2 - num;
				}
			}
		}
	}

	private bool HandleChatSwapCommand(string message)
	{
		bool result = true;
		if (message.Equals("/all", StringComparison.OrdinalIgnoreCase))
		{
			SwapChat(MVGameMsgType.Chat);
			message = string.Empty;
			inputField.text = message;
		}
		else if (message.Equals("/team", StringComparison.OrdinalIgnoreCase))
		{
			SwapChat(MVGameMsgType.TeamChat);
			message = string.Empty;
			inputField.text = message;
		}
		else if (message.Equals("/say", StringComparison.OrdinalIgnoreCase))
		{
			SwapChat(MVGameMsgType.SayChat);
			message = string.Empty;
			inputField.text = message;
		}
		else
		{
			result = false;
		}
		return result;
	}

	private void SwapChat(MVGameMsgType newChat)
	{
		switch (newChat)
		{
		case MVGameMsgType.Chat:
			ActivateAllChat();
			break;
		case MVGameMsgType.TeamChat:
			ActivateTeamChat();
			break;
		case MVGameMsgType.SayChat:
			ActivateSayChat();
			break;
		}
	}

	private void ActivateAllChat()
	{
		if (selectedChat == MVGameMsgType.SayChat)
		{
			MVGameControllerBase.OperationRequests.SetSayChatBubbleVisible(shouldShow: false);
			isSayChatIconVisible = false;
		}
		currentChat.text = "[ All ]";
		currentChat.color = Color.white;
		selectedChat = MVGameMsgType.Chat;
	}

	private void ActivateTeamChat()
	{
		if (MVGameControllerBase.Game.TeamManager.TeamCount() >= 2)
		{
			if (selectedChat == MVGameMsgType.SayChat)
			{
				MVGameControllerBase.OperationRequests.SetSayChatBubbleVisible(shouldShow: false);
				isSayChatIconVisible = false;
			}
			currentChat.text = "[ Team ]";
			selectedChat = MVGameMsgType.TeamChat;
			ChangeTeamChatColor();
		}
	}

	private void ActivateSayChat()
	{
		if (selectedChat != MVGameMsgType.SayChat)
		{
			MVGameControllerBase.OperationRequests.SetSayChatBubbleVisible(shouldShow: true);
			isSayChatIconVisible = true;
		}
		currentChat.text = "[ Say ]";
		currentChat.color = sayChatColor;
		selectedChat = MVGameMsgType.SayChat;
	}

	private void ChangeTeamChatColor()
	{
		if (selectedChat == MVGameMsgType.TeamChat)
		{
			currentChat.color = Styles.GetTeamColor(MVGameControllerBase.LocalPlayer.Team);
		}
	}

	public void SetToNextChat()
	{
		MVGameMsgType nextChat = GetNextChat();
		SwapChat(nextChat);
	}

	private MVGameMsgType GetNextChat()
	{
		MVGameMsgType mVGameMsgType = selectedChat;
		mVGameMsgType++;
		if (mVGameMsgType > MVGameMsgType.SayChat)
		{
			mVGameMsgType = MVGameMsgType.Chat;
		}
		if (mVGameMsgType == MVGameMsgType.TeamChat && MVGameControllerBase.Game.TeamManager.TeamCount() < 2)
		{
			mVGameMsgType = MVGameMsgType.SayChat;
		}
		return mVGameMsgType;
	}

	public void OnInputFocusChange(bool isFocused)
	{
		if (selectedChat == MVGameMsgType.SayChat)
		{
			MVGameControllerBase.OperationRequests.SetSayChatBubbleVisible(isFocused);
			isSayChatIconVisible = isFocused;
		}
	}
}
