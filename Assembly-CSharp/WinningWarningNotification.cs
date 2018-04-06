using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WinningWarningNotification : Notification
{
	[Serializable]
	private class WinninConditionImage
	{
		[SerializeField]
		public GameStatCounterType Key;

		[SerializeField]
		public Image Value;
	}

	[SerializeField]
	private Text scoreText;

	[SerializeField]
	private Text warningText;

	[SerializeField]
	private Text userNameText;

	[SerializeField]
	private NotificationFade fader;

	[SerializeField]
	private List<WinninConditionImage> winningConditionImages;

	protected override NotificationLifetime Lifetime => NotificationLifetime.High;

	public override void Initialize(Dictionary<object, object> data)
	{
		int actorNr = (int)data[(byte)9];
		GameStatCounterType winningConditionType = (GameStatCounterType)(byte)data[(byte)5];
		MVPlayer playerUnsafe = MVGameControllerBase.Game.MVPlayerContainer.GetPlayerUnsafe(actorNr);
		if (playerUnsafe != null)
		{
			base.Initialize(data);
			scoreText.text = (string)data[(byte)1];
			userNameText.text = playerUnsafe.Username;
			fader.Activate();
			SelectWinningConditionImage(winningConditionType, playerUnsafe);
			SetWarningText(winningConditionType);
			NotificationFade notificationFade = fader;
			notificationFade.OnFinished = (Action)Delegate.Combine(notificationFade.OnFinished, new Action(DestroyNotification));
		}
	}

	private void SelectWinningConditionImage(GameStatCounterType winningConditionType, MVPlayer player)
	{
		for (int i = 0; i < winningConditionImages.Count; i++)
		{
			if (winningConditionImages[i].Key == winningConditionType)
			{
				winningConditionImages[i].Value.gameObject.SetActive(value: true);
				winningConditionImages[i].Value.color = Styles.GetTeamColor(player.Team);
			}
			else
			{
				winningConditionImages[i].Value.gameObject.SetActive(value: false);
			}
		}
	}

	private void SetWarningText(GameStatCounterType winningConditionType)
	{
		switch (winningConditionType)
		{
		case GameStatCounterType.Collectible:
			warningText.text = "STARS LEFT!";
			break;
		case GameStatCounterType.Kill:
		case GameStatCounterType.OculusKill:
			warningText.text = "KILLS LEFT!";
			break;
		case GameStatCounterType.Flag:
		case GameStatCounterType.Time:
		case GameStatCounterType.FlagCaptured:
			break;
		}
	}

	private void DestroyNotification()
	{
		timeSinceStart = (float)Lifetime;
	}
}
