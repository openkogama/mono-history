using System;
using MV.Common;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

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

	private static Vector3 prevMousePos = default;

	private static DateTime latestMouseMoveTime = DateTime.Now;

	private static bool allAxisAvailable;

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

	public static DateTime LatestMouseMoveTime => latestMouseMoveTime;

	public static void Update()
	{
		if (allAxisAvailable)
		{
			UpdateMouse();
		}
		else if (CrossPlatformInputManager.AxisExists("Mouse ScrollWheel") && CrossPlatformInputManager.AxisExists("Mouse X") && CrossPlatformInputManager.AxisExists("Mouse Y"))
		{
			allAxisAvailable = true;
		}
		if (DateTime.Now - LatestMouseMoveTime < awayCheckFrequency && DateTime.Now - latestResetAFKTime > awayCheckFrequency)
		{
			BrowserComm.ToJavaScript.ExternalCall("resetAFKtimer");
			latestResetAFKTime = DateTime.Now;
		}
		if (IdleKickEnabled)
		{
			HandleIdle();
		}
	}

	private static void UpdateMouse()
	{
		if (Input.mousePosition != prevMousePos || MVInputWrapper.GetAxisRaw("Mouse ScrollWheel") > Mathf.Epsilon || MVInputWrapper.GetAxisRaw("Mouse X") > Mathf.Epsilon || MVInputWrapper.GetAxisRaw("Mouse Y") > Mathf.Epsilon)
		{
			prevMousePos = Input.mousePosition;
			latestMouseMoveTime = DateTime.Now;
		}
	}

	private static void HandleIdle()
	{
		TimeSpan timeSpan = DateTime.Now - LatestMouseMoveTime;
		if (timeSpan < warningTimeSpan)
		{
			state = State.Active;
		}
		else if (timeSpan > warningTimeSpan && state != State.InActive10Min)
		{
			MVGameControllerBase.PostGameMsg(MVGameMsgType.AdminMsg, "Idle.You will be kicked in 5 min.");
			state = State.InActive10Min;
		}
		else if (timeSpan > idleKickTimeSpan && state != State.InActive15Min)
		{
			MVGameControllerBase.PostGameMsg(MVGameMsgType.AdminMsg, "Kicked. Idle for 15 min.");
			MVGameControllerBase.ApplicationQuit(new QuitIdle());
			state = State.InActive15Min;
		}
	}
}
