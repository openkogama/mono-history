using System;
using System.Collections.Generic;
using UnityEngine;

public class MoveAnimation : MoveAnimationBase
{
	private float beginTime;

	private float doneTime;

	private Vector3 direction;

	[SerializeField]
	private List<KeyFrameCallback> keyFrameCallbacks;

	[SerializeField]
	private AnimationCurve animationCurve;

	[SerializeField]
	private Transform moveTo;

	public override void Play(float offsetTime = 0f)
	{
		beginTime = Time.time - offsetTime;
		state = State.Playing;
		target.localPosition = originalLocalPos;
		foreach (KeyFrameCallback keyFrameCallback in keyFrameCallbacks)
		{
			keyFrameCallback.Reset();
		}
		Vector3 vector = target.worldToLocalMatrix.MultiplyPoint(moveTo.position);
		direction = vector - target.localPosition;
	}

	private void Stopped(float extraTime)
	{
		state = State.Stopped;
		if (OnMoveAnimationStopped != null)
		{
			OnMoveAnimationStopped(extraTime);
		}
	}

	private void Awake()
	{
		keyFrameCallbacks.Sort((KeyFrameCallback a, KeyFrameCallback b) => a.KeyFrameIndex - b.KeyFrameIndex);
		if (target != null)
		{
			originalLocalPos = target.localPosition;
		}
		else
		{
			Debug.LogError("Target not set");
		}
		doneTime = animationCurve.keys[animationCurve.length - 1].time;
	}

	private void Update()
	{
		Test();
		if (state == State.Playing)
		{
			float num = Time.time - beginTime;
			if (num > doneTime)
			{
				target.localPosition = direction * animationCurve.Evaluate(doneTime);
				Stopped(num - doneTime);
			}
			else
			{
				target.localPosition = direction * animationCurve.Evaluate(num);
			}
			EvaluateKeyFrameCallbacks(num);
		}
	}

	public void SubscribeToKeyFrame(string keyFrameName, Action callback)
	{
		foreach (KeyFrameCallback keyFrameCallback in keyFrameCallbacks)
		{
			if (keyFrameCallback.KeyFrameName == keyFrameName)
			{
				keyFrameCallback.callbacks = (Action)Delegate.Combine(keyFrameCallback.callbacks, callback);
				return;
			}
		}
		Debug.LogError("Failed to find keyFrame");
	}

	private void EvaluateKeyFrameCallbacks(float timePassedSincePlay)
	{
		foreach (KeyFrameCallback keyFrameCallback in keyFrameCallbacks)
		{
			keyFrameCallback.Evaluate(timePassedSincePlay, animationCurve);
		}
	}
}
