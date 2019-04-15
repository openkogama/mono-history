using UnityEngine;
using UnityEngine.UI;

public class GamePassesTextBubble : MonoBehaviour
{
	[SerializeField]
	private NotificationFade fader;

	[SerializeField]
	private Text text;

	public void Activate(string textBubbleText)
	{
		fader.Activate();
		text.text = textBubbleText;
	}
}
