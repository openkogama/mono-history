using UnityEngine;
using UnityEngine.Events;

public static class FullScreenController
{
	private static int screenWidthBeforeFullscreen = 940;

	private static int screenHeightBeforeFullscreen = 482;

	private static bool fullScreen;

	private static bool waitingForFullscreenChange;

	private static bool fullscreenStatCollected;

	public static UnityAction<bool> OnFullScreenChange;

	public static bool FullScreen
	{
		get
		{
			return fullScreen;
		}
		set
		{
			if (value == fullScreen || !AllowFullscreenChange())
			{
				return;
			}
			waitingForFullscreenChange = true;
			fullScreen = value;
			if (fullScreen)
			{
				Screen.SetResolution(Screen.currentResolution.width, Screen.currentResolution.height, fullscreen: true);
				if (!fullscreenStatCollected)
				{
					StatHatWrapper.Count("FullscreenActivated", 1);
					fullscreenStatCollected = true;
				}
			}
			else
			{
				Screen.fullScreen = false;
				Screen.SetResolution(screenWidthBeforeFullscreen, screenHeightBeforeFullscreen, fullscreen: false);
			}
		}
	}

	public static void Init(int width, int height)
	{
		screenWidthBeforeFullscreen = width;
		screenHeightBeforeFullscreen = height;
	}

	public static void LateUpdate()
	{
		if (waitingForFullscreenChange && (!fullScreen || (Screen.currentResolution.width == Screen.width && Screen.currentResolution.height == Screen.height)) && Screen.fullScreen == fullScreen)
		{
			Debug.Log("Switched to fullscreen " + fullScreen);
			if (OnFullScreenChange != null)
			{
				OnFullScreenChange(Screen.fullScreen);
			}
			waitingForFullscreenChange = false;
		}
	}

	public static bool AllowFullscreenChange()
	{
		return !(BrowserComm.BrowserName == "Microsoft Internet Explorer") || BrowserComm.BrowserVersion < 8;
	}
}
