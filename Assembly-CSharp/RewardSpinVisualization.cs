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

	private bool rotating;

	private float spin;

	private float targetPos;

	private float curveDelta;

	private Vector3 startPos;

	private UnityAction OnFinishedSpinning;

	public void Initialize()
	{
		SetSpinsLeftText(RewardManager.NumberOfPendingRewards);
		RewardManager.NumberOfPendingRewardsChanged = (UnityAction)Delegate.Combine(RewardManager.NumberOfPendingRewardsChanged, new UnityAction(UpdateRewardCount));
		startPos = contentMoverHandle.anchoredPosition;
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

	public void ResetPosition()
	{
		contentMoverHandle.anchoredPosition = startPos;
	}

	private void Update()
	{
		if (rotating)
		{
			spin += Time.deltaTime;
			curveDelta = spinSpeedDeltaCurve.Evaluate(spin / spinDuration);
			Vector3 vector = startPos;
			vector.x = curveDelta * targetPos;
			contentMoverHandle.anchoredPosition = vector;
			if (spin >= spinDuration)
			{
				rotating = false;
				OnFinishedSpinning();
			}
		}
	}

	private void SetSpinsLeftText(int count)
	{
		spinsLeftText.text = string.Format(TM._("Spins left: {0}"), count);
	}

	public void Clear()
	{
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
