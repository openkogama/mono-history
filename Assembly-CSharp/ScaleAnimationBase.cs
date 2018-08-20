using UnityEngine;

public class ScaleAnimationBase : MonoBehaviour
{
	public delegate void OnScaleAnimationStoppedDelegate(float extraTime);

	protected enum State
	{
		None,
		Stopped,
		Playing
	}

	protected State state;

	protected Vector3 originalScale = Vector3.one;

	[SerializeField]
	protected Transform target;

	[SerializeField]
	private State testState;

	public OnScaleAnimationStoppedDelegate OnScaleAnimationStopped;

	public Vector3 OriginalScale => originalScale;

	public void ResetScaleAnimation()
	{
		target.localScale = originalScale;
		state = State.Stopped;
	}

	protected void Test()
	{
		if (testState == State.Playing)
		{
			Play();
			testState = State.None;
		}
	}

	public virtual void Play(float offsetTime = 0f)
	{
	}

	public void SetTarget(Transform target)
	{
		if (this.target == null)
		{
			this.target = target;
			originalScale = target.localScale;
		}
		else
		{
			Debug.LogError("Target already set");
		}
	}
}
