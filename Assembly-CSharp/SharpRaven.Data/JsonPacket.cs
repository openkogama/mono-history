using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using UnityEngine;

namespace SharpRaven.Data;

public class JsonPacket
{
	[JsonProperty(PropertyName = "tags", NullValueHandling = NullValueHandling.Ignore)]
	public Dictionary<string, string> Tags;

	[JsonProperty(PropertyName = "extra", NullValueHandling = NullValueHandling.Ignore)]
	public object Extra;

	[JsonProperty(PropertyName = "event_id", NullValueHandling = NullValueHandling.Ignore)]
	public string EventID { get; set; }

	[JsonProperty(PropertyName = "project", NullValueHandling = NullValueHandling.Ignore)]
	public string Project { get; set; }

	[JsonProperty(PropertyName = "culprit", NullValueHandling = NullValueHandling.Ignore)]
	public string Culprit { get; set; }

	[JsonConverter(typeof(StringEnumConverter))]
	[JsonProperty(PropertyName = "level", NullValueHandling = NullValueHandling.Ignore)]
	public ErrorLevel Level { get; set; }

	[JsonProperty(PropertyName = "timestamp", NullValueHandling = NullValueHandling.Ignore)]
	public string TimeStamp { get; set; }

	[JsonProperty(PropertyName = "logger", NullValueHandling = NullValueHandling.Ignore)]
	public string Logger { get; set; }

	[JsonProperty(PropertyName = "platform", NullValueHandling = NullValueHandling.Ignore)]
	public string Platform { get; set; }

	[JsonProperty(PropertyName = "message", NullValueHandling = NullValueHandling.Ignore)]
	public string Message { get; set; }

	[JsonProperty(PropertyName = "server_name", NullValueHandling = NullValueHandling.Ignore)]
	public string ServerName { get; set; }

	[JsonProperty(PropertyName = "modules", NullValueHandling = NullValueHandling.Ignore)]
	public List<Module> Modules { get; set; }

	[JsonProperty(PropertyName = "sentry.interfaces.Exception", NullValueHandling = NullValueHandling.Ignore)]
	public SentryException Exception { get; set; }

	[JsonProperty(PropertyName = "sentry.interfaces.Stacktrace", NullValueHandling = NullValueHandling.Ignore)]
	public SentryStacktrace StackTrace { get; set; }

	public JsonPacket(string project)
	{
		Initialize();
		Project = project;
	}

	public JsonPacket(string project, Exception e)
	{
		Initialize();
		Message = e.Message;
		if (e.TargetSite != null)
		{
			Culprit = string.Format("{0} in {1}", (e.TargetSite.ReflectedType != null) ? e.TargetSite.ReflectedType.FullName : "<dynamic type>", e.TargetSite.Name);
		}
		Project = project;
		Level = ErrorLevel.error;
		Exception = new SentryException(e);
		Exception.Module = e.Source;
		Exception.Type = e.GetType().Name;
		Exception.Value = e.Message;
		StackTrace = new SentryStacktrace(e);
		if (StackTrace.Frames.Count == 0)
		{
			StackTrace = null;
		}
	}

	public JsonPacket(string project, string log, string stack, LogType logType)
	{
		Initialize();
		Message = log;
		Project = project;
		Level = ErrorLevel.error;
		Exception = new SentryException(log, stack, logType);
		Exception.Module = log;
		Exception.Type = logType.ToString();
		Exception.Value = stack;
		StackTrace = null;
	}

	private void Initialize()
	{
		TimeStamp = $"{DateTime.UtcNow.Year:D4}-{DateTime.UtcNow.Month:D2}-{DateTime.UtcNow.Day:D2}T{DateTime.UtcNow.Hour:D2}:{DateTime.UtcNow.Minute:D2}:{DateTime.UtcNow.Second:D2}.{DateTime.UtcNow.Millisecond:D7}Z";
		Logger = "root";
		Level = ErrorLevel.error;
		EventID = Guid.NewGuid().ToString().Replace("-", string.Empty);
		Project = "default";
		Platform = "csharp";
	}

	public string Serialize()
	{
		return JsonConvert.SerializeObject(this);
	}
}
