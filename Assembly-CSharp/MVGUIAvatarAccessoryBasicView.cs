using System;
using MV.Common;
using UnityEngine;

public abstract class MVGUIAvatarAccessoryBasicView : UXViewScript
{
	public UXTabWindow tabs;

	private int currentTab;

	private UXScreen uxScreen;

	private float windowedOffset;

	private bool _isInitialized;

	protected MVBody AvatarBody
	{
		get
		{
			if (MVGameControllerLegacyUI.CharacterEditorController != null)
			{
				return MVGameControllerLegacyUI.CharacterEditorController.CurrentBody;
			}
			try
			{
				return MVGameControllerBase.Game.LocalPlayer.Avatar.Body;
			}
			catch (Exception)
			{
				return null;
			}
		}
	}

	public override void OnShow()
	{
		base.OnShow();
		Debug.Log("Show acc view");
		if (!_isInitialized)
		{
			DoInitialize();
			UXTabWindow uXTabWindow = tabs;
			uXTabWindow.OnExitButtonClick = (UXWindow.OnExitButtonClickDelegate)Delegate.Combine(uXTabWindow.OnExitButtonClick, new UXWindow.OnExitButtonClickDelegate(HideAvatarAccessoryView));
			UXTabWindow uXTabWindow2 = tabs;
			uXTabWindow2.OnTabSelect = (UXTabWindow.OnTabSelectedDelegate)Delegate.Combine(uXTabWindow2.OnTabSelect, new UXTabWindow.OnTabSelectedDelegate(OnTabsChange));
			windowedOffset = tabs.transform.localPosition.x;
			uxScreen = UXUtils.UXScreen;
			UXScreen uXScreen = uxScreen;
			uXScreen.OnFullScreenChange = (UXScreen.OnFullScreenChangeDelegate)Delegate.Combine(uXScreen.OnFullScreenChange, new UXScreen.OnFullScreenChangeDelegate(OnFullScreenChange));
			OnFullScreenChange(uxScreen.Fullscreen);
			_isInitialized = true;
		}
		tabs.SelectTab(currentTab);
		if (MVGameControllerBase.GameMode == MVGameMode.CharacterEditor)
		{
			MVGameControllerLegacyUI.CharacterEditorController.EditorStateMachine.Event = EditorEvent.CEAvatarAccessory;
		}
	}

	private void OnFullScreenChange(bool full)
	{
		if (full)
		{
			View.horizontalAnchor = UXHorizontal.Center;
			View.UpdatePlacement();
			tabs.transform.localPosition = Vector3.zero;
		}
		else
		{
			View.horizontalAnchor = UXHorizontal.Left;
			View.UpdatePlacement();
			tabs.transform.localPosition = Vector3.right * windowedOffset;
		}
	}

	public override void OnHide()
	{
		base.OnHide();
		tabs.GetTab(currentTab).Hide();
		if (MVGameControllerBase.GameMode == MVGameMode.CharacterEditor && MVGameControllerLegacyUI.CharacterEditorController.EditorStateMachine != null)
		{
			MVGameControllerLegacyUI.CharacterEditorController.EditorStateMachine.Event = EditorEvent.CERoam;
		}
	}

	public UXTabPane GetCurrentTab()
	{
		return tabs.GetTab(currentTab);
	}

	protected abstract void DoInitialize();

	private void OnTabsChange(int tab)
	{
		currentTab = tab;
	}

	private void HideAvatarAccessoryView()
	{
		UXUtils.FindGUIObjectOfType<AvatarAccessoryController>().CloseAvatarAccessoryView();
	}
}
