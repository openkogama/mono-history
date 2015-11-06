using System;
using System.Collections;
using MV.Common;
using UnityEngine;

public class MVGUIReward : UXViewScript
{
	public float fadeTime = 2f;

	public UXGroup rewardGroup;

	public RewardCube rewardCube;

	private TimeSpan timeSpan;

	private float showBeforeRewardTime = 3f;

	private RewardType rewardType = RewardType.Gold;

	public override void Awake()
	{
		base.Awake();
		MVGameControllerBase.TimeReward.RewardStateChanged += TimeReward_RewardStateChanged;
	}

	private void TimeReward_RewardStateChanged(object sender, RewardStateDataEventArgs e)
	{
		if (EvaluateRewardAmounts(e.amountGold, e.amountSilver))
		{
			SetupRewardType(e.amountGold, e.amountSilver);
			timeSpan = e.timeSpan;
			StartCoroutine(CountDown());
			StartCoroutine(RewardCoroutine());
		}
	}

	private bool EvaluateRewardAmounts(int amountGold, int amountSilver)
	{
		if (amountGold <= 0 && amountSilver <= 0)
		{
			Debug.LogError("Both gold and silver amounts are 0");
			return false;
		}
		if (amountGold > 0 && amountSilver > 0)
		{
			Debug.LogWarning("Both gold and silver amounts are more than 0. Using gold");
		}
		return true;
	}

	private void SetupRewardType(int amountGold, int amountSilver)
	{
		if (amountGold > 0)
		{
			rewardType = RewardType.Gold;
		}
		else
		{
			rewardType = RewardType.Silver;
		}
		rewardCube.SetRewardType(rewardType);
	}

	private IEnumerator CountDown()
	{
		while (timeSpan.TotalSeconds >= 0.0)
		{
			timeSpan = timeSpan.Subtract(new TimeSpan(0, 0, 0, 0, (int)(Time.deltaTime * 1000f)));
			yield return 0;
		}
	}

	private IEnumerator RewardCoroutine()
	{
		yield return StartCoroutine(WaitUntilRewardTimeIsAlmostUp());
		yield return StartCoroutine(ShowGUI());
		yield return StartCoroutine(WaitUntilRewardTimeIsUp());
		StartCoroutine(rewardCube.DoAnimation());
		yield return StartCoroutine(HideGUI());
	}

	private IEnumerator ShowGUI()
	{
		View.Show();
		rewardGroup.SetVisible(visible: true);
		rewardCube.SetVisible(visible: true);
		yield return StartCoroutine(pTween.To(fadeTime, 0f, 1f, (float t) =>
		{
			rewardGroup.SetAlpha(t, string.Empty);
			rewardCube.SetAlpha(t);
		}));
	}

	private IEnumerator HideGUI()
	{
		yield return StartCoroutine(pTween.To(fadeTime, 1f, 0f, (float t) =>
		{
			rewardGroup.SetAlpha(t, string.Empty);
			rewardCube.SetAlpha(t);
			if (t == 0f)
			{
				rewardGroup.SetVisible(visible: false);
				rewardCube.SetVisible(visible: false);
			}
		}));
	}

	private IEnumerator WaitUntilRewardTimeIsAlmostUp()
	{
		while (GetTimeLeftBeforeReward() > showBeforeRewardTime)
		{
			yield return 0;
		}
	}

	private IEnumerator WaitUntilRewardTimeIsUp()
	{
		while (GetTimeLeftBeforeReward() >= 0f)
		{
			yield return 0;
		}
	}

	private float GetTimeLeftBeforeReward()
	{
		double totalSeconds = timeSpan.TotalSeconds;
		return (float)totalSeconds;
	}
}
