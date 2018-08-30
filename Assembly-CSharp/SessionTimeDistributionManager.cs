using System;
using System.Collections.Generic;
using UnityEngine;

public class SessionTimeDistributionManager : MonoBehaviour
{
	private bool countingSessionTime;

	private float startTime;

	private Queue<int> intervals = new Queue<int>();

	private void Start()
	{
		for (int i = 0; i < 18; i += 2)
		{
			intervals.Enqueue(i);
		}
		if (!MVGameControllerBase.Game.LocalPlayer.IsReady)
		{
			MVPlayerContainer mVPlayerContainer = MVGameControllerBase.Game.MVPlayerContainer;
			mVPlayerContainer.OnLocalPlayerReady = (Action)Delegate.Combine(mVPlayerContainer.OnLocalPlayerReady, new Action(OnPlayerReady));
		}
		else
		{
			OnPlayerReady();
		}
	}

	private void OnPlayerReady()
	{
		MVPlayerContainer mVPlayerContainer = MVGameControllerBase.Game.MVPlayerContainer;
		mVPlayerContainer.OnLocalPlayerReady = (Action)Delegate.Remove(mVPlayerContainer.OnLocalPlayerReady, new Action(OnPlayerReady));
		countingSessionTime = true;
		startTime = Time.time;
	}

	private void Update()
	{
		if (countingSessionTime)
		{
			if (intervals.Count <= 0)
			{
				enabled = false;
			}
			else if (Time.time - startTime > (float)(intervals.Peek() * 60))
			{
				StatHatWrapper.Count("SessionTimeMeasuredAtMinute" + intervals.Dequeue(), 1);
			}
		}
	}
}
