using System;
using System.Collections.Generic;
using UnityEngine;

public class TimeReward : IUpdatecontrollerSubscriber
{
	private abstract class RewardStateBase
	{
		protected RewardStateDataEventArgs rewardStateEventArgs;

		public RewardStateDataEventArgs RewardStateEventArgs
		{
			get
			{
				RewardStateDataEventArgs result = rewardStateEventArgs;
				rewardStateEventArgs = null;
				return result;
			}
		}

		public abstract RewardStateBase Update();
	}

	private class RequestRewardData : RewardStateBase
	{
		private enum RequestRewardDataStatus
		{
			Requested,
			Accepted,
			Denied
		}

		private RequestRewardDataStatus requestedRewardDataStatus;

		private RewardCountdown rewardCountdown;

		private bool testRewardEnabled = true;

		public RequestRewardData()
		{
			BrowserComm.ToWeb.ExternalCall("requestRewardData", OnRewardData);
		}

		private void TestExternalCallBack(string function, Action<Dictionary<string, object>> action)
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary.Add("rewardEnabled", testRewardEnabled);
			if (testRewardEnabled)
			{
				dictionary.Add("timeInSeconds", 180);
				dictionary.Add("gold", 1);
				dictionary.Add("silver", 0);
				action(dictionary);
			}
			else
			{
				action(dictionary);
			}
		}

		public override RewardStateBase Update()
		{
			if (requestedRewardDataStatus == RequestRewardDataStatus.Denied)
			{
				return new RewardDone();
			}
			if (requestedRewardDataStatus == RequestRewardDataStatus.Accepted)
			{
				return rewardCountdown;
			}
			return this;
		}

		private void OnRewardData(Dictionary<string, object> rewardData)
		{
			if (!(bool)rewardData["rewardEnabled"])
			{
				requestedRewardDataStatus = RequestRewardDataStatus.Denied;
				return;
			}
			int timeInSeconds = (int)rewardData["timeInSeconds"];
			int amountGold = (int)rewardData["gold"];
			int amountSilver = (int)rewardData["silver"];
			rewardCountdown = new RewardCountdown(timeInSeconds, amountGold, amountSilver);
			requestedRewardDataStatus = RequestRewardDataStatus.Accepted;
		}
	}

	private class RewardCountdown : RewardStateBase
	{
		private WaitForTicks waitForTicks;

		public RewardCountdown(int timeInSeconds, int amountGold, int amountSilver)
		{
			Debug.Log((object)"Time is started");
			waitForTicks = new WaitForTicks(timeInSeconds * 1000);
			rewardStateEventArgs = new RewardStateDataEventArgs(timeInSeconds, amountGold, amountSilver);
		}

		public override RewardStateBase Update()
		{
			if (waitForTicks.TimeIsUp)
			{
				Debug.Log((object)"Time is up");
				return new RequestReward();
			}
			return this;
		}
	}

	private class RequestReward : RewardStateBase
	{
		public RequestReward()
		{
			BrowserComm.ToWeb.ExternalCall("requestReward");
		}

		public override RewardStateBase Update()
		{
			return new RewardDone();
		}
	}

	private class RewardDone : RewardStateBase
	{
		public override RewardStateBase Update()
		{
			return this;
		}
	}

	private RewardStateBase rewardStateBase = new RewardDone();

	public event EventHandler<RewardStateDataEventArgs> RewardStateChanged;

	public void Init()
	{
		MVGameController.Instance.UpdateController.AddUpdateObject(this, UpdatePriority.UPDATEBUCKET_STANDARD);
		rewardStateBase = new RequestRewardData();
	}

	public void UpdateControllerFixedUpdate()
	{
	}

	public void UpdateControllerUpdate()
	{
		rewardStateBase = rewardStateBase.Update();
		RewardStateDataEventArgs rewardStateEventArgs = rewardStateBase.RewardStateEventArgs;
		if (rewardStateEventArgs != null && RewardStateChanged != null)
		{
			RewardStateChanged(this, rewardStateEventArgs);
		}
	}
}
