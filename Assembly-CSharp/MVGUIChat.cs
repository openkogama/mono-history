using System;
using UnityEngine;

public class MVGUIChat : UXViewScript
{
	public delegate void ChatMessageSendDelegate(string message);

	public UXButton sendButton;

	public UXTextField messageTextField;

	private bool firstShow = true;

	public ChatMessageSendDelegate ChatMessageSend;

	public override void OnShow()
	{
		if (firstShow)
		{
			UXButton uXButton = sendButton;
			uXButton.OnClick = (UXButton.OnClickDelegate)Delegate.Combine(uXButton.OnClick, (UXButton.OnClickDelegate)(() =>
			{
				if (!messageTextField.Text.Equals(string.Empty) && ChatMessageSend != null)
				{
					ChatMessageSend(messageTextField.Text);
				}
				View.Hide();
			}));
			messageTextField.Initialize();
			firstShow = false;
		}
		((Component)sendButton).gameObject.SetActiveRecursively(true);
		messageTextField.Text = string.Empty;
		messageTextField.RequestFocus();
		((Component)messageTextField).gameObject.SetActiveRecursively(true);
	}

	public void Update()
	{
		if (MVInputWrapper.GetKeyUp((KeyCode)13) && View.isVisible)
		{
			sendButton.FireOnClick();
		}
	}

	public override void OnHide()
	{
		((Component)sendButton).gameObject.SetActiveRecursively(false);
		((Component)messageTextField).gameObject.SetActiveRecursively(false);
	}
}
