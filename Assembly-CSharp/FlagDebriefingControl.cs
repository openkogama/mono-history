using System;
using UnityEngine;

public class FlagDebriefingControl
{
	public float RunStartTime;

	public bool IsInFlagDebriefing;

	public Action<int> OnFlagDebriefing;

	public Action OnFlagCountDown;

	public Action OnFlagDebriefingEnd;

	public Action OnFlagCountDownEnd;

	public void StartFlagDebriefing(int captureTime)
	{
		IsInFlagDebriefing = true;
		if (OnFlagDebriefing != null)
		{
			OnFlagDebriefing(captureTime);
		}
	}

	public void StartFlagCountDown()
	{
		if (OnFlagCountDown != null)
		{
			OnFlagCountDown();
		}
		IsInFlagDebriefing = true;
	}

	public void EndFlagDebriefing()
	{
		if (OnFlagDebriefingEnd != null)
		{
			OnFlagDebriefingEnd();
		}
	}

	public void EndFlagCountDown()
	{
		RunStartTime = Time.time;
		IsInFlagDebriefing = false;
		if (OnFlagCountDownEnd != null)
		{
			OnFlagCountDownEnd();
		}
	}

	public void ResetToSpawnPoint()
	{
		MVGameControllerBase.Game.LocalPlayer.ResetCheckpoint();
	}
}
