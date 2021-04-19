using System.Diagnostics;
using UnityEngine;

public class MVUtils : MonoBehaviour
{
	private float CheckProInterval = 5f;

	private void Awake()
	{
	}

	private void Update()
	{
		if (false || CheckPro())
		{
			Application.Quit();
		}
	}

	private bool CheckPro()
	{
		Process[] processes = Process.GetProcesses();
		foreach (Process process in processes)
		{
			if (process.ProcessName.ToLower().Contains("cheat") && process.ProcessName.ToLower().Contains("engine"))
			{
				return true;
			}
		}
		return false;
	}
}
