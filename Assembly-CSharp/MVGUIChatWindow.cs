using System;
using System.Collections;
using UnityEngine;

public class MVGUIChatWindow : UXViewScript
{
	private const float FADE_IN_TIME = 1f;

	private const float MESSAGE_STAY_TIME = 5f;

	private const float FADE_OUT_TIME = 0.5f;

	private const float ALPHA_FADE_TO = 0.7f;

	public UXWindow chatWindow;

	public UXScrollableBox chatLog;

	public UXGroup chatControlGroup;

	public UXTextField chatField;

	public UXTextButton chatButton;

	public MVGUIChatWindowLine chatLinePrefab;

	private int _lastMessageProfileID = -1;

	private bool _useAlternateChatColor;

	private bool _isShown;

	private bool _openedFromShortCut;

	private bool _fadingIn;

	private bool _isInitialized;

	private bool _enterDown;

	public void InitializeChat()
	{
		if (!_isInitialized)
		{
			MVNetworkGame game = MVGameController.Instance.Game;
			game.OnReceivedChatMessage = (MVNetworkGame.OnReceivedChatMessageDelegate)Delegate.Combine(game.OnReceivedChatMessage, new MVNetworkGame.OnReceivedChatMessageDelegate(OnChatMessage));
			InitializeChatFunctions();
			InitializeFocusHelpers();
			_isInitialized = true;
		}
		View.releaseFocusOnHide = true;
		View.Hide();
	}

	private void InitializeChatFunctions()
	{
		UXTextButton uXTextButton = chatButton;
		uXTextButton.OnClick = (UXBaseButton.OnClickDelegate)Delegate.Combine(uXTextButton.OnClick, (UXBaseButton.OnClickDelegate)(() =>
		{
			MVGameController.Instance.Game.SendChatMsg(chatField.Text);
			chatField.Text = string.Empty;
			if (_openedFromShortCut)
			{
				HideChat();
			}
			_enterDown = false;
		}));
	}

	private void InitializeFocusHelpers()
	{
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		UXTextButton uXTextButton = chatButton;
		uXTextButton.OnClick = (UXBaseButton.OnClickDelegate)Delegate.Combine(uXTextButton.OnClick, (UXBaseButton.OnClickDelegate)(() =>
		{
			View.ReleaseFocus();
		}));
		UXTextField uXTextField = chatField;
		uXTextField.OnFocusChange = (UXTextInputElement.OnFocusChangeDelegate)Delegate.Combine(uXTextField.OnFocusChange, new UXTextInputElement.OnFocusChangeDelegate(HandleFocusChange));
		BoxCollider val = ((Component)chatWindow).gameObject.AddComponent<BoxCollider>();
		UXMouseClickObject uXMouseClickObject = ((Component)chatWindow).gameObject.AddComponent<UXMouseClickObject>();
		uXMouseClickObject.OnMouseDown = (UXMouseClickObject.OnMouseDownDelegate)Delegate.Combine(uXMouseClickObject.OnMouseDown, (UXMouseClickObject.OnMouseDownDelegate)((UXMouseClickObject clickObject, Vector3 mousePositionWorld) => true));
		val.size = chatField.Size;
		val.center = ((Component)chatWindow).transform.InverseTransformPoint(((Component)chatField).transform.localPosition) + new Vector3(0f, 0f, 0.1f);
	}

	public override void OnHide()
	{
		chatWindow.SetVisible(visible: false);
		chatLog.SetVisible(visible: false);
		chatControlGroup.SetVisible(visible: false);
	}

	public override void OnShow()
	{
		chatWindow.SetVisible(visible: true);
		chatLog.SetVisible(visible: true);
		chatControlGroup.SetVisible(visible: true);
		chatWindow.SetAlpha(1f);
		chatLog.SetAlpha(1f);
		chatField.SetAlpha(1f);
		chatButton.SetAlpha(1f);
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
		else if ((Object)(object)UXFullscreenColliderBox.Instance != (Object)null)
		{
			UXFullscreenColliderBox.Instance.OnClick = null;
			UXFullscreenColliderBox.Instance.RemoveBlockingObject(chatWindow);
		}
	}

	public void ShowChat(bool openFromShortCut)
	{
		if (!View.isVisible || _fadingIn)
		{
			((MonoBehaviour)this).StopAllCoroutines();
			_enterDown = false;
			_fadingIn = false;
			_isShown = true;
			_openedFromShortCut = openFromShortCut;
			View.Show();
			chatField.ReleaseFocus();
		}
	}

	public void HideChat()
	{
		View.Hide();
		chatField.ReleaseFocus();
		_isShown = false;
	}

	private void Update()
	{
		if (_enterDown && (Input.GetKeyUp((KeyCode)13) || Input.GetKeyUp((KeyCode)271)) && chatField.HasFocus && !_fadingIn)
		{
			chatButton.FireOnClick();
		}
		if ((Input.GetKeyDown((KeyCode)13) || Input.GetKeyDown((KeyCode)271)) && chatField.HasFocus && !_fadingIn)
		{
			_enterDown = true;
		}
	}

	private void OnChatMessage(MVPlayer sender, string message)
	{
		if (!_isShown && !_fadingIn)
		{
			((MonoBehaviour)this).StartCoroutine("FadeShowMessage");
		}
		if (_lastMessageProfileID != -1 && _lastMessageProfileID != sender.ProfileID)
		{
			_useAlternateChatColor = !_useAlternateChatColor;
		}
		MVGUIChatWindowLine mVGUIChatWindowLine = Object.Instantiate((Object)(object)chatLinePrefab) as MVGUIChatWindowLine;
		mVGUIChatWindowLine.BuildLine(sender, message, _useAlternateChatColor);
		mVGUIChatWindowLine.SetAlpha((!View.isVisible || _fadingIn) ? 0f : 1f);
		chatLog.AddLine(mVGUIChatWindowLine);
		_lastMessageProfileID = sender.ProfileID;
	}

	private IEnumerator FadeShowMessage()
	{
		_fadingIn = true;
		View.Show();
		chatField.SetAlpha(0f);
		chatButton.SetAlpha(0f);
		yield return ((MonoBehaviour)this).StartCoroutine(pTween.To(1f, 0f, 1f, (float t) =>
		{
			chatWindow.SetAlpha(Mathf.Clamp(t, 0f, 0.7f));
			chatLog.SetAlpha(t);
		}));
		_isShown = true;
		yield return (object)new WaitForSeconds(5f);
		yield return ((MonoBehaviour)this).StartCoroutine(pTween.To(0.5f, 1f, 0f, (float t) =>
		{
			chatWindow.SetAlpha(Mathf.Clamp(t, 0f, 0.7f));
			chatLog.SetAlpha(t);
		}));
		View.Hide();
		_isShown = false;
		_fadingIn = false;
	}
}
