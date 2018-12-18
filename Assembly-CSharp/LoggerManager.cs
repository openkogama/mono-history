using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class LoggerManager
{
	public class Logger : ILogger
	{
		private LoggerManager manager;

		private readonly string name;

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

	private static LoggerManager instance;

	private Dictionary<string, Logger> loggers = new Dictionary<string, Logger>();

	private HashSet<string> interestingLoggers = new HashSet<string>();

	private bool appendAll = true;

	private IAppender appender;

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

	public static void Destroy()
	{
		instance = null;
	}

	private void ApplySettingsFromIniFile()
	{
		appendAll = false;
		using FileStream stream = new FileStream(Application.dataPath + "/../../LogSetup.ini", FileMode.OpenOrCreate, FileAccess.Read);
		using StreamReader streamReader = new StreamReader(stream);
		string text = streamReader.ReadLine();
		if (text == null)
		{
			return;
		}
		string[] array = text.Split(',');
		foreach (string text2 in array)
		{
			if (text2.Length > 0)
			{
				interestingLoggers.Add(text2);
			}
			if (text2.Equals("*"))
			{
				appendAll = true;
			}
		}
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
