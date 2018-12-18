using System;
using MV.Common;
using UnityEngine;

public class AwayMonitor
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

	private static AwayMonitor instance;

	private DateTime latestResetAFKTime = DateTime.Now;

	private readonly TimeSpan awayCheckFrequency = new TimeSpan(0, 0, 0, 59);

	private IdleKickTimes idleKickTimes;

	private State state;

	private bool idleKickEnabled = true;

	private DateTime latestMouseMoveTime = DateTime.Now;

	private const string mouseX = "Mouse X";

	private const string mouseY = "Mouse Y";

	private const string scroll = "Mouse ScrollWheel";

	public static bool IdleKickEnabled
	{
		get
		{
			return instance.idleKickEnabled;
		}
		set
		{
			instance.idleKickEnabled = value;
		}
	}

	public DateTime LatestMouseMoveTime => latestMouseMoveTime;

	private AwayMonitor()
	{
	}

	public static void Initialize(MVGameMode mode)
	{
		instance = new AwayMonitor();
		switch (mode)
		{
		case MVGameMode.Play:
			instance.idleKickTimes = new IdleKickTimes(5, 15);
			break;
		case MVGameMode.Edit:
		case MVGameMode.CharacterEditor:
			instance.idleKickTimes = new IdleKickTimes(15, 30);
			break;
		default:
			Debug.LogError(string.Concat("GameMode: ", mode, ", is not accounted"));
			break;
		}
	}

	public static void Destroy()
	{
		instance = null;
	}

	public static void Update()
	{
		instance.UpdateMouse();
		instance.UpdateIdle();
	}

	public static void UpdateIdleAction()
	{
		instance.latestMouseMoveTime = DateTime.Now;
	}

	private void UpdateIdle()
	{
		if (DateTime.Now - LatestMouseMoveTime < awayCheckFrequency && DateTime.Now - latestResetAFKTime > awayCheckFrequency)
		{
			BrowserComm.ToJavaScript.ExternalCall("resetAFKtimer");
			latestResetAFKTime = DateTime.Now;
		}
		if (idleKickEnabled)
		{
			HandleIdle();
		}
	}

	private void UpdateMouse()
	{
		if (MVInputWrapper.GetAxisRawWithoutSensitivity("Mouse ScrollWheel") > Mathf.Epsilon || MVInputWrapper.GetAxisRawWithoutSensitivity("Mouse X") > Mathf.Epsilon || MVInputWrapper.GetAxisRawWithoutSensitivity("Mouse Y") > Mathf.Epsilon)
		{
			latestMouseMoveTime = DateTime.Now;
		}
	}

	private void HandleIdle()
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
