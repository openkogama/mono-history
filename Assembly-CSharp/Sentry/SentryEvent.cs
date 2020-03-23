using System;
using System.Collections.Generic;
using UnityEngine;

namespace Sentry;

[Serializable]
public class SentryEvent
{
	public string event_id;

	public string message;

	public string timestamp;

	public string logger;

	public string level;

	public string platform = "csharp";

	public string release;

	public Context contexts;

	public SdkVersion sdk = new SdkVersion();

	public List<Breadcrumb> breadcrumbs;

	public Dictionary<string, string> tags;

	public Dictionary<string, object> extra;

	public SentryEvent(string message, Dictionary<string, string> tags, Dictionary<string, object> extra, List<Breadcrumb> breadcrumbs = null)
	{
		event_id = Guid.NewGuid().ToString("N");
		this.message = message;
		timestamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH\\:mm\\:ss");
		level = "error";
		this.breadcrumbs = breadcrumbs;
		contexts = new Context();
		release = Application.version;
		this.tags = tags;
		this.extra = extra;
	}
}
