using System;
using System.Collections.Generic;
using UnityEngine;

public class ScaleAnimations : ScaleAnimationBase
{
	[SerializeField]
	private List<ScaleAnimationBase> scaleAnimations;

	public OnScaleAnimationStoppedDelegate OnIntermediateScaleAnimationStopped;

	private int index;

	private void Awake()
	{
		foreach (ScaleAnimationBase scaleAnimation in scaleAnimations)
		{
			scaleAnimation.SetTarget(target);
			scaleAnimation.OnScaleAnimationStopped = (OnScaleAnimationStoppedDelegate)Delegate.Combine(scaleAnimation.OnScaleAnimationStopped, new OnScaleAnimationStoppedDelegate(OnScaleAnimationDone));
		}
	}

	private void OnScaleAnimationDone(float extraTime)
	{
		index++;
		if (scaleAnimations.Count > index)
		{
			Play();
			if (OnIntermediateScaleAnimationStopped != null)
			{
				OnIntermediateScaleAnimationStopped(extraTime);
			}
		}
		else
		{
			if (OnScaleAnimationStopped != null)
			{
				OnScaleAnimationStopped(extraTime);
			}
			index = 0;
		}
	}

	public override void Play(float offsetTime = 0f)
	{
		scaleAnimations[index].Play();
	}

	private void Update()
	{
		Test();
	}
}
