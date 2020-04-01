using System;
using UnityEngine;

public class ProxyLogHandler : ILogHandler
{
	public class LogFormatData : EventArgs
	{
		public LogType LogType;

		public UnityEngine.Object context;

		public string format;

		public object[] args;

		public string Message => string.Format(format, args);

		public LogFormatData(LogType logType, UnityEngine.Object context, string format, params object[] args)
		{
			LogType = logType;
			this.context = context;
			this.format = format;
			this.args = args;
		}
	}

	private ILogHandler defaultLogHandler = Debug.unityLogger.logHandler;

	public LogType filterLogTypeConsoleWrite = LogType.Log;

	public event EventHandler<LogFormatData> OnLogReceived;

	public ProxyLogHandler()
	{
		Debug.unityLogger.logHandler = this;
	}

	private bool isAllowed(LogType logType)
	{
		if (logType == LogType.Exception)
		{
			return true;
		}
		if (logType <= filterLogTypeConsoleWrite)
		{
			return true;
		}
		return false;
	}

	public void Disable()
	{
		Debug.unityLogger.logHandler = defaultLogHandler;
	}

	public void LogFormat(LogType logType, UnityEngine.Object context, string format, params object[] args)
	{
		if (isAllowed(logType))
		{
			defaultLogHandler.LogFormat(logType, context, format, args);
		}
		else if (OnLogReceived != null)
		{
			LogFormatData e = new LogFormatData(logType, context, format, args);
			OnLogReceived(this, e);
		}
	}

	public void LogException(Exception exception, UnityEngine.Object context)
	{
		defaultLogHandler.LogException(exception, context);
	}
}
