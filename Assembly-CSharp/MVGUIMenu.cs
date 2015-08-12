using System;
using UnityEngine;

public class MVGUIMenu : UXViewScript
{
	public MVGUIPlayersWindow playersWindow;

	private bool _shortcut;

	private bool _isInitialized;

	[SerializeField]
	private UXIconButton _exitButton;

	public override void OnInitialize()
	{
		UXIconButton exitButton = _exitButton;
		exitButton.OnClick = (UXBaseButton.OnClickDelegate)Delegate.Combine(exitButton.OnClick, (UXBaseButton.OnClickDelegate)(() =>
		{
			ToggleMenu();
		}));
	}

	public void ToggleMenu()
	{
		if (View.isVisible)
		{
			View.Hide();
		}
		else
		{
			View.Show();
		}
	}

	private void InitializeComponents()
	{
		playersWindow.InitializeListeners();
		_isInitialized = true;
	}

	public override void OnShow()
	{
		base.OnShow();
		if (!_isInitialized)
		{
			InitializeComponents();
		}
		UXFullscreenColliderBox.Instance.AddBlockingObject(this);
	}

	public override void OnHide()
	{
		base.OnHide();
		if (_isInitialized)
		{
			UXFullscreenColliderBox.Instance.RemoveBlockingObject(this);
		}
	}

	public void ShowOnShortcut(bool show)
	{
		if (show)
		{
			_shortcut = true;
			if (!View.isVisible)
			{
				View.Show();
			}
		}
		if (!show && _shortcut)
		{
			_shortcut = false;
			View.Hide();
		}
	}
}
