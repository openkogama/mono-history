using System;
using System.Collections;
using System.Threading;
using MV.WorldObject.AntiCheat;
using UnityEngine;

public class HackingToolDetector : MonoBehaviour
{
	public enum ReportCategory
	{
		process,
		regKey,
		unknown,
		SIZE
	}

	public class HackingToolReport
	{
		public enum Kind
		{
			process,
			suspectProcess,
			regKey,
			suspectKey,
			debugLog
		}

		public Kind kind;

		public ApplicationDesc app;

		public ApplicationDesc.RegistryKey foundKey;

		public string exactFind;

		public HackingToolReport(ApplicationDesc app)
		{
			kind = Kind.process;
			this.app = app;
		}

		public HackingToolReport(ApplicationDesc app, string exactFind)
		{
			kind = Kind.suspectProcess;
			this.app = app;
			this.exactFind = exactFind;
		}

		public HackingToolReport(ApplicationDesc app, ApplicationDesc.RegistryKey foundKey)
		{
			kind = Kind.regKey;
			this.app = app;
			this.foundKey = foundKey;
		}

		public HackingToolReport(ApplicationDesc app, ApplicationDesc.RegistryKey foundKey, string exactFind)
		{
			kind = Kind.suspectKey;
			this.app = app;
			this.foundKey = foundKey;
			this.exactFind = exactFind;
		}
	}

	public static readonly string CheatWarning = TM._("Cheating/Hacking is not allowed and will cause a permanent, irrevocable ban.");

	public Action<HackingToolReport> onHackToolDetected;

	private BitArray alreadyReported = new BitArray(3);

	[SerializeField]
	[Tooltip("Scans per second.")]
	private float scanFrequency = 1f / 60f;

	private ApplicationDesc[] banList;

	private static HackingToolDetector instance = null;

	private bool _quitRequest;

	private object _quitLock = new object();

	public ThreadSafeQueue<HackingToolReport> detectedHackingTools = new ThreadSafeQueue<HackingToolReport>(2);

	private Thread scanThread = new Thread(Scan_Threaded);

	public static bool InstallTracesDetected => Instance.alreadyReported[1];

	public static bool ProcessDetected => Instance.alreadyReported[0];

	private static HackingToolDetector Instance => instance;

	private static float WaitTime => 1f / Instance.scanFrequency;

	private bool QuitRequest
	{
		get
		{
			lock (_quitLock)
			{
				return _quitRequest;
			}
		}
		set
		{
			lock (_quitLock)
			{
				_quitRequest = value;
			}
		}
	}

	private static void DebugLogToChat(string str)
	{
	}

	public void TemporaryReportHandler(HackingToolReport a)
	{
		ReportCategory reportCategory = ReportCategory.unknown;
		string text;
		switch (a.kind)
		{
		case HackingToolReport.Kind.process:
			reportCategory = ReportCategory.process;
			text = "Running process \"" + a.app.ExeCertSubjectName + "\" associated with \"" + a.app.ProgramName + "\" detected.";
			break;
		case HackingToolReport.Kind.suspectProcess:
			reportCategory = ReportCategory.process;
			text = "Running process \"" + a.app.ExeCertSubjectName + "\" associated with \"" + a.app.ProgramName + "\" detected as \"" + a.exactFind + "\"";
			break;
		case HackingToolReport.Kind.regKey:
			reportCategory = ReportCategory.regKey;
			text = "Registry key \"" + a.foundKey.Name + "\" associated with \"" + a.app.ProgramName + "\" detected.";
			break;
		case HackingToolReport.Kind.suspectKey:
			reportCategory = ReportCategory.regKey;
			text = "Registry key \"" + a.foundKey.Name + "\" associated with \"" + a.app.ProgramName + "\" detected as \"" + a.exactFind + "\"";
			break;
		case HackingToolReport.Kind.debugLog:
			return;
		default:
			text = "Report default label have been hit. Tampering with HackingToolDetector suspected.";
			DebugLogHandler.ReportError(text, string.Empty, LogType.Warning);
			break;
		}
		DebugLogToChat(text);
		if (!alreadyReported[(int)reportCategory])
		{
			DebugLogToChat("Previous log was its first of its kind.");
			Debug.Log(text);
			DebugLogHandler.ReportError("Potential cheat detected.", string.Empty, LogType.Warning);
			StatHatWrapper.Count("Cheat detected: " + reportCategory, 1);
			alreadyReported[(int)reportCategory] = true;
			if (reportCategory == ReportCategory.process)
			{
				Debug.Log("Application quit!");
				CheatHandling.CheatSoftwareRunningDetected();
			}
		}
	}

