using System;
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
			if ((Object)(object)MVGameController.Instance == (Object)null || MVGameController.Instance.CharacterEditorController == null)
			{
				return null;
			}
			return MVGameController.Instance.CharacterEditorController.CurrentBody;
		}
	}

	public override void OnShow()
	{
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		base.OnShow();
		Debug.Log((object)"Show acc view");
		if (!_isInitialized)
		{
			DoInitialize();
			UXTabWindow uXTabWindow = tabs;
			uXTabWindow.OnExitButtonClick = (UXWindow.OnExitButtonClickDelegate)Delegate.Combine(uXTabWindow.OnExitButtonClick, new UXWindow.OnExitButtonClickDelegate(HideAvatarAccessoryView));
			UXTabWindow uXTabWindow2 = tabs;
			uXTabWindow2.OnTabSelect = (UXTabWindow.OnTabSelectedDelegate)Delegate.Combine(uXTabWindow2.OnTabSelect, new UXTabWindow.OnTabSelectedDelegate(OnTabsChange));
			windowedOffset = ((Component)tabs).transform.localPosition.x;
			uxScreen = UXUtils.FindGUIObjectOfType<UXScreen>();
			UXScreen uXScreen = uxScreen;
			uXScreen.OnFullScreenChange = (UXScreen.OnFullScreenChangeDelegate)Delegate.Combine(uXScreen.OnFullScreenChange, new UXScreen.OnFullScreenChangeDelegate(OnFullScreenChange));
			OnFullScreenChange(uxScreen.Fullscreen);
			_isInitialized = true;
		}
		tabs.SelectTab(currentTab);
	}

	private void OnFullScreenChange(bool full)
	{
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		if (full)
		{
			View.horizontalAnchor = UXHorizontal.Center;
			View.UpdatePlacement();
			((Component)tabs).transform.localPosition = Vector3.zero;
		}
		else
		{
			View.horizontalAnchor = UXHorizontal.Left;
			View.UpdatePlacement();
			((Component)tabs).transform.localPosition = Vector3.right * windowedOffset;
		}
	}

	public override void OnHide()
	{
		base.OnHide();
		tabs.GetTab(currentTab).Hide();
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
		MVGameController.Instance.CharacterEditorController.CloseAvatarAccessoryView();
	}
}
