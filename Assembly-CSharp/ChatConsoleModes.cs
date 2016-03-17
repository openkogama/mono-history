using System.Collections.Generic;
using UnityEngine;

public class ChatConsoleModes : MonoBehaviour
{
	private ChatConsoleMode chatConsoleMode;

	[SerializeField]
	private List<ChatConsoleModeDef> chatConsoleModeDefs = new List<ChatConsoleModeDef>();

	public ChatConsoleMode ChatConsoleMode => chatConsoleMode;

	public void Set(ChatConsoleMode chatConsoleMode, ref RectTransform rectTransform)
	{
		this.chatConsoleMode = chatConsoleMode;
		foreach (ChatConsoleModeDef chatConsoleModeDef in chatConsoleModeDefs)
		{
			if (chatConsoleModeDef.ChatConsoleMode == chatConsoleMode)
			{
				chatConsoleModeDef.Set(ref rectTransform);
				break;
			}
		}
	}
}
