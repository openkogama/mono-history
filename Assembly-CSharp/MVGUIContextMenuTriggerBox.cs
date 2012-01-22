using System;
using System.Collections;
using UnityEngine;

public class MVGUIContextMenuTriggerBox : UXViewScript
{
	public UXToggle onceToggle;

	public UXButton okButton;

	public UXButton cancelButton;

	public static MVGUIContextMenuTriggerBox New()
	{
		Object val = Object.Instantiate(Resources.Load("Prefabs/GUI/Box Settings Dialogs/ContextMenuTriggerBox"));
		MVGUIContextMenuTriggerBox menu = ((GameObject)((val is GameObject) ? val : null)).GetComponent<MVGUIContextMenuTriggerBox>();
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
		UXToggle uXToggle = onceToggle;
		uXToggle.OnToggle = (UXToggle.OnToggleDelegate)Delegate.Combine(uXToggle.OnToggle, new UXToggle.OnToggleDelegate(OnceToggleOnToggle));
		MVWorldObjectClient contextMenuSelectionWO = MVGameController.Instance.EditorController.GetContextMenuSelectionWO();
		onceToggle.On = (bool)contextMenuSelectionWO.Data["once"];
	}

	public void OkButtonOnClick()
	{
		MVWorldObjectClient contextMenuSelectionWO = MVGameController.Instance.EditorController.GetContextMenuSelectionWO();
		Hashtable hashtable = (Hashtable)contextMenuSelectionWO.Data.Clone();
		hashtable["once"] = onceToggle.On;
		MVGameController.Instance.Game.UpdateWorldObjectData(contextMenuSelectionWO.Id, hashtable);
		View.Hide();
	}

	public void CancelButtonOnClick()
	{
		View.Hide();
	}

	public void OnceToggleOnToggle(UXToggle toggle)
	{
	}
}
