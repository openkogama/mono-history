using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using Sentry;
using UnityEngine;
using UnityEngine.Networking;

public class SentrySdk : MonoBehaviour
{
	private static class RegionDatas
	{
		private struct RegionData(string regionKey, bool isEnabled)
		{
			public string regionKey = regionKey;

			public bool isEnabled = isEnabled;
		}

		private static Dictionary<string, RegionData> regionTagToRegionKeyMap = new Dictionary<string, RegionData>
		{
			{
				"local",
				new RegionData("https://fa4a258cdafc49bd9ad0ab6bd48a0f17@sentry.io/22239", isEnabled: true)
			},
			{
				"dev",
				new RegionData("https://662a317f4d044b22863ef9d1da030f16@sentry.io/22236", isEnabled: true)
			},
			{
				"test",
				new RegionData("https://f95c698753144f1e8b918d9193447479@sentry.io/22238", isEnabled: true)
			},
			{
				"friends",
				new RegionData("https://f4d3872cb35e41149c3fca2ea7b9bca5@sentry.io/22242", isEnabled: true)
			},
			{
				"br",
				new RegionData("br", isEnabled: false)
			},
			{
				"www",
				new RegionData("eu", isEnabled: false)
			}
		};

		public static string Key => regionTagToRegionKeyMap[MVGameControllerBase.KoGaMaSettings.RegionTag].regionKey;

		public static bool IsEnabled => regionTagToRegionKeyMap[MVGameControllerBase.KoGaMaSettings.RegionTag].isEnabled;
	}

	private float _timeLastError;

	private const float MinTime = 0.5f;

	private Breadcrumb[] _breadcrumbs;

	private int _lastBreadcrumbPos;

	private int _noBreadcrumbs;

	private string _lastErrorMessage = string.Empty;

	private Dsn _dsn;

	private bool _initialized;

	private bool sendDefaultPii = true;

	private bool Debug = true;

	private static SentrySdk _instance;

	public void Initialize()
	{
		if (!RegionDatas.IsEnabled)
		{
			UnityEngine.Debug.LogWarning("The client Sentry SDK is disabled for region.");
		}
		else if (_instance == null)
		{
			try
			{
				_dsn = new Dsn(RegionDatas.Key);
			}
			catch (Exception ex)
			{
				UnityEngine.Debug.LogError($"Error parsing DSN: {ex.Message}");
				return;
			}
			_breadcrumbs = new Breadcrumb[100];
			UnityEngine.Object.DontDestroyOnLoad(this);
			_instance = this;
			_initialized = true;
		}
		else
		{
			UnityEngine.Object.Destroy(this);
		}
	}

	public static void AddBreadcrumb(string message)
	{
		if (!(_instance == null))
		{
			_instance.DoAddBreadcrumb(message);
		}
	}

	public static void CaptureMessage(string message, Dictionary<string, object> extraSentryData, Dictionary<string, string> tags)
	{
		if (!(_instance == null))
		{
			_instance.DoCaptureMessage(message, extraSentryData, tags);
		}
	}

	public static void CaptureEvent(SentryEvent @event)
	{
		if (!(_instance == null))
		{
			_instance.DoCaptureEvent(@event);
		}
	}

	private void DoCaptureMessage(string message, Dictionary<string, object> extraSentryData, Dictionary<string, string> tags)
	{
		if (_instance.Debug)
		{
			UnityEngine.Debug.Log("sending message to sentry.");
		}
		SentryEvent sentryEvent = new SentryEvent(message, tags, extraSentryData, GetBreadcrumbs());
		sentryEvent.level = "info";
		SentryEvent sentryEvent2 = sentryEvent;
		DoCaptureEvent(sentryEvent2);
	}

	private void DoCaptureEvent(SentryEvent @event)
	{
		if (_instance.Debug)
		{
			UnityEngine.Debug.Log("sending event to sentry.");
		}
		StartCoroutine(ContinueSendingEvent(@event));
	}

	private void DoAddBreadcrumb(string message)
	{
		if (!_initialized)
		{
			UnityEngine.Debug.LogError("Cannot AddBreadcrumb if we are not initialized");
			return;
		}
		string timestamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH\\:mm\\:ss");
		_breadcrumbs[_lastBreadcrumbPos] = new Breadcrumb(timestamp, message);
		_lastBreadcrumbPos++;
		_lastBreadcrumbPos %= 100;
		if (_noBreadcrumbs < 100)
		{
			_noBreadcrumbs++;
		}
	}

	private List<Breadcrumb> GetBreadcrumbs()
	{
		return Breadcrumb.CombineBreadcrumbs(_breadcrumbs, _lastBreadcrumbPos, _noBreadcrumbs);
	}

	public void ScheduleError(string condition, string stackTrace, Dictionary<string, object> extraSentryData, Dictionary<string, string> tags)
	{
		if (_instance.Debug)
		{
			UnityEngine.Debug.Log("sending exception to sentry.");
			UnityEngine.Debug.LogFormat("condition. {0}", condition);
			UnityEngine.Debug.LogFormat("stackTrace. {0}", stackTrace);
		}
		SentryErrorEvent sentryErrorEvent = new SentryErrorEvent(condition, _instance.GetBreadcrumbs(), stackTrace, tags, extraSentryData);
		StartCoroutine(ContinueSendingEvent(sentryErrorEvent));
	}

