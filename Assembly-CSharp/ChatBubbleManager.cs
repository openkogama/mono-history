using System;

public static class ChatBubbleManager
{
	public static Action<string, int, ChatAnchor> OnShowChatBubble;

	public static void ShowChatBubble(string text, int anchorId, ChatAnchor chatBubbleAnchor)
	{
		if (OnShowChatBubble != null)
		{
			OnShowChatBubble(text, anchorId, chatBubbleAnchor);
		}
	}
}
