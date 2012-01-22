using System;
using UnityEngine;

[RequireComponent(typeof(UXToggleIconButton))]
public class MVGUIFullscreenToggle : MonoBehaviour
{
	private UXScreen screen;

	private void Awake()
	{
		screen = Object.FindObjectOfType(typeof(UXScreen)) as UXScreen;
		UXToggleIconButton toggleButton = ((Component)this).GetComponent<UXToggleIconButton>();
		toggleButton.ToggleState = Screen.fullScreen;
		UXToggleIconButton uXToggleIconButton = toggleButton;
		uXToggleIconButton.OnToggle = (UXToggleIconButton.OnToggleDelegate)Delegate.Combine(uXToggleIconButton.OnToggle, new UXToggleIconButton.OnToggleDelegate(HandleOnToggle));
		UXScreen uXScreen = screen;
		uXScreen.OnFullScreenChange = (UXScreen.OnFullScreenChangeDelegate)Delegate.Combine(uXScreen.OnFullScreenChange, (UXScreen.OnFullScreenChangeDelegate)((bool fullscreen) =>
		{
			toggleButton.ToggleState = fullscreen;
		}));
	}

	public void HandleOnToggle(bool fullscreen)
	{
		screen.Fullscreen = fullscreen;
	}
}
