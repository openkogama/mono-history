using System;
using System.Collections.Generic;
using MV.Common;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PlayButtonMobile : MonoBehaviour, IUpdatecontrollerSubscriber
{
	private class RewardData
	{
		public bool rewardEnabled;

		public int timeInSeconds;

		public int gold;

		public int silver;

		public int xp;

		public override string ToString()
		{
			return $"rewardEnabled {rewardEnabled}. timeInSeconds {timeInSeconds}. gold {gold}. silver {silver}.";
		}
	}

	private WaitForTicks waitForTicks;

	[SerializeField]
	private int timeInSeconds;

	[SerializeField]
	private GameObject claimRewardObject;

	[SerializeField]
	private Text timerText;

	[SerializeField]
	private Sprite notificationImage;

	[SerializeField]
	private Text text;

	[SerializeField]
	private EnterPlaySessionRoundCountDown enterPlaySessionRoundCountDownPrefab;

	public static bool IsCollected;

	public static Action CollectedChanged;

	public bool rewardAvailable { get; private set; }

	private int rewardXP { get; set; }

	private int rewardGold { get; set; }

	public void Play()
	{
		if (!IsCollected && CollectedChanged != null)
		{
			CollectedChanged();
		}
		if (MVGameControllerBase.Game.NetworkGameStateListener.CurrentGameState == MVGameStateType.Round)
		{
			MVGameControllerBase.IPlayModeUI.InLobbyState = false;
			if (MVGameControllerBase.WOCM.AvatarLocal.IsInMode(AvatarModeTypes.Hidden))
			{
				MVGameControllerBase.WOCM.AvatarLocal.SetMode(AvatarRuntimeState.Playing);
			}
		}
		else
		{
			EnterPlaySessionRoundCountDown enterPlaySessionRoundCountDown = UnityEngine.Object.Instantiate(enterPlaySessionRoundCountDownPrefab);
			ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
			{
				x.Push(enterPlaySessionRoundCountDown.gameObject, UIPushOption.Blocking);
			});
		}
	}

	private void Update()
	{
		if (MVGameControllerBase.Game.NetworkGameStateListener.CurrentGameState == MVGameStateType.RoundEnded)
		{
			text.text = TM._("New round starts in: ") + MVGameControllerBase.Game.NetworkGameStateListener.CountdownInSeconds;
			if (!text.enabled)
			{
				text.enabled = true;
			}
		}
		else if (text.enabled)
		{
			text.enabled = false;
		}
	}

	public void Initialize()
	{
		CollectedChanged = (Action)Delegate.Combine(CollectedChanged, new Action(OnCollectedChanged));
		rewardAvailable = false;
		claimRewardObject.SetActive(value: false);
		IsCollected = true;
		if (!MVGameControllerBase.UsingDevSessionData)
		{
			UpdateController.AddUpdateObject(this, UpdatePriority.UPDATEBUCKET_STANDARD);
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
		IsCollected = true;
		if (MVClientSettings.SpinEnabled)
		{
			ExecuteEvents.ExecuteHierarchy(gameObject, null, (IOfferController x, BaseEventData y) =>
			{
				x.RequestShowOffer(OnFinishedViewingAd);
			});
		}
		else
		{
			OnFinishedViewingAd();
		}
	}

	private void OnFinishedViewingAd()
	{
		claimRewardObject.gameObject.SetActive(value: false);
		NotificationController.PushNotification(TM._("Thank you for playing this NEW game! Received " + rewardXP + " XP and " + rewardGold + " gold!"), notificationImage);
		ParticleSystem particleSystem = UnityEngine.Object.Instantiate(PrefabPool.Instance.GoldExplosion);
		particleSystem.transform.parent = MVGameControllerBase.WOCM.AvatarLocal.Transform;
		particleSystem.transform.localPosition = new Vector3(0f, 1f, 0f);
		timerText.text = string.Empty;
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
		rewardXP = rewardData.xp;
		rewardGold = rewardData.gold;
		rewardAvailable = rewardData.rewardEnabled;
		claimRewardObject.gameObject.SetActive(rewardAvailable);
		IsCollected = !rewardAvailable;
		if (!rewardAvailable)
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
		if (rewardAvailable)
		{
			gameObject.SetActive(value: true);
			if (waitForTicks.TimeIsUp)
			{
				rewardAvailable = false;
				timerText.text = TM._("Claim!");
				Dictionary<object, object> dictionary = new Dictionary<object, object>();
				dictionary.Add((byte)2, 8);
				NotificationController.OnNotificationReceived(NotificationType.GoldRewardReady, dictionary);
			}
			else
			{
				float num = timeInSeconds * 1000 - (MVGameControllerBase.Game.ServerTimeInMilliSeconds - waitForTicks.startTicks);
				float num2 = num / 1000f;
				string arg = Mathf.Max(Mathf.Floor(num2 / 60f), 0f).ToString("00");
				string arg2 = (num2 % 60f).ToString("00");
				timerText.text = $"{arg:00}:{arg2:00}";
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
		CollectedChanged = (Action)Delegate.Remove(CollectedChanged, new Action(OnCollectedChanged));
	}
}
