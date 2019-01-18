using System;
using System.Collections.Generic;
using MV.Common;
using Newtonsoft.Json;
using UnityEngine;

public class TimedPlayReward : RewardButtonBase, IUpdatecontrollerSubscriber
{
	public static class RewardTracker
	{
		public static bool IsCollected;

		public static Action CollectedChanged;

		public static void Reset()
		{
			IsCollected = false;
		}

		public static void PostResetCleanup()
		{
			if (CollectedChanged != null)
			{
				Debug.LogWarning("TimedPlayReward.RewardTracker.CollectedChanged still have subscribers.");
				CollectedChanged = null;
			}
		}
	}

	private class RewardData
	{
		public bool rewardEnabled;

		public int timeInSeconds;

		public int xp;

		public override string ToString()
		{
			return $"rewardEnabled {rewardEnabled}. timeInSeconds {timeInSeconds}.";
		}
	}

	private WaitForTicks waitForTicks;

	[SerializeField]
	private int timeInSeconds;

	public bool RewardAvailable { get; private set; }

	private int RewardXP { get; set; }

	public bool IsClaimable { get; private set; }

	public void Initialize()
	{
		RewardTracker.CollectedChanged = (Action)Delegate.Combine(RewardTracker.CollectedChanged, new Action(OnCollectedChanged));
		if (!MVGameControllerBase.UsingDevSessionData)
		{
			UpdateController.AddUpdateObject(this, UpdatePriority.UPDATEBUCKET_STANDARD);
			RewardAvailable = false;
			gameObject.SetActive(value: false);
			IsClaimable = false;
			RequestRewardPermission();
		}
	}

	private void RequestRewardPermission()
	{
		GameSessionData gameSessionData = MVGameControllerBase.GameSessionData;
		string text = $"?profile_id={gameSessionData.profileID}&planet_id={gameSessionData.planetID}&token={gameSessionData.token}";
		AsyncWWWManager.WWWRequest(new GetRequest(gameSessionData.gameRewardDataURL + text, OnRewardData, WWWRequestPriority.WaitUntilSyncronizingIsDone));
	}

	private void OnCollectedChanged()
	{
		RewardTracker.IsCollected = true;
		OnFinishedViewingAd();
	}

	public void ClaimReward()
	{
		if (!RewardTracker.IsCollected && RewardTracker.CollectedChanged != null)
		{
			RewardTracker.CollectedChanged();
		}
	}

	private void OnFinishedViewingAd()
	{
		gameObject.SetActive(value: false);
		NotificationController.PushNotification(TM._("Thank you for playing this NEW game! Received " + RewardXP + " XP!"));
		AvatarPooledXPParticles avatarPooledXPParticles = PrefabPool.Instance.EnumPoolManager.Instantiate<AvatarPooledXPParticles>(PoolEnums.XP);
		avatarPooledXPParticles.transform.parent = MVGameControllerBase.WOCM.AvatarLocal.Transform;
		avatarPooledXPParticles.transform.localPosition = new Vector3(0f, 1f, 0f);
		avatarPooledXPParticles.transform.localRotation = Quaternion.identity;
		avatarPooledXPParticles.transform.localScale = Vector3.one;
		avatarPooledXPParticles.gameObject.layer = MVGameControllerBase.Game.LocalPlayer.Avatar.Body.GameObject.layer;
		avatarPooledXPParticles.Initialize(RewardXP);
		IsClaimable = false;
		GameSessionData gameSessionData = MVGameControllerBase.GameSessionData;
		WWWForm wWWForm = new WWWForm();
		wWWForm.AddField("token", gameSessionData.token);
		wWWForm.AddField("profile_id", gameSessionData.profileID);
		wWWForm.AddField("planet_id", gameSessionData.planetID);
		AsyncWWWManager.WWWRequest(new PostRequest(gameSessionData.gameRewardURL, wWWForm, OnFinishedRewardCollecting, WWWRequestPriority.ExecuteWhileSyncronizing));
	}

	private void OnFinishedRewardCollecting(WWW www)
	{
		RequestRewardPermission();
	}

	private void OnRewardData(WWW www)
	{
		if (!string.IsNullOrEmpty(www.error))
		{
			Debug.LogError(www.error);
			return;
		}
		RewardData rewardData = JsonConvert.DeserializeObject<RewardData>(www.text);
		RewardXP = rewardData.xp;
		RewardAvailable = rewardData.rewardEnabled;
		gameObject.SetActive(RewardAvailable);
		RewardTracker.IsCollected = !RewardAvailable;
		if (!RewardAvailable)
		{
			Debug.Log("no gold reward available");
		}
		else
		{
			waitForTicks = new WaitForTicks(timeInSeconds * 1000);
		}
	}

	public void UpdateControllerUpdate()
	{
		if (RewardAvailable)
		{
			gameObject.SetActive(value: true);
			if (waitForTicks.TimeIsUp)
			{
				IsClaimable = true;
				RewardAvailable = false;
				Dictionary<object, object> dictionary = new Dictionary<object, object>();
				dictionary.Add((byte)2, 8);
				NotificationController.OnNotificationReceived(NotificationType.GoldRewardReady, dictionary);
				EnableEffects();
			}
			else
			{
				float num = timeInSeconds * 1000 - (MVGameControllerBase.Game.ServerTimeInMilliSeconds - waitForTicks.startTicks);
				float num2 = num / 1000f;
				string text = Mathf.Max(Mathf.Floor(num2 / 60f), 0f).ToString("00");
				string text2 = (num2 % 60f).ToString("00");
				IsClaimable = false;
				DisableEffects();
				float num3 = timeInSeconds * 1000;
				UpdateOutline((num3 - num) / num3);
			}
		}
	}

	public void UpdateControllerFixedUpdate()
	{
	}

	private void OnDestroy()
	{
		AsyncWWWManager.UnsubscribeWWWRequest(OnRewardData);
		AsyncWWWManager.UnsubscribeWWWRequest(OnFinishedRewardCollecting);
		UpdateController.RemoveUpdateObject(this);
		RewardTracker.CollectedChanged = (Action)Delegate.Remove(RewardTracker.CollectedChanged, new Action(OnCollectedChanged));
	}
}
