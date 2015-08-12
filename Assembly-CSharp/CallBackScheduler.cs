using System;
using UnityEngine;

internal class CallBackScheduler : MonoBehaviour
{
	private Action callBack;

	private int startFrame = -1;

	private int framesToWait = -1;

	public static void ScheduleAfterTime(GameObject hostObj, Action callBack, float time)
	{
		CallBackScheduler callBackScheduler = hostObj.AddComponent<CallBackScheduler>();
		callBackScheduler.ScheduleCallAfterTime(callBack, time);
	}

	public static void ScheduleAfterTime(Action callBack, float time)
	{
		GameObject gameObject = new GameObject("CallBackSheduler");
		CallBackScheduler callBackScheduler = gameObject.AddComponent<CallBackScheduler>();
		callBackScheduler.ScheduleCallAfterTime(callBack, time);
	}

	public static void ScheduleAfterFrames(GameObject hostObj, Action callBack, int framesToWait)
	{
		CallBackScheduler callBackScheduler = hostObj.AddComponent<CallBackScheduler>();
		callBackScheduler.ScheduleCallAfterFrames(callBack, framesToWait);
	}

	public static void ScheduleAfterFrames(Action callBack, int framesToWait)
	{
		GameObject gameObject = new GameObject("CallBackSheduler");
		CallBackScheduler callBackScheduler = gameObject.AddComponent<CallBackScheduler>();
		callBackScheduler.ScheduleCallAfterFrames(callBack, framesToWait);
	}

	private void ScheduleCallAfterTime(Action callBack, float time)
	{
		this.callBack = callBack;
		Invoke("CallFunc", time);
	}

	private void ScheduleCallAfterFrames(Action callBack, int framesToWait)
	{
		this.callBack = callBack;
		startFrame = Time.frameCount;
		this.framesToWait = framesToWait;
	}

	private void CallFunc()
	{
		callBack();
		UnityEngine.Object.Destroy(this);
	}

	private void Update()
	{
		if (0 <= framesToWait && startFrame + framesToWait == Time.frameCount)
		{
			callBack();
			UnityEngine.Object.Destroy(this);
		}
	}
}
