using System;
using UnityEngine;

public class MVGUIContextMenuLogicCube : UXViewScript
{
	public UXButton cloneButton;

	public UXButton resetButton;

	public static MVGUIContextMenuLogicCube New()
	{
		Object val = Object.Instantiate(Resources.Load("Prefabs/GUI/ContextMenuLogicCube"));
		MVGUIContextMenuLogicCube menu = ((GameObject)((val is GameObject) ? val : null)).GetComponent<MVGUIContextMenuLogicCube>();
		menu.Initialize();
		UXView uXView = menu.View;
		uXView.OnHide = (UXView.OnHideDelegate)Delegate.Combine(uXView.OnHide, (UXView.OnHideDelegate)(() =>
		{
			UXFullscreenColliderBox.Instance.RemoveBlockingObject(menu);
			UXFullscreenColliderBox.Instance.OnClick = null;
			MVGameController.Instance.EditorController.LeaveContextMenuState();
		}));
		UXFullscreenColliderBox.Instance.AddBlockingObject(menu);
		UXFullscreenColliderBox instance = UXFullscreenColliderBox.Instance;
		instance.OnClick = (Action)Delegate.Combine(instance.OnClick, (Action)(() =>
		{
			menu.View.Hide();
		}));
		return menu;
	}

	public new void Awake()
	{
		UXButton uXButton = cloneButton;
		uXButton.OnClick = (UXButton.OnClickDelegate)Delegate.Combine(uXButton.OnClick, new UXButton.OnClickDelegate(CloneButtonOnClick));
		UXButton uXButton2 = resetButton;
		uXButton2.OnClick = (UXButton.OnClickDelegate)Delegate.Combine(uXButton2.OnClick, new UXButton.OnClickDelegate(ResetButtonOnClick));
	}

	public void CloneButtonOnClick()
	{
		MVWorldObjectClient contextMenuSelectionWO = MVGameController.Instance.EditorController.GetContextMenuSelectionWO();
		MVGameController.Instance.EditorController.AddLogicObject(contextMenuSelectionWO.WorldObjectType, contextMenuSelectionWO.Data);
		View.Hide();
	}

	public void ResetButtonOnClick()
	{
		int contextMenuSelectionWOID = MVGameController.Instance.EditorController.GetContextMenuSelectionWOID();
		MVGameController.Instance.Game.ResetLogicChunk(contextMenuSelectionWOID);
		View.Hide();
	}
}
