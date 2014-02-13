using System;
using UnityEngine;

public class MVGUIAvatarShopWindow : UXViewScript
{
	public MVGUIAvatarShopInventory avatarShopInventory;

	private UXWindow _window;

	private bool _isInitialized;

	private bool _beenReset;

	public override void OnShow()
	{
		base.OnShow();
		if (!_isInitialized)
		{
			InitializeAvatarShop();
		}
		if (_beenReset)
		{
			avatarShopInventory.InitializeAfterReset();
			_beenReset = false;
		}
		UXFullscreenColliderBox.Instance.AddBlockingObject(this);
	}

	public override void OnHide()
	{
		base.OnHide();
		UXFullscreenColliderBox.Instance.RemoveBlockingObject(this);
	}

	public void ResetAvatarShop()
	{
		avatarShopInventory.ResetInventoryGroup();
	}

	public override void OnInitialize()
	{
		_window = ((Component)this).gameObject.GetComponentInChildren<UXWindow>();
	}

	private void InitializeAvatarShop()
	{
		UXWindow window = _window;
		window.OnExitButtonClick = (UXWindow.OnExitButtonClickDelegate)Delegate.Combine(window.OnExitButtonClick, (UXWindow.OnExitButtonClickDelegate)(() =>
		{
			View.Hide();
		}));
		avatarShopInventory.Initialize();
		_isInitialized = true;
	}
}
