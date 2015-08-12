using System;
using UnityEngine;

public class MVGUIFullscreenToggle : UXViewScript
{
	private UXScreen screen;

	[SerializeField]
	private UXToggleIconButton fullscreenButton;

	public override void OnInitialize()
	{
		base.OnInitialize();
		screen = UXUtils.UXScreen;
		fullscreenButton.SetToggleState(screen.Fullscreen);
		UXToggleIconButton uXToggleIconButton = fullscreenButton;
		uXToggleIconButton.OnToggle = (UXToggleIconButton.OnToggleDelegate)Delegate.Combine(uXToggleIconButton.OnToggle, new UXToggleIconButton.OnToggleDelegate(HandleOnToggle));
		UXScreen uXScreen = screen;
		uXScreen.OnFullScreenChange = (UXScreen.OnFullScreenChangeDelegate)Delegate.Combine(uXScreen.OnFullScreenChange, (UXScreen.OnFullScreenChangeDelegate)((bool fullscreen) =>
		{
			fullscreenButton.SetToggleState(fullscreen);
		}));
	}

	public void HandleOnToggle(bool fullscreen)
	{
		screen.Fullscreen = fullscreen;
	}
}
