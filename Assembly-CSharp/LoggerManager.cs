using System;
using System.Collections.Generic;
using UnityEngine;

public class LoggerManager
{
	public class Logger : ILogger
	{
		private LoggerManager manager;

		private string name;

		public Logger(LoggerManager manager, string name)
		{
			this.manager = manager;
			this.name = name;
		}

		public void Log(string message)
		{
			manager.Log(name, message);
		}
	}

	private Dictionary<string, Logger> loggers = new Dictionary<string, Logger>();

	private HashSet<string> interestingLoggers = new HashSet<string>();

	private bool appendAll = true;

	private IAppender appender;

	private static LoggerManager instance;

	public static LoggerManager Instance
	{
		get
		{
			if (instance == null)
			{
				instance = new LoggerManager();
			}
			return instance;
		}
	}

	private LoggerManager()
	{
		appender = new UnityLogAppender();
		appendAll = Debug.isDebugBuild;
	}

	public ILogger GetLogger(Type type)
	{
		return GetLogger(type.Name);
	}

	public ILogger GetLogger(string name)
	{
		if (!loggers.TryGetValue(name, out var value))
		{
			value = new Logger(this, name);
			loggers.Add(name, value);
		}
		return value;
	}

	private void Log(string loggerName, string message)
	{
		if (appendAll || interestingLoggers.Contains(loggerName))
		{
			appender.Log(loggerName, message);
		}
	}
}
