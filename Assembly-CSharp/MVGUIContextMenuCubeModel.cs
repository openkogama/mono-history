using System;
using UnityEngine;

public class MVGUIContextMenuCubeModel : UXViewScript
{
	public UXButton cloneButton;

	public UXButton addToInventory;

	public static MVGUIContextMenuCubeModel New()
	{
		Object val = Object.Instantiate(Resources.Load("Prefabs/GUI/ContextMenuCubeModel"));
		MVGUIContextMenuCubeModel menu = ((GameObject)((val is GameObject) ? val : null)).GetComponent<MVGUIContextMenuCubeModel>();
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

	public override void Awake()
	{
		UXButton uXButton = cloneButton;
		uXButton.OnClick = (UXButton.OnClickDelegate)Delegate.Combine(uXButton.OnClick, new UXButton.OnClickDelegate(CloneButtonOnClick));
	}

	public void CloneButtonOnClick()
	{
		View.Hide();
		MVCubeModelBase mVCubeModelBase = (MVCubeModelBase)MVGameController.Instance.EditorController.GetContextMenuSelectionWO();
		MVGameController.Instance.EditorController.RequestInstance(mVCubeModelBase.Pid, MVGameController.Instance.EditorController.GetContextMenuSelectionWO().GameObject);
	}
}
