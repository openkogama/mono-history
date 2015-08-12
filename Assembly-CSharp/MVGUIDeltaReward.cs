using System;
using UnityEngine;

public class MVGUIDeltaReward : UXGroup
{
	[SerializeField]
	private UXText amount;

	[SerializeField]
	private MoveAnimation moveUp;

	[SerializeField]
	private MoveAnimation moveSideWays;

	public void Init(int amount)
	{
		this.amount.Text = amount.ToString();
		MoveAnimation moveAnimation = moveUp;
		moveAnimation.OnMoveAnimationStopped = (MoveAnimationBase.OnMoveAnimationStoppedDelegate)Delegate.Combine(moveAnimation.OnMoveAnimationStopped, new MoveAnimationBase.OnMoveAnimationStoppedDelegate(OnMoveAnimationDone));
		moveUp.Play();
		moveSideWays.Play();
	}

	private void OnMoveAnimationDone(float extraTime)
	{
		UnityEngine.Object.Destroy(gameObject);
	}
}
