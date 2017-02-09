using UnityEngine;

public class HolsterTip : MonoBehaviour
{
	private enum HolsterState
	{
		NotSet,
		Holstered,
		Unholstered
	}

	[SerializeField]
	private CanvasGroup group;

	[SerializeField]
	private AnimationCurve textVisibilityCurve;

	[SerializeField]
	private float duration = 1f;

	[SerializeField]
	private float timeoutPeriod = 120f;

	private float currentTime;

	private float activatedTime = -120f;

	private HolsterState holsterState;

	public void Reset()
	{
		currentTime = 0f;
		group.alpha = 0f;
	}

	public void SetHolsterState(bool itemIsHolstered)
	{
		HolsterState holsterState = (itemIsHolstered ? HolsterState.Holstered : HolsterState.Unholstered);
		if (this.holsterState != holsterState && currentTime == 0f && Time.time > activatedTime + timeoutPeriod)
		{
			this.holsterState = holsterState;
			enabled = true;
			activatedTime = Time.time;
		}
	}

	private void Update()
	{
		if (holsterState != HolsterState.NotSet)
		{
			currentTime += Time.deltaTime;
			group.alpha = textVisibilityCurve.Evaluate(currentTime / duration);
			if (currentTime >= duration)
			{
				group.alpha = 0f;
				enabled = false;
				currentTime = 0f;
			}
		}
	}
}
