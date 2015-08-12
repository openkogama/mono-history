using System;
using System.Collections.Generic;
using MV.Common;
using UnityEngine;

public class MVGUIChatWindow : UXViewScript
{
	private enum State
	{
		Hidden,
		ShowWithChatControls,
		ShowWithOutChatControls
	}

	private const float ALPHA_FADE_TO = 0.5f;

	private const float MESSAGE_STAY_TIME = 10f;

	private State state;

	[SerializeField]
	private FadeTransition fadeTransition;

	public UXWindow chatWindow;

	public UXScrollableBox chatLog;

	public UXGroup chatControlGroup;

	public UXTextField chatField;

	public UXTextButton chatButton;

	public MVGUIChatWindowLine chatLinePrefab;

	private bool _isInitialized;

	private bool _enterDown;

	private string helpString = "/h";

	[SerializeField]
	private Color friendColor = new Color(122f, 209f, 122f);

	[SerializeField]
	private Color gameMessageColor = default;

	private bool canAutoHide = true;

	private bool retainControlAfterMessageSend;

	private float activatedTime = -10f;

	public bool CanAutoHide
	{
		set
		{
			canAutoHide = value;
		}
	}

	public void InitializeChat()
	{
		if (_isInitialized)
		{
			return;
		}
		View.releaseFocusOnHide = true;
		View.Hide();
		MVNetworkGame game = MVGameController.Game;
		game.OnReceivedGameMsg = (MVNetworkGame.OnReceivedGameMsgDelegate)Delegate.Combine(game.OnReceivedGameMsg, new MVNetworkGame.OnReceivedGameMsgDelegate(AddLine));
		InitializeChatFunctions();
		InitializeFocusHelpers();
		UXSlider uXSlider = chatLog.UXSlider;
		uXSlider.OnValueChangedIntermediate = (UXSlider.OnValueChangedIntermediateDelegate)Delegate.Combine(uXSlider.OnValueChangedIntermediate, new UXSlider.OnValueChangedIntermediateDelegate(OnSliderChange));
		activatedTime = Time.time - 10f;
		if (!LevelingManager.silentMode)
		{
			if (!LevelingManager.IsInitialized)
			{
				LevelingManager.OnLevelingInitialized = (LevelingManager.OnlevelingInitializedDelegate)Delegate.Combine(LevelingManager.OnLevelingInitialized, new LevelingManager.OnlevelingInitializedDelegate(OnLevelingInitialize));
			}
			else
			{
				OnLevelingInitialize();
			}
		}
		CreateStatus();
		_isInitialized = true;
	}

	public void KeepAlive()
	{
		View.Show();
	}

	private void OnSliderChange(UXSlider slider, float val)
	{
		View.Show();
	}

	public void AddLine(string line, Color color)
	{
		MVGUIChatWindowLine mVGUIChatWindowLine = UnityEngine.Object.Instantiate(chatLinePrefab);
		mVGUIChatWindowLine.BuildLine(line, color);
		AddLine(mVGUIChatWindowLine);
	}

	public void RemoveLine()
	{
		chatLog.RemoveLine(0);
	}

	private void CreateStatus()
	{
		CreateFriendsStatus();
		CreateHelpKey();
	}

	private void CreateHelpKey()
	{
		if (MVGameController.GameMode != MVGameMode.CharacterEditor && !MVGameController.Game.IsTouristSession)
		{
			string format = TM._("Type {0} for help\nPress <Enter> or <T> to chat");
			AddLine(string.Format(format, helpString), Color.grey);
		}
	}

	public void CreateHelpTxt()
	{
		string line = TM._("<M> Menu\n<H> Toggle HD Mode");
		switch (MVGameController.GameMode)
		{
		case MVGameMode.CharacterEditor:
			break;
		case MVGameMode.Edit:
		{
			AddLine(line, Color.grey);
			string line3 = TM._("\n\n<PgDown> Move Workplane Down\n<PgUp> Move Workplane Up\n<TAB> Show Players\n<P> Play Mode\n<P> Edit Mode\n<1> Edit Cube\n<2> Delete Cube\n<3> Paint Cube\n<G> Toggle Grid Snap Size\n");
			string line4 = TM._("<F> Toggle Workplane\n<H> Toggle Vanity Item\n<L> Toggle Show Logic Cubes\n<R> Change Cube Material\n<I> Open Inventory\n<N> Create New Model\n<V> Focus on selected object");
			AddLine(line3, Color.grey);
			AddLine(line4, Color.grey);
			break;
		}
		case MVGameMode.Play:
		{
			AddLine(line, Color.grey);
			string line2 = TM._("\n\n<WASD> Move\n<Space> Jump\n<K> Respawn\n<Left Mouse> Fire Weapon\n<Q> Drop currently equipped weapon");
			AddLine(line2, Color.grey);
			break;
		}
		}
	}

