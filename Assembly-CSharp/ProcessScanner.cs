using System;
using System.Runtime.InteropServices;
using MV.WorldObject.AntiCheat;
using UnityEngine;

public static class ProcessScanner
{
	[DllImport("NativeFuncs")]
	private static extern void AddToBanList(string certificateSerialNumber, bool strictComparison);

	[DllImport("NativeFuncs")]
	private static extern int ScanForForbiddenProcesses();

	[DllImport("NativeFuncs")]
	private static extern void Cleanup();

	[DllImport("NativeFuncs", CharSet = CharSet.Unicode)]
	[return: MarshalAs(UnmanagedType.LPStr)]
	public static extern string GetLastExactFind();

	public static void Initialize(ApplicationDesc[] banList)
	{
		for (int i = 0; i < banList.Length; i++)
		{
			try
			{
				AddToBanList(banList[i].ExeCertSubjectName, banList[i].StrictComparison);
			}
			catch (Exception exception)
			{
				Debug.LogException(exception);
			}
		}
	}

	public static void Destroy()
	{
		Cleanup();
	}

	public static void StartScan(ApplicationDesc[] banList)
	{
		try
		{
			int num = ScanForForbiddenProcesses();
			if (num >= 0)
			{
				if (banList[num].StrictComparison)
				{
					HackingToolDetector.Report(new HackingToolDetector.HackingToolReport(banList[num]));
				}
				else
				{
					HackingToolDetector.Report(new HackingToolDetector.HackingToolReport(banList[num], GetLastExactFind()));
				}
				return;
			}
			switch (num)
			{
			case -1:
				break;
			case -2:
				Debug.LogError("NativeFunc error: eError_CreateToolhelp32SnapshotFailed");
				break;
			case -3:
				Debug.LogError("NativeFunc error: eError_Process32FirstFailed");
				break;
			default:
				Debug.LogError("NativeFunc error code: UNKNOWN");
				break;
			}
		}
		catch (Exception exception)
		{
			Debug.LogException(exception);
		}
	}
}
