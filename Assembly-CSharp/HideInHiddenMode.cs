using System;
using UnityEngine;

public class HideInHiddenMode : MonoBehaviour
{
	private void Start()
	{
		if (!MVGameControllerBase.Game.LocalPlayer.IsReady)
		{
			MVPlayerContainer mVPlayerContainer = MVGameControllerBase.Game.MVPlayerContainer;
			mVPlayerContainer.OnLocalPlayerReady = (Action)Delegate.Combine(mVPlayerContainer.OnLocalPlayerReady, new Action(LateInitialize));
		}
		else
		{
			Initialize();
		}
	}

	private void OnAvatarStateChange(object state)
	{
		int num = (int)state;
		if ((num & 4) > 0)
		{
			gameObject.SetActive(value: false);
		}
		else
		{
			gameObject.SetActive(value: true);
		}
	}

	private void OnDestroy()
	{
		if (MVGameControllerBase.IsAlive && MVGameControllerBase.Game != null)
		{
			MVRuntimeDataVariable avatarModeTypeFlags = MVGameControllerBase.WOCM.AvatarLocal.avatarModeTypeFlags;
			avatarModeTypeFlags.OnChange = (MVRuntimeDataVariable.OnChangeDelegate)Delegate.Remove(avatarModeTypeFlags.OnChange, new MVRuntimeDataVariable.OnChangeDelegate(OnAvatarStateChange));
		}
	}

	private void Initialize()
	{
		MVRuntimeDataVariable avatarModeTypeFlags = MVGameControllerBase.WOCM.AvatarLocal.avatarModeTypeFlags;
		avatarModeTypeFlags.OnChange = (MVRuntimeDataVariable.OnChangeDelegate)Delegate.Combine(avatarModeTypeFlags.OnChange, new MVRuntimeDataVariable.OnChangeDelegate(OnAvatarStateChange));
		OnAvatarStateChange(MVGameControllerBase.WOCM.AvatarLocal.avatarModeTypeFlags.Value);
	}

	private void LateInitialize()
	{
		Initialize();
		MVPlayerContainer mVPlayerContainer = MVGameControllerBase.Game.MVPlayerContainer;
		mVPlayerContainer.OnLocalPlayerReady = (Action)Delegate.Remove(mVPlayerContainer.OnLocalPlayerReady, new Action(LateInitialize));
	}
}
