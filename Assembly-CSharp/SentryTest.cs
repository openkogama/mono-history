using System;
using System.Collections.Generic;
using Sentry;
using UnityEngine;

public class SentryTest : MonoBehaviour
{
	private int _counter;

	public bool SendMessageToSentry;

	private void Awake()
	{
	}

	private void Update()
	{
		_counter++;
		if (_counter % 100 == 0)
		{
			SentrySdk.AddBreadcrumb("Frame number: " + _counter);
		}
		if (SendMessageToSentry)
		{
			SentrySdk.CaptureMessage("this is a message 2", new Dictionary<string, object> { { "Extra key", "Extra value" } }, new Dictionary<string, string> { { "Tag key", "Tag value" } });
			SendMessageToSentry = false;
		}
	}

	private new void SendMessage(string message)
	{
		switch (message)
		{
		case "exception":
			throw new DivideByZeroException();
		case "assert":
			break;
		case "message":
			SentrySdk.CaptureMessage("this is a message2", new Dictionary<string, object> { { "Extra key", "Extra value" } }, new Dictionary<string, string> { { "Tag key", "Tag value" } });
			break;
		case "event":
		{
			SentryEvent sentryEvent = new SentryEvent("Event message", new Dictionary<string, string>(), new Dictionary<string, object>());
			sentryEvent.level = "debug";
			SentryEvent sentryEvent2 = sentryEvent;
			SentrySdk.CaptureEvent(sentryEvent2);
			break;
		}
		}
	}
}
