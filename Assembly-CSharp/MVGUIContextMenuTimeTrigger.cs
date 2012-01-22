using System;
using System.Collections;
using UnityEngine;

public class MVGUIContextMenuTimeTrigger : UXViewScript
{
	public UXTextField timeTextField;

	public UXButton okButton;

	public UXButton cancelButton;

	public static MVGUIContextMenuTimeTrigger New()
	{
		Object val = Object.Instantiate(Resources.Load("Prefabs/GUI/Box Settings Dialogs/ContextMenuTimeTrigger"));
		MVGUIContextMenuTimeTrigger menu = ((GameObject)((val is GameObject) ? val : null)).GetComponent<MVGUIContextMenuTimeTrigger>();
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
		timeTextField.Text = ((float)contextMenuSelectionWO.Data["time"]).ToString();
	}

	public void OkButtonOnClick()
	{
		string text = timeTextField.Text;
		foreach (char c in text)
		{
			if (!char.IsDigit(c) && c != '.')
			{
				MVGUIMessageBox.New("Please enter only digits\nin the time field!");
				return;
			}
		}
		MVWorldObjectClient contextMenuSelectionWO = MVGameController.Instance.EditorController.GetContextMenuSelectionWO();
		Hashtable hashtable = (Hashtable)contextMenuSelectionWO.Data.Clone();
		hashtable["time"] = Convert.ToSingle(timeTextField.Text);
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
