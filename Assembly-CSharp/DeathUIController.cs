using System;
using MV.Common;
using UnityEngine;
using UnityEngine.UI;

public class DeathUIController : MonoBehaviour
{
	[SerializeField]
	private Text deathReason;

	[SerializeField]
	private NotificationFade fader;

	private void Awake()
	{
		MVAvatarLocal avatarLocal = MVGameControllerBase.WOCM.AvatarLocal;
		avatarLocal.OnKilled = (Action<string>)Delegate.Combine(avatarLocal.OnKilled, new Action<string>(OnLocalAvatarKilled));
		MVRuntimeDataVariable avatarModeTypeFlags = MVGameControllerBase.WOCM.AvatarLocal.avatarModeTypeFlags;
		avatarModeTypeFlags.OnChange = (MVRuntimeDataVariable.OnChangeDelegate)Delegate.Combine(avatarModeTypeFlags.OnChange, new MVRuntimeDataVariable.OnChangeDelegate(OnAvatarStateChanged));
		NotificationFade notificationFade = fader;
		notificationFade.OnFinished = (Action)Delegate.Combine(notificationFade.OnFinished, new Action(OnFadeFinished));
	}

	private void OnAvatarStateChanged(object state)
	{
		AvatarModeTypes avatarModeTypes = (AvatarModeTypes)state;
		if ((avatarModeTypes & AvatarModeTypes.Hidden) != 0)
		{
			fader.Deactivate();
			fader.gameObject.SetActive(value: false);
		}
	}

	private void OnFadeFinished()
	{
		fader.gameObject.SetActive(value: false);
	}

	private void OnLocalAvatarKilled(string text)
	{
		deathReason.text = text;
		fader.gameObject.SetActive(value: true);
		fader.Activate();
	}
}
