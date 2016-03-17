using UnityEngine;

public class ChatConsoleModeDef : MonoBehaviour
{
	[SerializeField]
	public ChatConsoleMode ChatConsoleMode;

	[SerializeField]
	private RectTransform rectTransform;

	public void Set(ref RectTransform targetRectTransform)
	{
		targetRectTransform.position = rectTransform.position;
	}
}
