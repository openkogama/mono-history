using System;
using UnityEngine;
using UnityEngine.UI;

public class FirstTimeEventMessage : MonoBehaviour
{
	[SerializeField]
	private FirstTimeFadeHandler fader;

	[SerializeField]
	private Text message;

	public void FadeIn()
	{
		fader.StartFadeIn();
	}

	public void FadeOut(Action<GameObject> onFinished)
	{
		fader.StartFadeOut(onFinished, gameObject);
	}

	public void SetText(string messageText)
	{
		message.text = messageText;
	}
}
