using UnityEngine;

public abstract class AIngameController
{
	protected bool uiShown = true;

	protected UXView currentView;

	protected ChatController chatController;

	private bool hd;

	private bool AllowHotkeys => UXUtils.FindGUIObjectOfType<UXFocusManager>().CurrentFocus == null && !UXUtils.UXDialogFactory.DialogOpen;

	public bool IsInitialized { get; private set; }

	public virtual bool WindowShown => currentView != null && currentView.isVisible;

	public virtual void Initialize()
	{
		chatController = new ChatController();
		ResolveGUIElements();
		UXUtils.FindGUIObjectOfType<MVGUIAvatarAccessoryExpirationHandler>().enabled = true;
		MVGameController.Game.CameraController.Init();
		if (!MVGameController.Game.IsTouristSession)
		{
			MVGameController.TimeReward.Init();
		}
		IsInitialized = true;
	}

	protected virtual void ResolveGUIElements()
	{
	}

	public virtual void Update()
	{
		MVInputWrapper.Update();
		AwayMonitor.Update();
	}

	public virtual void HandleInput()
	{
		if (AllowHotkeys)
		{
			HandleSharedHotKeys();
		}
	}

	private void HandleSharedHotKeys()
	{
		if (MVInputWrapper.GetBooleanControlUp(KogamaControls.ToggleFullScreen))
		{
			ToggleFullScreen();
		}
		if (MVInputWrapper.GetBooleanControlUp(KogamaControls.ToggleHD))
		{
			ToggleHD();
		}
		if (MVInputWrapper.GetBooleanControlDown(KogamaControls.TogglePlayerParticles))
		{
			MVBody body = MVGameController.WOCM.AvatarLocal.Body;
			body.AccessoryParticlesVisible = !body.AccessoryParticlesVisible;
		}
	}

	private void ToggleHD()
	{
		hd = !hd;
		if (hd)
		{
			MVQualitySettings.CurrentLevel = 1;
		}
		else
		{
			MVQualitySettings.CurrentLevel = 0;
		}
	}

	public void ToggleFullScreen()
	{
		UXScreen uXScreen = UXUtils.UXScreen;
		uXScreen.Fullscreen = !uXScreen.Fullscreen;
	}

	public virtual void RespawnAvatar()
	{
		if (MVGameController.Game.IsPlaying)
		{
			MVGameController.WOCM.AvatarLocal.Suicide();
		}
		else
		{
			MVGameController.WOCM.AvatarLocal.Respawn(toHiddenState: false);
		}
	}

	public void ShowSingleWindow(UXView view)
	{
		bool flag = currentView != null && currentView.isVisible;
		if (flag)
		{
			HideCurrentWindow();
		}
		if (view != currentView || !flag)
		{
			currentView = view;
			currentView.Show();
		}
		else
		{
			currentView = null;
		}
	}

	public void HideCurrentWindow()
	{
		if (currentView != null)
		{
			currentView.Hide();
		}
	}

	public static T FindGUIObjectOfType<T>(GameObject root) where T : Component
	{
		T[] componentsInChildren = root.GetComponentsInChildren<T>(includeInactive: true);
		if (componentsInChildren.Length == 0)
		{
			Debug.LogError("result.Length == 0");
			return (T)null;
		}
		return componentsInChildren[0];
	}
}