	private void CreateUsersOfLanguageStatus()
	{
		Dictionary<int, MVPlayer> onlineFriends = MVGameController.Game.Friends.GetOnlineFriends();
		string regionCode = MVGameController.Game.LocalPlayer.RegionCode;
		bool flag = false;
		foreach (KeyValuePair<int, MVPlayer> player in MVGameController.Game.Players)
		{
			if (player.Value.RegionCode == regionCode && !onlineFriends.ContainsKey(player.Key) && player.Value.ActorNr != MVGameController.Game.LocalPlayer.ActorNr)
			{
				if (!flag)
				{
					AddLine(TM._("Users from your country: "), friendColor);
				}
				flag = true;
				AddLine(player.Value.Username, friendColor);
			}
		}
	}

	private void CreateFriendsStatus()
	{
		Dictionary<int, MVPlayer> onlineFriends = MVGameController.Game.Friends.GetOnlineFriends();
		if (onlineFriends.Count <= 0)
		{
			return;
		}
		AddLine("Friends online:", friendColor);
		foreach (MVPlayer value in onlineFriends.Values)
		{
			AddLine(value.Username, friendColor);
		}
	}

	public override void OnHide()
	{
		chatWindow.SetVisible(visible: false);
		chatLog.SetVisible(visible: false);
		chatControlGroup.SetVisible(visible: false);
		state = State.Hidden;
	}

	public override void OnShow()
	{
		chatWindow.SetVisible(visible: true);
		chatLog.SetVisible(visible: true);
		if (state == State.ShowWithChatControls)
		{
			chatControlGroup.SetVisible(visible: true);
		}
		activatedTime = Time.time;
		fadeTransition.FadeIn(null);
	}

	public void ShowChat(bool takeFocus, bool retainControlAfterMessageSend)
	{
		this.retainControlAfterMessageSend = retainControlAfterMessageSend;
		if (takeFocus)
		{
			TakeFocus();
		}
		state = State.ShowWithChatControls;
		View.Show();
		_enterDown = false;
	}

	private void Update()
	{
		if (_enterDown && MVInputWrapper.GetBooleanControlUp(KogamaControls.ChatSendLine, forceKeyUse: true) && chatField.HasFocus)
		{
			chatButton.FireOnClick();
		}
		if (MVInputWrapper.GetBooleanControlDown(KogamaControls.ChatSendLine, forceKeyUse: true) && chatField.HasFocus)
		{
			_enterDown = true;
		}
		if (Time.time - activatedTime > 10f && canAutoHide && !chatField.HasFocus && View.isVisible && fadeTransition.GuiFadeState != GuiFadeState.FadeOut)
		{
			fadeTransition.FadeOut(OnFadedOut);
		}
	}

	private void OnFadedOut()
	{
		View.Hide();
		chatLog.SetSliderValue(100f);
	}

	private void AddLine(UXLine uxLine)
	{
		if (state == State.Hidden)
		{
			state = State.ShowWithOutChatControls;
		}
		uxLine.SetAlpha((!View.isVisible) ? 0f : 1f, string.Empty);
		chatLog.AddLine(uxLine);
		View.Show();
	}

	private void OnLevelingInitialize()
	{
		if (MVGameController.GameMode == MVGameMode.Play)
		{
			OnPlayModeLevelingEnabledChanged(LevelingManager.LevelingEnabled);
			LevelingManager.OnPlayModeLevelingEnabledChanged = (LevelingManager.OnPlayModeLevelingEnabledChangedDelegate)Delegate.Combine(LevelingManager.OnPlayModeLevelingEnabledChanged, new LevelingManager.OnPlayModeLevelingEnabledChangedDelegate(OnPlayModeLevelingEnabledChanged));
		}
		MVLocalPlayer localPlayer = MVGameController.Game.LocalPlayer;
		localPlayer.OnXPProgressData = (XPProgress.OnXPProgressDataDelegate)Delegate.Combine(localPlayer.OnXPProgressData, new XPProgress.OnXPProgressDataDelegate(OnXPProgressData));
	}

	private void OnXPProgressData(XPProgressData xpProgressData)
	{
		AddLine(xpProgressData.XPString, Color.yellow);
	}

	private void OnPlayModeLevelingEnabledChanged(bool activated)
	{
		if (activated)
		{
			string line = TM._("Leveling activated! Players in game: ") + MVGameController.Game.Players.Count;
			AddLine(line, Color.green);
		}
		else
		{
			string line = TM._("Leveling deactivated! Players in game: ") + MVGameController.Game.Players.Count;
			AddLine(line, Color.red);
		}
	}

