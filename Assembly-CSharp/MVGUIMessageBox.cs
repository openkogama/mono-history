using System;
using UnityEngine;

public class MVGUIMessageBox : UXViewScript
{
	public UXText messageText;

	public UXButton okButton;

	public static MVGUIMessageBox New(string message)
	{
		Object val = Object.Instantiate(Resources.Load("Prefabs/GUI/MessageBox"));
		MVGUIMessageBox messageBox = ((GameObject)((val is GameObject) ? val : null)).GetComponent<MVGUIMessageBox>();
		messageBox.messageText.Text = message;
		messageBox.Initialize();
		UXView uXView = messageBox.View;
		uXView.OnHide = (UXView.OnHideDelegate)Delegate.Combine(uXView.OnHide, (UXView.OnHideDelegate)(() =>
		{
			UXFullscreenColliderBox.Instance.RemoveBlockingObject(messageBox);
			Object.Destroy((Object)(object)((Component)messageBox).gameObject);
		}));
		UXButton uXButton = messageBox.okButton;
		uXButton.OnClick = (UXButton.OnClickDelegate)Delegate.Combine(uXButton.OnClick, (UXButton.OnClickDelegate)(() =>
		{
			messageBox.View.Hide();
		}));
		UXFullscreenColliderBox.Instance.AddBlockingObject(messageBox);
		return messageBox;
	}
}
