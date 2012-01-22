using UnityEngine;

public class UnityLogAppender : IAppender
{
	public void Log(string loggerName, string message)
	{
		Debug.Log((object)("[" + loggerName + "] " + message));
	}
}
