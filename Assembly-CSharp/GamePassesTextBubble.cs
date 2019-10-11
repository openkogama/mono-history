using System;
using UnityEngine;
using UnityEngine.UI;

public class GamePassesTextBubble : MonoBehaviour
{
	[SerializeField]
	private NotificationFade fader;

	[SerializeField]
	private Text text;

	[SerializeField]
	private bool deactivateAfterFade;

	private bool isActive;

	public bool DeactivateAfterFade
	{
		set
		{
			deactivateAfterFade = value;
		}
	}

	public bool IsActive => isActive;

	private void Start()
	{
		NotificationFade notificationFade = fader;
		notificationFade.OnFinished = (Action)Delegate.Combine(notificationFade.OnFinished, new Action(OnFaderDone));
	}

	private void OnDestroy()
	{
		NotificationFade notificationFade = fader;
		notificationFade.OnFinished = (Action)Delegate.Remove(notificationFade.OnFinished, new Action(OnFaderDone));
	}

	private void OnFaderDone()
	{
		isActive = false;
		if (deactivateAfterFade)
		{
			gameObject.SetActive(value: false);
		}
	}

	public void Activate(string textBubbleText)
	{
		fader.Activate();
		text.text = textBubbleText;
		isActive = true;
	}
}
