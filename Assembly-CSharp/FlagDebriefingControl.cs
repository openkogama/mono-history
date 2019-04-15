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

	public static void PostResetCleanup()
	{
		if (OnFlagDebriefing != null)
		{
			Debug.LogWarning("FlagDebriefingControl.OnFlagDebriefing still have subscribers.");
			OnFlagDebriefing = null;
		}
		if (OnFlagCountDown != null)
		{
			Debug.LogWarning("FlagDebriefingControl.OnFlagCountDown still have subscribers.");
			OnFlagCountDown = null;
		}
		if (OnFlagDebriefingEnd != null)
		{
			Debug.LogWarning("FlagDebriefingControl.OnFlagDebriefingEnd still have subscribers.");
			OnFlagDebriefingEnd = null;
		}
		if (OnFlagCountDownEnd != null)
		{
			Debug.LogWarning("FlagDebriefingControl.OnFlagCountDownEnd still have subscribers.");
			OnFlagCountDownEnd = null;
		}
		if (OnResetToSpawnPoint != null)
		{
			Debug.LogWarning("FlagDebriefingControl.OnResetToSpawnPoint still have subscribers.");
			OnResetToSpawnPoint = null;
		}
		if (IsInFlagDebriefing)
		{
			Debug.LogWarning("IsInFlagDebriefing is not properly reset on game unload.");
			IsInFlagDebriefing = false;
		}
		if (RunStartTime != 0f)
		{
			Debug.LogWarning("RunStartTime is not properly reset on game unload.");
			RunStartTime = 0f;
		}
	}
}
