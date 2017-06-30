using System;
using System.Collections;
using System.Threading;
using UnityEngine;

public class HackingToolDetector : MonoBehaviour
{
	private enum ReportCategory
	{
		process,
		regKey,
		unknown,
		SIZE
	}

	[Serializable]
	public struct ApplicationDesc
	{
		[Serializable]
		public struct RegistryKey
		{
			[SerializeField]
			private string name;

			[SerializeField]
			[Tooltip("If true; comparison will be done with Equals() instead of StartsWith().")]
			private bool strictComparison;

			public string Name
			{
				get
				{
					return name;
				}
				set
				{
					name = value;
				}
			}

			public bool StrictComparison => strictComparison;
		}

		[SerializeField]
		[Tooltip("")]
		private string programName;

		[SerializeField]
		[Tooltip("Process name")]
		private string processName;

		[SerializeField]
		[Tooltip("If true; comparison will be done with Equals() instead of Contains().")]
		private bool strictComparison;

		[SerializeField]
		public RegistryKey[] associatedRegistryKeys;

		public string ProgramName
		{
			get
			{
				return programName;
			}
			set
			{
				programName = value;
			}
		}

		public string ProcessName
		{
			get
			{
				return processName;
			}
			set
			{
				processName = value;
			}
		}

		public bool StrictComparison => strictComparison;
	}

	public class HackingToolReport
	{
		public enum Kind
		{
			process,
			suspectProcess,
			regKey,
			suspectKey
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

	public Action<HackingToolReport> onHackToolDetected;

	private BitArray alreadyReported = new BitArray(3);

	[Tooltip("Scans per second.")]
	[SerializeField]
	private float scanFrequency = 1f / 60f;

	[Tooltip("How many times per second the report queue is checked.")]
	[SerializeField]
	private float reportFrequency = 0.05f;

	[SerializeField]
	private ApplicationDesc[] banList;

	public static HackingToolDetector instance;

	private bool _quitRequest;

	private object _quitLock = new object();

	public ThreadSafeQueue<HackingToolReport> detectedHackingTools = new ThreadSafeQueue<HackingToolReport>(2);

	private Thread scanThread = new Thread(Scan_Threaded);

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

	public void TemporaryReportHandler(HackingToolReport a)
	{
		ReportCategory reportCategory = ReportCategory.unknown;
		string message;
		switch (a.kind)
		{
		case HackingToolReport.Kind.process:
			reportCategory = ReportCategory.process;
			message = "Running process \"" + a.app.ProcessName + "\" associated with \"" + a.app.ProgramName + "\" detected.";
			break;
		case HackingToolReport.Kind.suspectProcess:
			reportCategory = ReportCategory.process;
			message = "Running process \"" + a.app.ProcessName + "\" associated with \"" + a.app.ProgramName + "\" detected as \"" + a.exactFind + "\"";
			break;
		case HackingToolReport.Kind.regKey:
			reportCategory = ReportCategory.regKey;
			message = "Registry key \"" + a.foundKey.Name + "\" associated with \"" + a.app.ProgramName + "\" detected.";
			break;
		case HackingToolReport.Kind.suspectKey:
			reportCategory = ReportCategory.regKey;
			message = "Registry key \"" + a.foundKey.Name + "\" associated with \"" + a.app.ProgramName + "\" detected as \"" + a.exactFind + "\"";
			break;
		default:
			message = "Report default label have been hit. Tampering with HackingToolDetector suspected.";
			break;
		}
		if (!alreadyReported[(int)reportCategory])
		{
			Debug.Log(message);
			DebugLogHandler.ReportError("Potential cheat detected.", string.Empty, LogType.Warning);
			StatHatWrapper.Count("Cheat detected: " + reportCategory, 1);
			alreadyReported[(int)reportCategory] = true;
		}
		if (a.kind == HackingToolReport.Kind.process || a.kind == HackingToolReport.Kind.suspectProcess)
		{
			Debug.Log("Application quit!");
			StartCoroutine(Quit_Coroutine(5f));
		}
	}

	private IEnumerator Quit_Coroutine(float secondsDelay)
	{
		yield return new WaitForSeconds(secondsDelay);
		Application.Quit();
	}

	public void InjectionDetectedCallback(string msg)
	{
		Debug.LogWarning("Injection detector: " + msg);
	}

	protected void Start()
	{
		Debug.Log("Start");
		if (instance == null)
		{
			instance = this;
			onHackToolDetected = TemporaryReportHandler;
			Debug.Log("Init");
			scanThread.Start();
			StartCoroutine(HandleReports());
		}
		else
		{
			Debug.LogWarning("There's already a HackingToolDetector present. Selfdestructing this.");
			UnityEngine.Object.Destroy(this);
		}
	}

	protected void OnDestroy()
	{
		QuitRequest = true;
		if (instance != null)
		{
			try
			{
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
		RegistryScanner.StartScan(instance.banList);
		DateTime dateTime2 = DateTime.Now.ToUniversalTime();
		Debug.Log("Initial scan completed in " + (dateTime2 - dateTime).TotalSeconds + " seconds.");
		ProcessScanner.Initialize(instance.banList);
		float num = 1f / instance.scanFrequency;
		DateTime dateTime3 = DateTime.Now.ToUniversalTime().AddSeconds(0f - num - 1f);
		while (!instance.QuitRequest)
		{
			DateTime dateTime4 = DateTime.Now.ToUniversalTime();
			double totalSeconds = (dateTime4 - dateTime3).TotalSeconds;
			if (totalSeconds > (double)num)
			{
				dateTime3 = dateTime3.AddSeconds(totalSeconds);
				ScanForForbiddenProcesses();
			}
		}
		ProcessScanner.Destroy();
	}

	private static void Scan_NonThreaded()
	{
		ProcessScanner.Initialize(instance.banList);
		ScanForForbiddenProcesses();
		ProcessScanner.Destroy();
	}

	private static void ScanForForbiddenProcesses()
	{
		Debug.Log("Scan cycle started.");
		DateTime dateTime = DateTime.Now.ToUniversalTime();
		ProcessScanner.StartScan(instance.banList);
		DateTime dateTime2 = DateTime.Now.ToUniversalTime();
		Debug.Log("Scan cycle completed in " + (dateTime2 - dateTime).TotalSeconds + " seconds.");
	}

	private IEnumerator Scan_Coroutine()
	{
		float waitDuration = 1f / scanFrequency;
		RegistryScanner.StartScan(instance.banList);
		while (!QuitRequest)
		{
			ProcessScanner.StartScan(instance.banList);
			yield return new WaitForSeconds(waitDuration);
		}
	}

	private IEnumerator HandleReports()
	{
		yield return new WaitForSeconds(1f);
		float waitDuration = 1f / reportFrequency;
		while (!QuitRequest)
		{
			lock (detectedHackingTools.ObtainLockForMultiOps())
			{
				while (detectedHackingTools.Count > 0)
				{
					HackingToolReport report = detectedHackingTools.Dequeue();
					onHackToolDetected(report);
				}
			}
			yield return new WaitForSeconds(waitDuration);
		}
	}
}