	public void ScheduleException(string condition, string stackTrace, Dictionary<string, object> extraSentryData, Dictionary<string, string> tags)
	{
		if (_instance.Debug)
		{
			UnityEngine.Debug.Log("sending exception to sentry.");
			UnityEngine.Debug.LogFormat("condition. {0}", condition);
			UnityEngine.Debug.LogFormat("stackTrace. {0}", stackTrace);
		}
		List<StackTraceSpec> list = new List<StackTraceSpec>();
		string[] array = condition.Split(new char[1] { ':' }, 2);
		string exceptionType = array[0];
		string exceptionValue = array[1].Substring(1);
		foreach (StackTraceSpec stackTrace2 in GetStackTraces(stackTrace))
		{
			list.Add(stackTrace2);
		}
		SentryExceptionEvent sentryExceptionEvent = new SentryExceptionEvent(exceptionType, exceptionValue, _instance.GetBreadcrumbs(), list, tags, extraSentryData);
		StartCoroutine(ContinueSendingEvent(sentryExceptionEvent));
	}

	private static IEnumerable<StackTraceSpec> GetStackTraces(string stackTrace)
	{
		string[] stackList = stackTrace.Split('\n');
		for (int i = stackList.Length - 1; i >= 0; i--)
		{
			string item = stackList[i];
			if (item == string.Empty)
			{
				continue;
			}
			int closingParen = item.IndexOf(')');
			if (closingParen == -1)
			{
				continue;
			}
			string functionName;
			string filename;
			int lineNo;
			try
			{
				functionName = item.Substring(0, closingParen + 1);
				if (item.Length < closingParen + 6)
				{
					filename = string.Empty;
					lineNo = -1;
				}
				else if (item.Substring(closingParen + 1, 5) != " (at ")
				{
					UnityEngine.Debug.Log("failed parsing " + item);
					functionName = item;
					lineNo = -1;
					filename = string.Empty;
				}
				else
				{
					int num = item.LastIndexOf(':', item.Length - 1, item.Length - closingParen);
					if (closingParen == item.Length - 1)
					{
						filename = string.Empty;
						lineNo = -1;
					}
					else if (num == -1)
					{
						filename = item.Substring(closingParen + 6, item.Length - closingParen - 7);
						lineNo = -1;
					}
					else
					{
						filename = item.Substring(closingParen + 6, num - closingParen - 6);
						lineNo = Convert.ToInt32(item.Substring(num + 1, item.Length - 2 - num));
					}
				}
			}
			catch
			{
				continue;
			}
			bool inApp;
			if (filename == string.Empty || (filename[0] == '<' && filename[filename.Length - 1] == '>'))
			{
				filename = string.Empty;
				inApp = true;
				if (functionName.Contains("UnityEngine."))
				{
					inApp = false;
				}
			}
			else
			{
				inApp = filename.Contains("Assets/");
			}
			yield return new StackTraceSpec(filename, functionName, lineNo, inApp);
		}
	}

	public static void OnLogMessageReceived(string condition, string stackTrace, LogType type, Dictionary<string, object> extraSentryData, Dictionary<string, string> tags)
	{
		if (_instance == null || !_instance._initialized)
		{
			return;
		}
		_instance._lastErrorMessage = condition;
		if ((type == LogType.Error || type == LogType.Exception || type == LogType.Assert) && !(Time.time - _instance._timeLastError <= 0.5f))
		{
			_instance._timeLastError = Time.time;
			switch (type)
			{
			case LogType.Exception:
				_instance.ScheduleException(condition, stackTrace, extraSentryData, tags);
				break;
			case LogType.Error:
				_instance.ScheduleError(condition, stackTrace, extraSentryData, tags);
				break;
			}
		}
	}

	private void PrepareEvent(SentryEvent @event)
	{
		if (_instance.sendDefaultPii)
		{
			@event.contexts.device.name = SystemInfo.deviceName;
		}
	}

	private IEnumerator<UnityWebRequestAsyncOperation> ContinueSendingEvent<T>(T @event) where T : SentryEvent
	{
		PrepareEvent(@event);
		string s = JsonConvert.SerializeObject(@event);
		UnityEngine.Debug.Log(s);
		string sentryKey = _dsn.publicKey;
		string sentrySecret = _dsn.secretKey;
		string timestamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH\\:mm\\:ss");
		string authString = $"Sentry sentry_version=5,sentry_client=Unity0.1,sentry_timestamp={timestamp},sentry_key={sentryKey},sentry_secret={sentrySecret}";
		UnityWebRequest www = new UnityWebRequest(_dsn.callUri.ToString());
		www.method = "POST";
		www.SetRequestHeader("X-Sentry-Auth", authString);
		www.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(s));
		www.downloadHandler = new DownloadHandlerBuffer();
		yield return www.SendWebRequest();
		while (!www.isDone)
		{
			yield return null;
		}
		if (www.isNetworkError || www.isHttpError || www.responseCode != 200)
		{
			UnityEngine.Debug.LogWarning("error sending request to sentry: " + www.error);
		}
		else if (Debug)
		{
			UnityEngine.Debug.Log("Sentry sent back: " + www.downloadHandler.text);
		}
	}
}
