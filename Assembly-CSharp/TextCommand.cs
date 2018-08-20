using MV.Common;

public static class TextCommand
{
	private const string kick = "/kick";

	private const string ban = "/ban";

	public static void Resolve(string commandLine)
	{
		string[] array = commandLine.Split(' ');
		_ = array[0];
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
