using System;
using UnityEngine;

public static class AwayMonitor
{
	private static DateTime latestResetAFKTime = DateTime.Now;

	private static readonly TimeSpan awayCheckFrequency = new TimeSpan(0, 0, 0, 59);

	public static void Update()
	{
		if (DateTime.Now - MVInputWrapper.LatestMouseMoveTime < awayCheckFrequency && DateTime.Now - latestResetAFKTime > awayCheckFrequency)
		{
			Application.ExternalCall("resetAFKtimer", new object[0]);
			latestResetAFKTime = DateTime.Now;
		}
	}
}
