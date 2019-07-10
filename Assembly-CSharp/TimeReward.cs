using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

public class TimeReward : IUpdatecontrollerSubscriberUpdate, IUpdatecontrollerSubscriberBase
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

		public abstract void Destroy();
	}

	private class RequestRewardData : RewardStateBase
	{
		private enum RequestRewardDataStatus
		{
			Requested,
			Accepted,
			Denied
		}

		private class RewardData
		{
			public bool rewardEnabled;

			public int timeInSeconds;

			public int gold;

			public override string ToString()
			{
				return $"rewardEnabled {rewardEnabled}. timeInSeconds {timeInSeconds}. gold {gold}.";
			}
		}

		private RequestRewardDataStatus requestedRewardDataStatus;

		private RewardCountdown rewardCountdown;

		private bool testRewardEnabled = true;

		public RequestRewardData()
		{
			GameSessionData gameSessionData = MVGameControllerBase.GameSessionData;
			string text = $"?profile_id={gameSessionData.profileID}&planet_id={gameSessionData.planetID}&token={gameSessionData.token}";
			AsyncWWWManager.WWWRequest(new GetRequest(MVGameControllerBase.GameSessionData.gameRewardDataURL + text, OnRewardData, WWWRequestPriority.WaitUntilSyncronizingIsDone));
			Debug.Log("MVGameControllerBase.GameSessionData.gameRewardDataURL + args: " + MVGameControllerBase.GameSessionData.gameRewardDataURL + text);
		}

		private void TestExternalCallBack(string function, Action<Dictionary<string, object>> action)
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary.Add("rewardEnabled", testRewardEnabled);
			if (testRewardEnabled)
			{
				dictionary.Add("timeInSeconds", 180);
				dictionary.Add("gold", 1);
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

		private void OnRewardData(WWW www)
		{
			Debug.Log("www.error: " + www.error);
			if (!string.IsNullOrEmpty(www.error))
			{
				Debug.LogError(www.error);
				return;
			}
			RewardData rewardData = JsonConvert.DeserializeObject<RewardData>(www.text);
			Debug.Log("www.text: " + www.text);
			Debug.Log("!rewardData.rewardEnabled: " + !rewardData.rewardEnabled);
			if (!rewardData.rewardEnabled)
			{
				requestedRewardDataStatus = RequestRewardDataStatus.Denied;
				return;
			}
			Debug.Log("!rewardData.timeInSeconds: " + rewardData.timeInSeconds);
			rewardCountdown = new RewardCountdown(rewardData.timeInSeconds, rewardData.gold);
			requestedRewardDataStatus = RequestRewardDataStatus.Accepted;
		}

		public override void Destroy()
		{
			AsyncWWWManager.UnsubscribeWWWRequest(OnRewardData);
		}
	}

	private class RewardCountdown : RewardStateBase
	{
		private WaitForTicks waitForTicks;

		public RewardCountdown(int timeInSeconds, int amountGold)
		{
			Debug.Log("Time is started");
			waitForTicks = new WaitForTicks(timeInSeconds * 1000);
			rewardStateEventArgs = new RewardStateDataEventArgs(timeInSeconds, amountGold);
		}

		public override void Destroy()
		{
		}

		public override RewardStateBase Update()
		{
			if (waitForTicks.TimeIsUp)
			{
				Debug.Log("Time is up");
				return new RequestReward();
			}
			return this;
		}
	}

	private class RequestReward : RewardStateBase
	{
		public RequestReward()
		{
			GameSessionData gameSessionData = MVGameControllerBase.GameSessionData;
			WWWForm wWWForm = new WWWForm();
			wWWForm.AddField("token", gameSessionData.token);
			wWWForm.AddField("profile_id", gameSessionData.profileID);
			wWWForm.AddField("planet_id", gameSessionData.planetID);
			AsyncWWWManager.WWWRequest(new PostRequest(gameSessionData.gameRewardURL, wWWForm, null, WWWRequestPriority.ExecuteWhileSyncronizing));
			Debug.Log("s.gameRewardURL: " + gameSessionData.gameRewardURL);
		}

		public override void Destroy()
		{
		}

		public override RewardStateBase Update()
		{
			return new RewardDone();
		}
	}

	private class RewardDone : RewardStateBase
	{
		public override void Destroy()
		{
		}

		public override RewardStateBase Update()
		{
			return this;
		}
	}

	private RewardStateBase rewardStateBase = new RewardDone();

	public event EventHandler<RewardStateDataEventArgs> RewardStateChanged;

	public void Init()
	{
		UpdateController.AddUpdateObject(this, UpdatePriority.UPDATEBUCKET_STANDARD);
		Debug.Log("!MVGameControllerBase.UsingDevSessionData: " + !MVGameControllerBase.UsingDevSessionData);
		if (!MVGameControllerBase.UsingDevSessionData)
		{
			rewardStateBase = new RequestRewardData();
		}
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

	public void DestroyRewardRequest()
	{
		rewardStateBase.Destroy();
	}
}
