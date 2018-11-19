using MV.Common;

public static class TextCommand
{
	private class Command
	{
		private string[] commandComponents;

		public string Name => commandComponents[0];

		public int ArgCount => commandComponents.Length - 1;

		private Command(string[] commandComponents)
		{
			this.commandComponents = commandComponents;
		}

		public string Arg(int i)
		{
			int num = i + 1;
			if (num < commandComponents.Length)
			{
				return commandComponents[num];
			}
			return string.Empty;
		}

		public static implicit operator Command(string commandLine)
		{
			return new Command(commandLine.Split(' '));
		}
	}

	public static void Resolve(string commandLine)
	{
		Command command = commandLine;
		switch (command.Name.ToLower())
		{
		case "/abctest":
		case "/assetbundlecachetest":
			Command_AssetBundleCacheTest(command);
			break;
		default:
			Command_Invalid(command);
			break;
		}
	}

	private static void Command_AssetBundleCacheTest(Command command)
	{
		if (command.ArgCount == 1)
		{
			string text = command.Arg(0);
			if (int.TryParse(text, out var result))
			{
				AssetBundleCacheTest.Run(result);
			}
			else
			{
				NotifyUser($"{text} is not a valid version number argument, for {command.Name}");
			}
		}
		else
		{
			NotifyUser($"Command failed. {command.Name} expects exactly one parameter.");
		}
	}

	private static void Command_Invalid(Command command)
	{
		NotifyUser($"{command.Name} is not a valid command.");
	}

	public static void NotifyUser(string msg)
	{
		MVGameControllerBase.PostGameMsg(MVGameMsgType.AdminMsg, msg);
	}
}
