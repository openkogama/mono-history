using System;
using UnityEngine;

public static class AwayMonitor
{
	private enum State
	{
		Active,
		InActive10Min,
		InActive15Min
	}

	private static DateTime latestResetAFKTime = DateTime.Now;

	private static readonly TimeSpan awayCheckFrequency = new TimeSpan(0, 0, 0, 59);

	private static readonly TimeSpan warningTimeSpan = new TimeSpan(0, 0, 10, 0);

	private static readonly TimeSpan idleKickTimeSpan = new TimeSpan(0, 0, 15, 0);

	private static State state = State.Active;

	private static bool idleKickEnabled = true;

	public static bool IdleKickEnabled
	{
		get
		{
			return idleKickEnabled;
		}
		set
		{
			idleKickEnabled = value;
		}
	}

	public static void Update()
	{
		if (DateTime.Now - MVInputWrapper.LatestMouseMoveTime < awayCheckFrequency && DateTime.Now - latestResetAFKTime > awayCheckFrequency)
		{
			BrowserComm.ToJavaScript.ExternalCall("resetAFKtimer");
			latestResetAFKTime = DateTime.Now;
		}
		if (IdleKickEnabled)
		{
			HandleIdle();
		}
	}

	private static void HandleIdle()
	{
		TimeSpan timeSpan = DateTime.Now - MVInputWrapper.LatestMouseMoveTime;
		if (timeSpan < warningTimeSpan)
		{
			state = State.Active;
		}
		else if (timeSpan > warningTimeSpan && state != State.InActive10Min)
		{
			UXUtils.FindGUIObjectOfType<MVGUIChatWindow>().AddLine("Idle. You will be kicked in 5 min.", Color.red);
			state = State.InActive10Min;
		}
		else if (timeSpan > idleKickTimeSpan && state != State.InActive15Min)
		{
			UXUtils.FindGUIObjectOfType<MVGUIChatWindow>().AddLine("Kicked. Idle for 15 min.", Color.red);
			MVGameController.ApplicationQuit(new QuitIdle());
			state = State.InActive15Min;
		}
	}
}
