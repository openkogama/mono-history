using System;
using UnityEngine;

[RequireComponent(typeof(UXToggleIconButton))]
public class MVGUIFullscreenToggle : MonoBehaviour
{
	private UXScreen screen;

	public UXToggleIconButton FullScreenToggle => ((Component)this).GetComponent<UXToggleIconButton>();

	private void Awake()
	{
		screen = UXUtils.FindGUIObjectOfType<UXScreen>();
		FullScreenToggle.SetToggleState(screen.Fullscreen);
		UXToggleIconButton fullScreenToggle = FullScreenToggle;
		fullScreenToggle.OnToggle = (UXToggleIconButton.OnToggleDelegate)Delegate.Combine(fullScreenToggle.OnToggle, new UXToggleIconButton.OnToggleDelegate(HandleOnToggle));
		UXScreen uXScreen = screen;
		uXScreen.OnFullScreenChange = (UXScreen.OnFullScreenChangeDelegate)Delegate.Combine(uXScreen.OnFullScreenChange, (UXScreen.OnFullScreenChangeDelegate)((bool fullscreen) =>
		{
			FullScreenToggle.SetToggleState(fullscreen);
		}));
	}

	public void HandleOnToggle(bool fullscreen)
	{
		screen.Fullscreen = fullscreen;
	}
}