	public void InjectionDetectedCallback(string msg)
	{
		Debug.LogWarning("Injection detector: " + msg);
	}

	protected void Start()
	{
		if (instance == null)
		{
			instance = this;
			onHackToolDetected = TemporaryReportHandler;
		}
		else
		{
			Debug.LogWarning("There's already a HackingToolDetector present. Selfdestructing this.");
			UnityEngine.Object.Destroy(this);
		}
	}

	public static void Initialize(ApplicationDesc[] banList)
	{
		DebugLogToChat("Ban list received.");
		instance.banList = banList;
		instance.InitiateDetection();
	}

	private void InitiateDetection()
	{
		scanThread.Start();
		DebugLogToChat("Scan thread started.");
		StartCoroutine(HandleReports());
	}

	protected void OnDestroy()
	{
		QuitRequest = true;
		if (instance != null)
		{
			try
			{
				scanThread.Interrupt();
				scanThread.Join();
				Debug.Log("Hacking tool scan thread joined successfully.");
			}
			catch (Exception)
			{
				Debug.LogError("Hacking tool scan thread join failed.");
			}
		}
		instance = null;
	}

	private static void Scan_Threaded()
	{
		DateTime dateTime = DateTime.Now.ToUniversalTime();
		RegistryScanner.StartScan(Instance.banList);
		DateTime dateTime2 = DateTime.Now.ToUniversalTime();
		Debug.Log("Initial scan completed in " + (dateTime2 - dateTime).TotalSeconds + " seconds.");
		ProcessScanner.Initialize(Instance.banList);
		float waitTime = WaitTime;
		DateTime dateTime3 = DateTime.Now.ToUniversalTime().AddSeconds(0f - waitTime - 1f);
		while (!Instance.QuitRequest)
		{
			DateTime dateTime4 = DateTime.Now.ToUniversalTime();
			double totalSeconds = (dateTime4 - dateTime3).TotalSeconds;
			if (totalSeconds > (double)waitTime)
			{
				dateTime3 = dateTime3.AddSeconds(totalSeconds);
				ScanForForbiddenProcesses();
				Thread.Sleep((int)waitTime * 1000);
			}
		}
		ProcessScanner.Destroy();
	}

	private static void ScanForForbiddenProcesses()
	{
		Debug.Log("Scan cycle started.");
		DateTime dateTime = DateTime.Now.ToUniversalTime();
		ProcessScanner.StartScan(Instance.banList);
		DateTime dateTime2 = DateTime.Now.ToUniversalTime();
		Debug.Log("Scan cycle completed in " + (dateTime2 - dateTime).TotalSeconds + " seconds.");
	}

	private IEnumerator HandleReports()
	{
		float waitDuration = WaitTime;
		yield return new WaitForSeconds(1f);
		while (!QuitRequest)
		{
			while (detectedHackingTools.Count > 0)
			{
				HackingToolReport report = detectedHackingTools.Dequeue();
				onHackToolDetected(report);
			}
			yield return new WaitForSeconds(waitDuration);
		}
	}

	public static void Report(HackingToolReport report)
	{
		instance.detectedHackingTools.Enqueue(report);
	}
}
