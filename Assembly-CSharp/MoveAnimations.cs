using System;
using System.Collections.Generic;
using UnityEngine;

public class MoveAnimations : MoveAnimationBase
{
	[SerializeField]
	private List<MoveAnimationBase> moveAnimations;

	public OnMoveAnimationStoppedDelegate OnIntermediateMoveAnimationStopped;

	private int index;

	private void Awake()
	{
		foreach (MoveAnimationBase moveAnimation in moveAnimations)
		{
			moveAnimation.SetTarget(target);
			moveAnimation.OnMoveAnimationStopped = (OnMoveAnimationStoppedDelegate)Delegate.Combine(moveAnimation.OnMoveAnimationStopped, new OnMoveAnimationStoppedDelegate(OnMoveAnimationDone));
		}
	}

	private void OnMoveAnimationDone(float extraTime)
	{
		index++;
		if (moveAnimations.Count > index)
		{
			Play();
			if (OnIntermediateMoveAnimationStopped != null)
			{
				OnIntermediateMoveAnimationStopped(extraTime);
			}
		}
		else
		{
			if (OnMoveAnimationStopped != null)
			{
				OnMoveAnimationStopped(extraTime);
			}
			index = 0;
		}
	}

	public override void Play(float offsetTime = 0f)
	{
		moveAnimations[index].Play();
	}

	private void Update()
	{
		Test();
	}
}
