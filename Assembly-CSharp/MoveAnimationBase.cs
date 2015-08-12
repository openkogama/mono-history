using UnityEngine;

public class MoveAnimationBase : MonoBehaviour
{
	protected enum State
	{
		None,
		Stopped,
		Playing
	}

	public delegate void OnMoveAnimationStoppedDelegate(float extraTime);

	protected State state;

	[SerializeField]
	protected Transform target;

	protected Vector3 originalLocalPos;

	public OnMoveAnimationStoppedDelegate OnMoveAnimationStopped;

	[SerializeField]
	private State testState;

	public virtual void Play(float offsetTime = 0f)
	{
	}

	protected void Test()
	{
		if (testState == State.Playing)
		{
			Play();
			testState = State.None;
		}
	}

	public void SetTarget(Transform target)
	{
		if (this.target == null)
		{
			this.target = target;
			originalLocalPos = target.localPosition;
		}
		else
		{
			Debug.LogError("Target already set");
		}
	}
}
