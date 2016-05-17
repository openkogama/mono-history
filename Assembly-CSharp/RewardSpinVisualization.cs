using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class RewardSpinVisualization : MonoBehaviour
{
	[SerializeField]
	private RectTransform contentMoverHandle;

	[SerializeField]
	private RectTransform contentPosition;

	[SerializeField]
	private Text spinsLeftText;

	[SerializeField]
	private AnimationCurve spinSpeedDeltaCurve;

	[SerializeField]
	private float spinDuration;

	[SerializeField]
	private float animationSpeed;

	private bool rotating;

	private bool animating = true;

	private bool initialized;

	private float spin;

	private float targetPos;

	private float curveDelta;

	[SerializeField]
	private Vector2 hidingPosition;

	private Vector2 targetPosition;

	private Vector2 startPos;

	private Vector2 currPosition;

	private UnityAction OnFinishedSpinning;

	private UnityAction OnFinishedAnimating;

	private float animationTimer;

	public void Initialize(UnityAction OnInitialized)
	{
		OnFinishedAnimating = OnInitialized;
		SetSpinsLeftText(RewardManager.NumberOfPendingRewards);
		RewardManager.NumberOfPendingRewardsChanged = (UnityAction)Delegate.Combine(RewardManager.NumberOfPendingRewardsChanged, new UnityAction(UpdateRewardCount));
		startPos = contentMoverHandle.anchoredPosition;
		targetPosition = startPos;
	}

	private void UpdateRewardCount()
	{
		SetSpinsLeftText(RewardManager.NumberOfPendingRewards);
	}

	public void StartSpin(float targetPosition, UnityAction OnSpinFinished)
	{
		SetSpinsLeftText(RewardManager.NumberOfPendingRewards - 1);
		targetPos = targetPosition;
		spin = 0f;
		curveDelta = 0f;
		rotating = true;
		contentMoverHandle.anchoredPosition = startPos;
		OnFinishedSpinning = OnSpinFinished;
		rotating = true;
	}

	public void AnimateHiding(UnityAction OnFinished)
	{
		currPosition = contentMoverHandle.anchoredPosition;
		targetPosition = currPosition + hidingPosition;
		animating = true;
		OnFinishedAnimating = OnFinished;
		animationTimer = 0f;
	}

	public void AnimateShowing(UnityAction OnFinished)
	{
		contentMoverHandle.anchoredPosition = startPos + hidingPosition;
		targetPosition = startPos;
		currPosition = contentMoverHandle.anchoredPosition;
		animating = true;
		OnFinishedAnimating = OnFinished;
		animationTimer = 0f;
	}

	private void Update()
	{
		if (rotating)
		{
			spin += Time.deltaTime;
			curveDelta = spinSpeedDeltaCurve.Evaluate(spin / spinDuration);
			Vector2 anchoredPosition = startPos;
			anchoredPosition.x = curveDelta * targetPos;
			contentMoverHandle.anchoredPosition = anchoredPosition;
			if (spin >= spinDuration)
			{
				rotating = false;
				if (OnFinishedSpinning != null)
				{
					OnFinishedSpinning();
				}
			}
		}
		if (animating)
		{
			HandleTransition();
		}
	}

	private void HandleTransition()
	{
		animationTimer += Time.deltaTime * animationSpeed;
		if (initialized)
		{
			contentMoverHandle.anchoredPosition = Vector2.Lerp(currPosition, targetPosition, animationTimer);
		}
		if (contentMoverHandle.anchoredPosition == targetPosition)
		{
			animating = false;
			if (OnFinishedAnimating != null)
			{
				OnFinishedAnimating();
			}
			initialized = true;
		}
	}

	private void SetSpinsLeftText(int count)
	{
		spinsLeftText.text = string.Format(TM._("Spins left: {0}"), count);
	}

	public void Clear()
	{
		OnFinishedSpinning = null;
		OnFinishedAnimating = null;
		int childCount = contentPosition.childCount;
		for (int num = childCount - 1; num >= 0; num--)
		{
			UnityEngine.Object.Destroy(contentPosition.GetChild(num).gameObject);
		}
	}

	private void OnDestroy()
	{
		RewardManager.NumberOfPendingRewardsChanged = (UnityAction)Delegate.Remove(RewardManager.NumberOfPendingRewardsChanged, new UnityAction(UpdateRewardCount));
		Clear();
	}
}
