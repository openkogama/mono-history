using UnityEngine;

public class ScaleAnimation : ScaleAnimationBase
{
	private float beginTime;

	[SerializeField]
	private AnimationCurve animationCurve;

	private float doneTime;

	public override void Play(float offsetTime = 0f)
	{
		beginTime = Time.time - offsetTime;
		state = State.Playing;
	}

	private void Stopped(float extraTime)
	{
		state = State.Stopped;
		if (OnScaleAnimationStopped != null)
		{
			OnScaleAnimationStopped(extraTime);
		}
	}

	private void Awake()
	{
		if (target != null)
		{
			originalScale = target.localScale;
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
				target.localScale = originalScale * animationCurve.Evaluate(doneTime);
				Stopped(num - doneTime);
			}
			else
			{
				target.localScale = originalScale * animationCurve.Evaluate(num);
			}
		}
	}
}
