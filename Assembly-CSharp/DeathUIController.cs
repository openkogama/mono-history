using System;
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
		NotificationFade notificationFade = fader;
		notificationFade.OnFinished = (Action)Delegate.Combine(notificationFade.OnFinished, new Action(OnFadeFinished));
	}

	private void OnFadeFinished()
	{
		fader.gameObject.SetActive(value: false);
	}

	private void OnLocalAvatarKilled(string text)
	{
		deathReason.text = text;
		fader.gameObject.SetActive(value: true);
	}

	public void SetVisibility(bool visible)
	{
		gameObject.SetActive(visible);
	}
}
