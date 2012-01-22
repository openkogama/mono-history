using System;
using UnityEngine;

public class Chat
{
	public delegate void OnActivateDelegate();

	public delegate void OnDeactivateDelegate();

	private MVGUIChat chat;

	private MVGUIChatToggle chatToggle;

	private MVGUIChatMessages chatMessages;

	public OnActivateDelegate OnActivate;

	public Func<bool> AllowActivate;

	public OnDeactivateDelegate OnDeactivate;

	public Chat()
	{
		chat = Object.FindObjectOfType(typeof(MVGUIChat)) as MVGUIChat;
		chatToggle = Object.FindObjectOfType(typeof(MVGUIChatToggle)) as MVGUIChatToggle;
		chatMessages = Object.FindObjectOfType(typeof(MVGUIChatMessages)) as MVGUIChatMessages;
		UXIconButton uXIconButton = chatToggle.chat;
		uXIconButton.OnClick = (UXIconButton.OnClickDelegate)Delegate.Combine(uXIconButton.OnClick, (UXIconButton.OnClickDelegate)(() =>
		{
			Activate();
		}));
		UXView view = chat.View;
		view.OnHide = (UXView.OnHideDelegate)Delegate.Combine(view.OnHide, new UXView.OnHideDelegate(HandleOnHideChatWindow));
		MVGUIChat mVGUIChat = chat;
		mVGUIChat.ChatMessageSend = (MVGUIChat.ChatMessageSendDelegate)Delegate.Combine(mVGUIChat.ChatMessageSend, (MVGUIChat.ChatMessageSendDelegate)((string message) =>
		{
			MVGameController.Instance.Game.SendChatMsg(message);
		}));
		MVNetworkGame game = MVGameController.Instance.Game;
		game.OnReceivedChatMessage = (MVNetworkGame.OnReceivedChatMessageDelegate)Delegate.Combine(game.OnReceivedChatMessage, (MVNetworkGame.OnReceivedChatMessageDelegate)((string sender, string message) =>
		{
			chatMessages.AddMessage(sender, message);
		}));
	}

	public void Show()
	{
		chatToggle.View.Show();
	}

	public void Hide()
	{
		chatToggle.View.Hide();
	}

	public void Activate()
	{
		if (AllowActivate == null || AllowActivate())
		{
			chat.View.Show();
			UXFullscreenColliderBox.Instance.AddBlockingObject(chat);
			UXFullscreenColliderBox.Instance.OnClick = () =>
			{
				chat.View.Hide();
			};
		}
	}

	public void Deactivate()
	{
		chat.View.Hide();
		if (OnDeactivate != null)
		{
			OnDeactivate();
		}
	}

	private void HandleOnHideChatWindow()
	{
		UXFullscreenColliderBox.Instance.RemoveBlockingObject(chat);
	}
}
