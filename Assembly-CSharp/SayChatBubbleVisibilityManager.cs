using System;
using System.Collections.Generic;

public static class SayChatBubbleVisibilityManager
{
	public static Action<int, bool> OnSayChatIndicatorVisibilityChange;

	public static Action<int, Dictionary<object, object>> OnSayChatMessageRecieved;

	public static Action<Dictionary<object, object>> OnSayChatMessageHeard;
}
