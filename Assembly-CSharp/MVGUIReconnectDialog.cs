using System;
using UnityEngine;

public class MVGUIReconnectDialog : UXViewScript
{
	public delegate void OnReconnectDelegate();

	public delegate void OnDisconnectDelegate();

	public UXText messageText;

	public UXButton okButton;

	public UXButton cancelButton;

	public OnReconnectDelegate OnReconnect;

	public OnDisconnectDelegate OnDisconnect;

	public static MVGUIReconnectDialog New(string message, OnReconnectDelegate reconnectCallback, OnDisconnectDelegate disconnectCallback)
	{
		Object val = Object.Instantiate(Resources.Load("Prefabs/GUI/ReconnectDialog"));
		MVGUIReconnectDialog reconnectDialog = ((GameObject)((val is GameObject) ? val : null)).GetComponent<MVGUIReconnectDialog>();
		reconnectDialog.messageText.Text = message;
		reconnectDialog.Initialize();
		UXView uXView = reconnectDialog.View;
		uXView.OnHide = (UXView.OnHideDelegate)Delegate.Combine(uXView.OnHide, (UXView.OnHideDelegate)(() =>
		{
			UXFullscreenColliderBox.Instance.RemoveBlockingObject(reconnectDialog);
			MVGUIReconnectDialog mVGUIReconnectDialog3 = reconnectDialog;
			mVGUIReconnectDialog3.OnReconnect = (OnReconnectDelegate)Delegate.Remove(mVGUIReconnectDialog3.OnReconnect, reconnectCallback);
			MVGUIReconnectDialog mVGUIReconnectDialog4 = reconnectDialog;
			mVGUIReconnectDialog4.OnDisconnect = (OnDisconnectDelegate)Delegate.Remove(mVGUIReconnectDialog4.OnDisconnect, disconnectCallback);
			Object.Destroy((Object)(object)((Component)reconnectDialog).gameObject);
		}));
		UXButton uXButton = reconnectDialog.okButton;
		uXButton.OnClick = (UXButton.OnClickDelegate)Delegate.Combine(uXButton.OnClick, (UXButton.OnClickDelegate)(() =>
		{
			if (reconnectDialog.OnReconnect != null)
			{
				reconnectDialog.OnReconnect();
			}
			reconnectDialog.View.Hide();
		}));
		UXButton uXButton2 = reconnectDialog.cancelButton;
		uXButton2.OnClick = (UXButton.OnClickDelegate)Delegate.Combine(uXButton2.OnClick, (UXButton.OnClickDelegate)(() =>
		{
			if (reconnectDialog.OnDisconnect != null)
			{
				reconnectDialog.OnDisconnect();
			}
			reconnectDialog.View.Hide();
		}));
		UXFullscreenColliderBox.Instance.AddBlockingObject(reconnectDialog);
		MVGUIReconnectDialog mVGUIReconnectDialog = reconnectDialog;
		mVGUIReconnectDialog.OnReconnect = (OnReconnectDelegate)Delegate.Combine(mVGUIReconnectDialog.OnReconnect, reconnectCallback);
		MVGUIReconnectDialog mVGUIReconnectDialog2 = reconnectDialog;
		mVGUIReconnectDialog2.OnDisconnect = (OnDisconnectDelegate)Delegate.Combine(mVGUIReconnectDialog2.OnDisconnect, disconnectCallback);
		return reconnectDialog;
	}
}
