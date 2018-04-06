using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FlagHighScoreNotification : Notification
{
	[SerializeField]
	private Text timeText;

	[SerializeField]
	private Text userNameText;

	[SerializeField]
	private Image flagImage;

	[SerializeField]
	private NotificationFade fader;

	protected override NotificationLifetime Lifetime => NotificationLifetime.High;

	public override void Initialize(Dictionary<object, object> data)
	{
		int actorNr = (int)data[(byte)9];
		MVPlayer playerUnsafe = MVGameControllerBase.Game.MVPlayerContainer.GetPlayerUnsafe(actorNr);
		if (playerUnsafe != null)
		{
			base.Initialize(data);
			timeText.text = (string)data[(byte)1];
			userNameText.text = playerUnsafe.Username;
			fader.Activate();
			flagImage.color = Styles.GetTeamColor(MVGameControllerBase.Game.LocalPlayer.Team);
			NotificationFade notificationFade = fader;
			notificationFade.OnFinished = (Action)Delegate.Combine(notificationFade.OnFinished, new Action(DestroyNotification));
		}
	}

	private void DestroyNotification()
	{
		timeSinceStart = (float)Lifetime;
	}
}