	private void AddLine(MVGameMsgType msgType, Dictionary<object, object> message)
	{
		switch (msgType)
		{
		case MVGameMsgType.AvatarKilled:
			if (GameMessagesRepository.ShowKilledMessage(message))
			{
				string line = GameMessagesRepository.CreateKilledMessage(message);
				AddLine(line, gameMessageColor);
			}
			break;
		case MVGameMsgType.UserJoined:
		{
			MVGUIChatWindowLine mVGUIChatWindowLine2 = UnityEngine.Object.Instantiate(chatLinePrefab);
			int key = (int)message[(byte)0];
			MVPlayer mVPlayer = MVGameController.Game.Players[key];
			if (MVGameController.Game.Friends.IsFriend(mVPlayer.ProfileID))
			{
				if (msgType == MVGameMsgType.UserJoined)
				{
					string username = mVPlayer.Username;
					Color color = friendColor;
					mVGUIChatWindowLine2.BuildLine(string.Format("{0} {1}", username, TM._("joined the game")), color);
					GUIAudioBank.Instance.GetSound("toggle_on").Play();
				}
				AddLine(mVGUIChatWindowLine2);
			}
			break;
		}
		case MVGameMsgType.UserLeft:
		{
			MVGUIChatWindowLine mVGUIChatWindowLine = UnityEngine.Object.Instantiate(chatLinePrefab);
			string arg = (string)message[(byte)3];
			if ((bool)message[(byte)6])
			{
				mVGUIChatWindowLine.BuildLine(string.Format("{0} {1}", arg, TM._("left the game")), Color.grey);
				AddLine(mVGUIChatWindowLine);
			}
			break;
		}
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
		default:
			Debug.Log("GameMsg of type " + msgType.ToString() + " received...");
			break;
		}
	}

	private void AddChatLine(Dictionary<object, object> data)
	{
		string arg = (string)data[(byte)5];
		int key = (int)data[(byte)0];
		MVPlayer mVPlayer = MVGameController.Game.Players[key];
		arg = $"[{mVPlayer.Username}]: {arg}";
		if (MVGameController.Game.Friends.IsFriend(mVPlayer.ProfileID))
		{
			GUIAudioBank.Instance.GetSound("toggle_on").Play();
			AddLine(arg, friendColor);
		}
		else
		{
			AddLine(arg, Color.white);
		}
	}

	private void AddXPLine(byte xpId, int actorNumber)
	{
		string username = MVGameController.Game.Players[actorNumber].Username;
		string xPText = XPManager.GetXPText(xpId);
		string line = string.Format("{0} {1} {2}", username, TM._("got"), xPText);
		AddLine(line, Color.yellow);
	}

	private void InitializeChatFunctions()
	{
		UXTextButton uXTextButton = chatButton;
		uXTextButton.OnClick = (UXBaseButton.OnClickDelegate)Delegate.Combine(uXTextButton.OnClick, (UXBaseButton.OnClickDelegate)(() =>
		{
			if (!retainControlAfterMessageSend)
			{
				chatField.ReleaseFocus();
				chatControlGroup.Hide();
				state = State.ShowWithOutChatControls;
			}
			else
			{
				Debug.Log("Take focus");
				TakeFocus();
			}
			activatedTime = Time.time;
			_enterDown = false;
			if (chatField.Text != helpString && !MVGameController.Game.IsTouristSession)
			{
				SendChatMessage(chatField.Text);
			}
			else
			{
				CreateHelpTxt();
			}
			chatField.Text = string.Empty;
		}));
	}

	private void SendChatMessage(string chatMsg)
	{
		if (!(chatMsg == string.Empty))
		{
			if (chatMsg.Length > 256)
			{
				Debug.LogWarning("ChatMsg too long. Truncated to 256 chars!");
				chatMsg = chatMsg.Substring(0, 256);
			}
			MVGameController.Game.PostGameMsg(MVGameMsgType.Chat, new Dictionary<object, object>
			{
				{
					(byte)0,
					MVGameController.Game.LocalPlayer.ActorNr
				},
				{
					(byte)5,
					chatMsg
				}
			});
		}
	}

	private void InitializeFocusHelpers()
	{
		UXTextField uXTextField = chatField;
		uXTextField.OnFocusChange = (UXTextInputElement.OnFocusChangeDelegate)Delegate.Combine(uXTextField.OnFocusChange, new UXTextInputElement.OnFocusChangeDelegate(HandleFocusChange));
	}

	public void TakeFocus()
	{
		chatField.RequestFocus();
	}

	private void HandleFocusChange(bool hasFocus)
	{
		if (hasFocus)
		{
			UXFullscreenColliderBox.Instance.AddBlockingObject(chatWindow);
			UXFullscreenColliderBox.Instance.OnClick = () =>
			{
				View.ReleaseFocus();
			};
		}
		else if (UXFullscreenColliderBox.Instance != null)
		{
			UXFullscreenColliderBox.Instance.OnClick = null;
			UXFullscreenColliderBox.Instance.RemoveBlockingObject(chatWindow);
		}
	}

	public void HideChat()
	{
		View.Hide();
		chatField.ReleaseFocus();
	}
}
