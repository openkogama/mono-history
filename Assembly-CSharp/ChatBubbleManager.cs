using System;
using LivelyChatBubbles;

public static class ChatBubbleManager
{
	public static Action<string, int, ChatAnchor> OnShowChatBubble;

	public static void ShowChatBubble(string text, int woid, ChatAnchor chatBubbleAnchor)
	{
		if (OnShowChatBubble != null)
		{
			OnShowChatBubble(text, woid, chatBubbleAnchor);
		}
	}
}
