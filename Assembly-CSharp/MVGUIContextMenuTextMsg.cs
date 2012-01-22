using System;
using System.Collections;
using UnityEngine;

public class MVGUIContextMenuTextMsg : UXViewScript
{
	public UXTextField textField;

	public UXButton okButton;

	public UXButton cancelButton;

	public static MVGUIContextMenuTextMsg New()
	{
		Object val = Object.Instantiate(Resources.Load("Prefabs/GUI/Box Settings Dialogs/ContextMenuTextMsg"));
		MVGUIContextMenuTextMsg menu = ((GameObject)((val is GameObject) ? val : null)).GetComponent<MVGUIContextMenuTextMsg>();
		menu.Initialize();
		UXView uXView = menu.View;
		uXView.OnHide = (UXView.OnHideDelegate)Delegate.Combine(uXView.OnHide, (UXView.OnHideDelegate)(() =>
		{
			UXFullscreenColliderBox.Instance.RemoveBlockingObject(menu);
			UXFullscreenColliderBox.Instance.OnClick = null;
			MVGameController.Instance.EditorController.LeaveContextMenuState();
		}));
		UXFullscreenColliderBox.Instance.AddBlockingObject(menu);
		return menu;
	}

	public new void Awake()
	{
		UXButton uXButton = okButton;
		uXButton.OnClick = (UXButton.OnClickDelegate)Delegate.Combine(uXButton.OnClick, new UXButton.OnClickDelegate(OkButtonOnClick));
		UXButton uXButton2 = cancelButton;
		uXButton2.OnClick = (UXButton.OnClickDelegate)Delegate.Combine(uXButton2.OnClick, new UXButton.OnClickDelegate(CancelButtonOnClick));
		MVWorldObjectClient contextMenuSelectionWO = MVGameController.Instance.EditorController.GetContextMenuSelectionWO();
		textField.Text = (string)contextMenuSelectionWO.Data["text"];
	}

	public void OkButtonOnClick()
	{
		MVWorldObjectClient contextMenuSelectionWO = MVGameController.Instance.EditorController.GetContextMenuSelectionWO();
		Hashtable hashtable = (Hashtable)contextMenuSelectionWO.Data.Clone();
		hashtable["text"] = textField.Text;
		MVGameController.Instance.Game.UpdateWorldObjectData(contextMenuSelectionWO.Id, hashtable);
		View.Hide();
	}

	public void CancelButtonOnClick()
	{
		View.Hide();
	}
}
