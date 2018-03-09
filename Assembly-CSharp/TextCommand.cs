using System.Collections.Generic;
using System.Runtime.CompilerServices;
using MV.Common;

public static class TextCommand
{
	private const string kick = "/kick";

	private const string ban = "/ban";

	[CompilerGenerated]
	private static Dictionary<string, int> _003C_003Ef__switch_0024mapA;

	public static void Resolve(string commandLine)
	{
		string[] array = commandLine.Split(' ');
		string text = array[0];
		if (text != null)
		{
			if (_003C_003Ef__switch_0024mapA == null)
			{
				_003C_003Ef__switch_0024mapA = new Dictionary<string, int>(0);
			}
			if (!_003C_003Ef__switch_0024mapA.TryGetValue(text, out var _))
			{
			}
		}
		NotifyUser($"{array[0]} is not a valid command.");
	}

	private static void NotifyUser(string msg)
	{
		MVGameControllerBase.PostGameMsg(MVGameMsgType.AdminMsg, msg);
	}

	private static MVPlayer GetPlayer(string userName)
	{
		foreach (MVPlayer value in MVGameControllerBase.Game.MVPlayerContainer.Values)
		{
			if (value.Username == userName)
			{
				return value;
			}
		}
		return null;
	}
}
