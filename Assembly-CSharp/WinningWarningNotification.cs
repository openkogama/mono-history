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
		GameStatCounterType winningConditionType = (GameStatCounterType)data[(byte)5];
		int scoreLeft = (int)data[(byte)4];
		if (MVGameControllerBase.Game.MVPlayerContainer.TryGetValue(actorNr, out var player))
		{
			base.Initialize(data);
			scoreText.text = (string)data[(byte)1];
			userNameText.text = player.Username;
			fader.Activate();
			SelectWinningConditionImage(winningConditionType, player);
			SetWarningText(winningConditionType, scoreLeft);
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
			}
			else
			{
				winningConditionImages[i].Value.gameObject.SetActive(value: false);
			}
		}
	}

	private void SetWarningText(GameStatCounterType winningConditionType, int scoreLeft)
	{
		string text = string.Empty;
		switch (winningConditionType)
		{
		case GameStatCounterType.Collectible:
			text = "STAR";
			break;
		case GameStatCounterType.Kill:
		case GameStatCounterType.OculusKill:
			text = "KILL";
			break;
		}
		if (scoreLeft > 1)
		{
			text += "S";
		}
		text += " LEFT!";
		warningText.text = text;
	}

	private void DestroyNotification()
	{
		timeSinceStart = (float)Lifetime;
	}
}
