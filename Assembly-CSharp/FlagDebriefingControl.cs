using System;
using UnityEngine;

public static class FlagDebriefingControl
{
	public static float RunStartTime;

	public static bool IsInFlagDebriefing;

	public static Action<int> OnFlagDebriefing;

	public static Action OnFlagCountDown;

	public static Action OnFlagDebriefingEnd;

	public static Action OnFlagCountDownEnd;

	public static Action OnResetToSpawnPoint;

	public static void StartFlagDebriefing(int captureTime)
	{
		if (OnFlagDebriefing != null)
		{
			OnFlagDebriefing(captureTime);
		}
		IsInFlagDebriefing = true;
	}

	public static void StartFlagCountDown()
	{
		if (OnFlagCountDown != null)
		{
			OnFlagCountDown();
		}
		IsInFlagDebriefing = true;
	}

	public static void EndFlagDebriefing()
	{
		if (OnFlagDebriefingEnd != null)
		{
			OnFlagDebriefingEnd();
		}
	}

	public static void EndFlagCountDown()
	{
		RunStartTime = Time.time;
		IsInFlagDebriefing = false;
		if (OnFlagCountDownEnd != null)
		{
			OnFlagCountDownEnd();
		}
	}

	public static void ResetToSpawnPoint()
	{
		MVGameControllerBase.Game.LocalPlayer.ResetCheckpoint();
		if (OnResetToSpawnPoint != null)
		{
			OnResetToSpawnPoint();
		}
	}
}
