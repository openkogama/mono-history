using System;
using System.Collections.Generic;

public static class ChatCommandManager
{
	private static Dictionary<ChatCommand, Action> chatCommandCallBackDictionary = new Dictionary<ChatCommand, Action>();

	public static Action GetChatCommandCallback(ChatCommand chatCommand)
	{
		if (!chatCommandCallBackDictionary.ContainsKey(chatCommand))
		{
			chatCommandCallBackDictionary.Add(chatCommand, null);
		}
		return chatCommandCallBackDictionary[chatCommand];
	}

	public static void UpdateChatCommandCallback(ChatCommand chatCommand, Action callback)
	{
		if (chatCommandCallBackDictionary.ContainsKey(chatCommand))
		{
			chatCommandCallBackDictionary[chatCommand] = callback;
		}
	}

	public static void ChatCommandActivated(ChatCommand chatCommand)
	{
		if (chatCommandCallBackDictionary.ContainsKey(chatCommand) && chatCommandCallBackDictionary[chatCommand] != null)
		{
			chatCommandCallBackDictionary[chatCommand]();
		}
	}
}
