using System;
using MV.Common;
using UnityEngine;

public static class AwayMonitor
{
	private enum State
	{
		Active,
		IdleAndWarned,
		Kicked
	}

	private class IdleKickTimes
	{
		public int warningTimeMinutes;

		public int idleKickTimeMinutes;

		public readonly TimeSpan warningTimeSpan;

		public readonly TimeSpan idleKickTimeSpan;

		public IdleKickTimes(int warnAfterMinutes, int kickAfterMinutes)
		{
			warningTimeMinutes = warnAfterMinutes;
			idleKickTimeMinutes = kickAfterMinutes;
			warningTimeSpan = new TimeSpan(0, 0, warningTimeMinutes, 0);
			idleKickTimeSpan = new TimeSpan(0, 0, idleKickTimeMinutes, 0);
		}
	}

	private static DateTime latestResetAFKTime = DateTime.Now;

	private static readonly TimeSpan awayCheckFrequency = new TimeSpan(0, 0, 0, 59);

	private static IdleKickTimes idleKickTimes;

	private static State state = State.Active;

	private static bool idleKickEnabled = true;

	private static Vector3 prevMousePos = default;

	private static DateTime latestMouseMoveTime = DateTime.Now;

	private static readonly string mouseX = "Mouse X";

	private static readonly string mouseY = "Mouse Y";

	private static readonly string scroll = "Mouse ScrollWheel";

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

	public static void Initialize(MVGameMode mode)
	{
		switch (mode)
		{
		case MVGameMode.Play:
			idleKickTimes = new IdleKickTimes(5, 15);
			break;
		case MVGameMode.Edit:
		case MVGameMode.CharacterEditor:
			idleKickTimes = new IdleKickTimes(15, 30);
			break;
		default:
			Debug.LogError(string.Concat("GameMode: ", mode, ", is not accounted"));
			break;
		}
	}

	public static void Update()
	{
		UpdateMouse();
		UpdateIdle();
	}

	public static void UpdateIdleAction()
	{
		latestMouseMoveTime = DateTime.Now;
	}

	private static void UpdateIdle()
	{
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
		if (MVInputWrapper.GetAxisRaw(scroll) > Mathf.Epsilon || MVInputWrapper.GetAxisRaw(mouseX) > Mathf.Epsilon || MVInputWrapper.GetAxisRaw(mouseY) > Mathf.Epsilon)
		{
			prevMousePos = Input.mousePosition;
			latestMouseMoveTime = DateTime.Now;
		}
	}

	private static void HandleIdle()
	{
		if (state != State.Kicked)
		{
			TimeSpan timeSpan = DateTime.Now - LatestMouseMoveTime;
			if (timeSpan < idleKickTimes.warningTimeSpan)
			{
				state = State.Active;
			}
			else if (timeSpan > idleKickTimes.warningTimeSpan && state != State.IdleAndWarned)
			{
				MVGameControllerBase.PostGameMsg(MVGameMsgType.AdminMsg, $"Idle.You will be kicked in {idleKickTimes.idleKickTimeMinutes - idleKickTimes.warningTimeMinutes} min.");
				state = State.IdleAndWarned;
			}
			else if (timeSpan > idleKickTimes.idleKickTimeSpan && state != State.Kicked)
			{
				MVGameControllerBase.PostGameMsg(MVGameMsgType.AdminMsg, $"Kicked. Idle for {idleKickTimes.idleKickTimeMinutes} min.");
				MVGameControllerBase.ApplicationQuit(new QuitIdle());
				state = State.Kicked;
			}
		}
	}
}
