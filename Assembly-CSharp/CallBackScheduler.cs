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
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Expected Obj, but got Unknown
		GameObject val = new GameObject("CallBackSheduler");
		CallBackScheduler callBackScheduler = val.AddComponent<CallBackScheduler>();
		callBackScheduler.ScheduleCallAfterTime(callBack, time);
	}

	public static void ScheduleAfterFrames(GameObject hostObj, Action callBack, int framesToWait)
	{
		CallBackScheduler callBackScheduler = hostObj.AddComponent<CallBackScheduler>();
		callBackScheduler.ScheduleCallAfterFrames(callBack, framesToWait);
	}

	public static void ScheduleAfterFrames(Action callBack, int framesToWait)
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Expected Obj, but got Unknown
		GameObject val = new GameObject("CallBackSheduler");
		CallBackScheduler callBackScheduler = val.AddComponent<CallBackScheduler>();
		callBackScheduler.ScheduleCallAfterFrames(callBack, framesToWait);
	}

	private void ScheduleCallAfterTime(Action callBack, float time)
	{
		this.callBack = callBack;
		((MonoBehaviour)this).Invoke("CallFunc", time);
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
		Object.Destroy((Object)(object)this);
	}

	private void Update()
	{
		if (0 <= framesToWait && startFrame + framesToWait == Time.frameCount)
		{
			callBack();
			Object.Destroy((Object)(object)this);
		}
	}
}
