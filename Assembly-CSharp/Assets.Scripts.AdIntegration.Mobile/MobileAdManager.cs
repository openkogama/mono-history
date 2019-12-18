using System;
using System.Collections.Generic;
using GoogleMobileAds.Api;
using GoogleMobileAds.Api.Mediation.IronSource;
using GoogleMobileAds.Api.Mediation.Tapjoy;
using GoogleMobileAds.Api.Mediation.UnityAds;
using GoogleMobileAds.Api.Mediation.Vungle;
using MV.Common;
using Newtonsoft.Json;
using UnityEngine;

namespace Assets.Scripts.AdIntegration.Mobile;

public class MobileAdManager : IAdManager, IUpdatecontrollerSubscriberUpdate, IUpdatecontrollerSubscriberBase
{
	private class AdLoadState
	{
		private const int loadAttemptsMax = 0;

		public bool loadingAd;

		private int loadAttempts;

		public bool IsOk => loadAttempts >= 0;

		public void ResetAttempts()
		{
			loadAttempts = 0;
		}

		public bool Reload()
		{
			if (loadAttempts < 0)
			{
				loadAttempts++;
				return true;
			}
			Debug.LogError("Failed to get an ad after " + loadAttempts);
			return false;
		}

		public override string ToString()
		{
			return $"IsOk {IsOk}\n loadingAd {loadingAd}\n loadAttemps {loadAttempts}\n";
		}
	}

	public class InternalStateInterstitial
	{
		private DateTime prevInterstitialTime = DateTime.MinValue;

		private AdLoadState adLoadState = new AdLoadState();

		private InterstitialAdResult interstitialAdResult = InterstitialAdResult.Done;

		private InterstitialAd interstitial;

		private bool isHandlingRequest;

		private Action<InterstitialAdResult> interstitialAdCallback;

		public TimeSpan TimeSinceLastInterstitial => DateTime.Now - prevInterstitialTime;

		public bool IsHandlingRequest => isHandlingRequest;

		public bool IsOk => adLoadState.IsOk;

		public void RequestInterstitialAd(Action<InterstitialAdResult> interstitialAdCallback)
		{
			this.interstitialAdCallback = interstitialAdCallback;
			isHandlingRequest = true;
			if (interstitial.IsLoaded())
			{
				interstitial.Show();
			}
			else if (!adLoadState.loadingAd)
			{
				CreateAndLoadInterstitialAd();
			}
		}

		public void CreateAndLoadInterstitialAd()
		{
			Debug.Log("InitializeInterstitialAd");
			SendStat("Ad.InterstitialLoad");
			DestroyInterstitial();
			interstitial = new InterstitialAd(MobileAdManagerCredentials.GetAdMobCredentials().InterstitialAdUnitId);
			SetupCallbacks();
			LoadInterstitialAd();
		}

		private void LoadInterstitialAd()
		{
			if (adLoadState.loadingAd)
			{
				Debug.LogError("Already loading ad");
				return;
			}
			adLoadState.loadingAd = true;
			interstitial.LoadAd(CreateAdRequest());
		}

		private void SetupCallbacks()
		{
			interstitial.OnAdLoaded += HandleInterstitialLoaded;
			interstitial.OnAdFailedToLoad += HandleInterstitialFailedToLoad;
			interstitial.OnAdOpening += HandleInterstitialOpened;
			interstitial.OnAdClosed += HandleInterstitialClosed;
			interstitial.OnAdLeavingApplication += HandleInterstitialLeftApplication;
		}

		private void RemoveCallbacks()
		{
			interstitial.OnAdLoaded -= HandleInterstitialLoaded;
			interstitial.OnAdFailedToLoad -= HandleInterstitialFailedToLoad;
			interstitial.OnAdOpening -= HandleInterstitialOpened;
			interstitial.OnAdClosed -= HandleInterstitialClosed;
			interstitial.OnAdLeavingApplication -= HandleInterstitialLeftApplication;
		}

		private void DestroyInterstitial()
		{
			if (interstitial != null)
			{
				RemoveCallbacks();
				interstitial.Destroy();
			}
			interstitial = null;
		}

		public void Destroy()
		{
			DestroyInterstitial();
			interstitialAdCallback = null;
		}

		private void HandleError()
		{
			if (adLoadState.Reload())
			{
				LoadInterstitialAd();
				return;
			}
			interstitialAdResult = InterstitialAdResult.ErrorInternal;
			FinishRequest();
		}

		private void FinishRequest()
		{
			if (interstitialAdCallback == null)
			{
				Debug.LogError("Callback already done. This is probably due to multiple callbacks. For instance HandleRewardedAdFailedToShow and HandleRewardedAdClosed");
				return;
			}
			try
			{
				interstitialAdCallback(interstitialAdResult);
			}
			catch (Exception exception)
			{
				Debug.LogException(exception);
			}
			finally
			{
				prevInterstitialTime = DateTime.Now;
				interstitialAdResult = InterstitialAdResult.Done;
				interstitialAdCallback = null;
				isHandlingRequest = false;
			}
		}

		public override string ToString()
		{
			bool flag = interstitial != null;
			bool flag2 = false;
			if (flag)
			{
				flag2 = interstitial.IsLoaded();
			}
			return $"interstitialAdCreated {flag}\n isHandlingRequest {isHandlingRequest}\n adLoadState {adLoadState}\n isLoaded {flag2}\n";
		}

		public void HandleInterstitialLoaded(object sender, EventArgs args)
		{
			adLoadState.loadingAd = false;
			if (isHandlingRequest)
			{
				interstitial.Show();
			}
			Debug.Log("HandleInterstitialLoaded event received");
		}

		public void HandleInterstitialFailedToLoad(object sender, AdFailedToLoadEventArgs args)
		{
			Debug.Log("HandleInterstitialFailedToLoad event received with message: " + args.Message);
			adLoadState.loadingAd = false;
			HandleError();
		}

		public void HandleInterstitialOpened(object sender, EventArgs args)
		{
			Debug.Log("HandleInterstitialOpened event received");
			adLoadState.ResetAttempts();
		}

		public void HandleInterstitialClosed(object sender, EventArgs args)
		{
			Debug.Log("HandleInterstitialClosed event received");
			interstitialAdResult = InterstitialAdResult.Done;
			FinishRequest();
			CreateAndLoadInterstitialAd();
		}

		public void HandleInterstitialLeftApplication(object sender, EventArgs args)
		{
			Debug.Log("HandleInterstitialLeftApplication event received");
		}
	}

	private class InternalStateRewardedAd
	{
		private DateTime prevInterstitialTime = DateTime.MinValue;

		private AdLoadState adLoadState = new AdLoadState();

		private RewardedAdResult rewardAdResult = RewardedAdResult.RewardNotUnlocked;

		private Action<RewardedAdResult> rewardedAdCallback;

		private RewardedAd rewardedAd;

		private bool isHandlingRequest;

		public TimeSpan TimeSinceLastRewarded => DateTime.Now - prevInterstitialTime;

		public bool IsHandlingRequest => isHandlingRequest;

		public bool IsOk => adLoadState.IsOk;

		public void CreateAndLoadRewardedAd()
		{
			Debug.Log("InitializeRewardedAd");
			SendStat("Ad.RewardedLoad");
			if (rewardedAd != null)
			{
				DestroyRewardedAd();
			}
			rewardedAd = new RewardedAd(MobileAdManagerCredentials.GetAdMobCredentials().RewardedAdUnitId);
			SetupCallbacks();
			LoadRewardedAd();
		}

		public void RequestRewardedAd(Action<RewardedAdResult> rewardedAdCallback)
		{
			this.rewardedAdCallback = rewardedAdCallback;
			isHandlingRequest = true;
			if (rewardedAd.IsLoaded())
			{
				rewardedAd.Show();
			}
			else if (!adLoadState.loadingAd)
			{
				CreateAndLoadRewardedAd();
			}
		}

		private void SetupCallbacks()
		{
			rewardedAd.OnAdLoaded += HandleRewardedAdLoaded;
			rewardedAd.OnAdFailedToLoad += HandleRewardedAdFailedToLoad;
			rewardedAd.OnAdOpening += HandleRewardedAdOpening;
			rewardedAd.OnAdFailedToShow += HandleRewardedAdFailedToShow;
			rewardedAd.OnUserEarnedReward += HandleUserEarnedReward;
			rewardedAd.OnAdClosed += HandleRewardedAdClosed;
		}

		private void LoadRewardedAd()
		{
			if (adLoadState.loadingAd)
			{
				Debug.LogError("Already loading ad");
				return;
			}
			Debug.LogWarning("########################Load ad");
			adLoadState.loadingAd = true;
			rewardedAd.LoadAd(CreateAdRequest());
		}

		private void HandleRewardedAdOpening(object sender, EventArgs args)
		{
			Debug.Log("HandleRewardedAdOpening event received");
			adLoadState.ResetAttempts();
		}

		private void HandleRewardedAdLoaded(object sender, EventArgs args)
		{
			Debug.Log("HandleRewardedAdLoaded event received");
			adLoadState.loadingAd = false;
			if (isHandlingRequest)
			{
				rewardedAd.Show();
			}
		}

		private void HandleRewardedAdFailedToLoad(object sender, AdErrorEventArgs args)
		{
			Debug.LogError("HandleRewardedAdFailedToLoad event received with message: " + args.Message);
			adLoadState.loadingAd = false;
			HandleError();
		}

		private void HandleRewardedAdFailedToShow(object sender, AdErrorEventArgs args)
		{
			Debug.LogError("HandleRewardedAdFailedToShow event received with message: " + args.Message);
			HandleError();
		}

		private void HandleError()
		{
			if (adLoadState.Reload())
			{
				CreateAndLoadRewardedAd();
				return;
			}
			rewardAdResult = RewardedAdResult.ErrorInternal;
			FinishRequest();
		}

		private void HandleRewardedAdClosed(object sender, EventArgs args)
		{
			Debug.Log("HandleRewardedAdClosed event received");
			FinishRequest();
			CreateAndLoadRewardedAd();
		}

		private void HandleUserEarnedReward(object sender, Reward args)
		{
			string type = args.Type;
			Debug.Log("HandleRewardedAdRewarded event received for " + args.Amount + " " + type);
			rewardAdResult = RewardedAdResult.RewardUnlocked;
		}

		private void FinishRequest()
		{
			if (rewardedAdCallback == null)
			{
				Debug.LogError("Callback already done. This is probably due to multiple callbacks. For instance HandleRewardedAdFailedToShow and HandleRewardedAdClosed");
				return;
			}
			try
			{
				rewardedAdCallback(rewardAdResult);
			}
			catch (Exception exception)
			{
				Debug.LogException(exception);
			}
			finally
			{
				rewardAdResult = RewardedAdResult.RewardNotUnlocked;
				rewardedAdCallback = null;
				isHandlingRequest = false;
				prevInterstitialTime = DateTime.Now;
			}
		}

		public void Destroy()
		{
			rewardedAdCallback = null;
			DestroyRewardedAd();
		}

		private void DestroyRewardedAd()
		{
			RemoveCallbacks();
			rewardedAd = null;
		}

		public override string ToString()
		{
			bool flag = rewardedAd != null;
			bool flag2 = false;
			if (flag)
			{
				flag2 = rewardedAd.IsLoaded();
			}
			return $"rewardedAdCreated {flag}\n isHandlingRequest {isHandlingRequest}\n adLoadState {adLoadState}\n isLoaded {flag2}\n";
		}

		private void RemoveCallbacks()
		{
			rewardedAd.OnAdLoaded -= HandleRewardedAdLoaded;
			rewardedAd.OnAdFailedToLoad -= HandleRewardedAdFailedToLoad;
			rewardedAd.OnAdOpening -= HandleRewardedAdOpening;
			rewardedAd.OnAdFailedToShow -= HandleRewardedAdFailedToShow;
			rewardedAd.OnUserEarnedReward -= HandleUserEarnedReward;
			rewardedAd.OnAdClosed -= HandleRewardedAdClosed;
		}
	}

	private class InterstitialAdResultHandler
	{
		public bool IsDone;

		private readonly Action<InterstitialAdResult> interstitialCallback;

		private InterstitialAdResult interstitialAdResult;

		private IAdUIManager adUIManager;

		private AdContext context;

		public InterstitialAdResultHandler(Action<InterstitialAdResult> interstitialCallback, IAdUIManager adUIManager, AdContext context)
		{
			this.context = context;
			this.adUIManager = adUIManager;
			this.interstitialCallback = interstitialCallback;
		}

		public void SetResult(InterstitialAdResult interstitialAdResult)
		{
			IsDone = true;
			this.interstitialAdResult = interstitialAdResult;
		}

		public void DoCallBack()
		{
			try
			{
				SendStat("Ad.InterstitialFinished." + context);
				if (interstitialAdResult == InterstitialAdResult.Done)
				{
					SendStat("Ad.InterstitialShown");
					SendStat(string.Concat("Ad.InterstitialFinished.", context, ".Success"));
				}
				else
				{
					SendStat(string.Concat("Ad.InterstitialFinished.", context, ".Failure"));
				}
				adUIManager.PopInterstitial(interstitialAdResult);
				interstitialCallback(interstitialAdResult);
			}
			catch (Exception exception)
			{
				Debug.LogException(exception);
			}
		}
	}

	private class RewardedAdResultHandler
	{
		public bool IsDone;

		private readonly Action<RewardedAdResult> rewardedAdCallback;

		private RewardedAdResult rewardedAdResult;

		private IAdUIManager adUIManager;

		private AdContext context;

		public RewardedAdResultHandler(Action<RewardedAdResult> rewardedAdCallback, IAdUIManager adUIManager, AdContext context)
		{
			this.context = context;
			this.adUIManager = adUIManager;
			this.rewardedAdCallback = rewardedAdCallback;
		}

		public void SetResult(RewardedAdResult rewardedAdResult)
		{
			IsDone = true;
			this.rewardedAdResult = rewardedAdResult;
		}

		public void DoCallBack()
		{
			try
			{
				SendStat("Ad.RewardedFinished." + context);
				if (rewardedAdResult == RewardedAdResult.RewardUnlocked)
				{
					SendStat("Ad.RewardedShown");
					SendStat(string.Concat("Ad.RewardedFinished.", context, ".Success"));
				}
				else
				{
					SendStat(string.Concat("Ad.RewardedFinished.", context, ".Failure"));
				}
				adUIManager.PopRewardedVideo(rewardedAdResult);
				rewardedAdCallback(rewardedAdResult);
			}
			catch (Exception exception)
			{
				Debug.LogException(exception);
			}
		}
	}

	private class ConsentAndCompliance
	{
		private ConsentData consentData;

		public bool DoTagForChildDirectedTreatment => consentData.isChild && consentData.isAmerican;

		public bool TagForUnderAgeOfConsent => consentData.isChild && consentData.isEuropean;

		public bool HasConsented
		{
			get
			{
				bool flag = !consentData.isEuropean || !consentData.isChild;
				bool flag2 = consentData.isEuropean || consentData.isAmerican;
				return (consentData.hasConsented && flag) || !flag2;
			}
		}

		public bool IsGDPRConsentRequired => consentData.isEuropean;

		public ConsentAndCompliance()
		{
			consentData = new ConsentData();
		}

		public ConsentAndCompliance(ConsentData consentData)
		{
			this.consentData = consentData;
		}

		public override string ToString()
		{
			return consentData.ToString();
		}
	}

	private class InternalAdManagerState
	{
		private bool isReady;

		private InternalStateRewardedAd internalStateRewardedAds;

		private InternalStateInterstitial internalStateInterstitial;

		public TimeSpan TimeSinceLastAd => new TimeSpan(Math.Min(TimeSinceLastInterstitial.Ticks, TimeSinceLastRewarded.Ticks));

		public TimeSpan TimeSinceLastInterstitial => internalStateInterstitial.TimeSinceLastInterstitial;

		public TimeSpan TimeSinceLastRewarded => internalStateRewardedAds.TimeSinceLastRewarded;

		public bool IsReady => isReady;

		public bool ReadyForRewardedAdRequest => IsReady && !IsHandlingRequest && internalStateRewardedAds.IsOk;

		public bool ReadyForInterstitialAdRequest => IsReady && !IsHandlingRequest && internalStateInterstitial.IsOk;

		public bool IsHandlingRequest => internalStateRewardedAds.IsHandlingRequest || internalStateInterstitial.IsHandlingRequest;

		public InternalAdManagerState()
		{
			internalStateRewardedAds = new InternalStateRewardedAd();
			internalStateInterstitial = new InternalStateInterstitial();
		}

		public void Initialize()
		{
			internalStateRewardedAds.CreateAndLoadRewardedAd();
			internalStateInterstitial.CreateAndLoadInterstitialAd();
			isReady = true;
		}

		public void RequestRewardedAd(Action<RewardedAdResult> rewardedAdCallback)
		{
			if (!ReadyForRewardedAdRequest)
			{
				Debug.LogError("Rewarded ad request is not ready. Request should not have been made.");
				rewardedAdCallback(RewardedAdResult.ErrorClient);
			}
			else
			{
				internalStateRewardedAds.RequestRewardedAd(rewardedAdCallback);
			}
		}

		public void RequestInterstitial(Action<InterstitialAdResult> interstitialCallback)
		{
			if (!ReadyForInterstitialAdRequest)
			{
				Debug.LogError("Interstitial ad request is not ready. Request should not have been made.");
				interstitialCallback(InterstitialAdResult.ErrorClient);
			}
			else
			{
				internalStateInterstitial.RequestInterstitialAd(interstitialCallback);
			}
		}

		public void Destroy()
		{
			internalStateRewardedAds.Destroy();
			internalStateInterstitial.Destroy();
		}

		public override string ToString()
		{
			return $"isReady {isReady}\n internalStateInterstitial {internalStateInterstitial}\n internalStateRewardedAd {internalStateRewardedAds}\n";
		}
	}

	private static ConsentAndCompliance consentAndCompliance;

	private bool isInitialized;

	private static bool testing;

	private RewardedAdResultHandler rewardedAdResultHandler;

	private InterstitialAdResultHandler interstitialAdResultHandler;

	private InternalAdManagerState internalAdManagerState;

	private IAdUIManager adUIManager;

	public string RewardedAdNotAvailableText => TM._("Please try again later.");

	public TimeSpan TimeSinceLastAd => internalAdManagerState.TimeSinceLastAd;

	public TimeSpan TimeSinceLastInterstitial => internalAdManagerState.TimeSinceLastInterstitial;

	public TimeSpan TimeSinceLastRewarded => internalAdManagerState.TimeSinceLastRewarded;

	public bool ReadyForRewardedAdRequest => internalAdManagerState.ReadyForRewardedAdRequest;

	public bool ReadyForInterstitialAdRequest => internalAdManagerState.ReadyForRewardedAdRequest;

	public MobileAdManager(bool testing)
	{
		MobileAdManager.testing = testing;
	}

	public void Initialize()
	{
		if (isInitialized)
		{
			throw new Exception("AdManager already initialized");
		}
		string empty = string.Empty;
		Debug.Log("AdConsent: " + empty);
		ConsentData consentData = null;
		if (!string.IsNullOrEmpty(empty))
		{
			consentData = JsonConvert.DeserializeObject<ConsentData>(empty);
		}
		else
		{
			Debug.LogError("consent data string is null or empty");
		}
		SetupConsentAndCompliance(consentData);
		internalAdManagerState = new InternalAdManagerState();
		MobileAds.SetiOSAppPauseOnBackground(pause: true);
		MobileAds.Initialize(InitCompleteAction);
		MobileAds.Initialize(MobileAdManagerCredentials.GetAdMobCredentials().AppId);
		UpdateController.AddUpdateObject(this, UpdatePriority.UPDATEBUCKET_STANDARD);
		isInitialized = true;
		internalAdManagerState.Initialize();
	}

	public void InitializeCallbackManager(IAdUIManager adUIManager)
	{
		this.adUIManager = adUIManager;
	}

	private void SetupConsentAndCompliance(ConsentData consentData)
	{
		if (consentData == null)
		{
			Debug.LogError("Consent data not set. Using default consent");
			consentAndCompliance = new ConsentAndCompliance();
		}
		else
		{
			consentAndCompliance = new ConsentAndCompliance(consentData);
		}
		Debug.Log("consentAndCompliance: " + consentAndCompliance.ToString());
		SetConsent(consentAndCompliance.HasConsented, consentAndCompliance.IsGDPRConsentRequired);
	}

	public void RequestRewardedAd(Action<RewardedAdResult> rewardedAdCallback, AdContext context)
	{
		if (rewardedAdResultHandler != null)
		{
			Debug.LogError("Unhandled reward handler detected");
			rewardedAdCallback(RewardedAdResult.ErrorClient);
			return;
		}
		if (adUIManager.AdShowing())
		{
			Debug.Log("Ad already showing, aborting");
			rewardedAdCallback(RewardedAdResult.ErrorClient);
			return;
		}
		SendRewardRequestStats(context);
		adUIManager.ShowRewardedVideo(RewardedAdCallback);
		rewardedAdResultHandler = new RewardedAdResultHandler(rewardedAdCallback, adUIManager, context);
		internalAdManagerState.RequestRewardedAd(RewardedAdCallback);
	}

	private void SendRewardRequestStats(AdContext context)
	{
		SendStat("Ad.RewardRequest");
		SendStat("Ad.RewardRequest." + context);
		MVGameControllerBase.OperationRequests.IncrementStatRequest(IncrementStatRequestType.RewardedAdRequest);
	}

	public void RequestInterstitial(Action<InterstitialAdResult> interstitialCallback, AdContext context)
	{
		if (interstitialAdResultHandler != null)
		{
			Debug.LogError("Unhandled interstitial ad handler detected");
			interstitialCallback(InterstitialAdResult.ErrorClient);
			return;
		}
		if (adUIManager.AdShowing())
		{
			Debug.Log("Ad already showing, aborting");
			interstitialCallback(InterstitialAdResult.ErrorClient);
			return;
		}
		SendInterstitialAdRequestStats(context);
		adUIManager.ShowInterstitial(InterstitialCallback);
		interstitialAdResultHandler = new InterstitialAdResultHandler(interstitialCallback, adUIManager, context);
		internalAdManagerState.RequestInterstitial(InterstitialCallback);
	}

	private void SendInterstitialAdRequestStats(AdContext context)
	{
		SendStat("Ad.InterstitialRequest." + context);
		SendStat("Ad.InterstitialRequest");
		MVGameControllerBase.OperationRequests.IncrementStatRequest(IncrementStatRequestType.InterstitialAdRequest);
	}

	private void InterstitialCallback(InterstitialAdResult obj)
	{
		interstitialAdResultHandler.SetResult(obj);
	}

	private void RewardedAdCallback(RewardedAdResult obj)
	{
		rewardedAdResultHandler.SetResult(obj);
	}

	public void Destroy()
	{
		UpdateController.RemoveUpdateObject(this);
		rewardedAdResultHandler = null;
		interstitialAdResultHandler = null;
		internalAdManagerState.Destroy();
		internalAdManagerState = null;
		isInitialized = false;
	}

	public override string ToString()
	{
		if (internalAdManagerState == null)
		{
			return "internalAdManagerState not initialized";
		}
		return $"{consentAndCompliance}\n{internalAdManagerState}";
	}

	public void UpdateControllerUpdate()
	{
		if (rewardedAdResultHandler != null && rewardedAdResultHandler.IsDone)
		{
			rewardedAdResultHandler.DoCallBack();
			rewardedAdResultHandler = null;
		}
		if (interstitialAdResultHandler != null && interstitialAdResultHandler.IsDone)
		{
			interstitialAdResultHandler.DoCallBack();
			interstitialAdResultHandler = null;
		}
	}

	private void SetConsent(bool hasConsented, bool isGDPRConsentRequired)
	{
		SetConsentVungle(hasConsented);
		SetConsentUnityAds(hasConsented);
		SetConsentTapJoy(hasConsented, isGDPRConsentRequired);
		SetConsentIronSource(hasConsented);
	}

	private static AdRequest CreateAdRequest()
	{
		if (consentAndCompliance.DoTagForChildDirectedTreatment && consentAndCompliance.TagForUnderAgeOfConsent)
		{
			return new AdRequest.Builder().AddTestDevice("2F722B7F88436E816B98A0245E195219").TagForChildDirectedTreatment(tagForChildDirectedTreatment: true).AddExtra("tag_for_under_age_of_consent", "true")
				.Build();
		}
		if (consentAndCompliance.DoTagForChildDirectedTreatment)
		{
			return new AdRequest.Builder().AddTestDevice("2F722B7F88436E816B98A0245E195219").TagForChildDirectedTreatment(tagForChildDirectedTreatment: true).Build();
		}
		if (consentAndCompliance.TagForUnderAgeOfConsent)
		{
			return new AdRequest.Builder().AddTestDevice("2F722B7F88436E816B98A0245E195219").AddExtra("tag_for_under_age_of_consent", "true").Build();
		}
		return new AdRequest.Builder().AddTestDevice("2F722B7F88436E816B98A0245E195219").Build();
	}

	private void SetConsentVungle(bool hasConsented)
	{
		VungleConsent consentStatus = VungleConsent.ACCEPTED;
		if (!hasConsented)
		{
			consentStatus = VungleConsent.DENIED;
		}
		Vungle.UpdateConsentStatus(consentStatus);
	}

	private void SetConsentUnityAds(bool hasConsented)
	{
		UnityAds.SetGDPRConsentMetaData(hasConsented);
	}

	private void SetConsentTapJoy(bool hasConsented, bool isGDPRConsentRequired)
	{
		Tapjoy.SubjectToGDPR(isGDPRConsentRequired);
		Tapjoy.SetUserConsent(BoolToString(hasConsented));
	}

	private void SetConsentIronSource(bool hasConsented)
	{
		IronSource.SetConsent(hasConsented);
	}

	private static string BoolToString(bool b)
	{
		if (b)
		{
			return "1";
		}
		return "0";
	}

	private static void SendStat(string stat)
	{
		if (!testing)
		{
			StatHatWrapper.Count(stat, 1);
		}
	}

	private void InitCompleteAction(InitializationStatus initializationStatus)
	{
		Debug.Log("InitCompleteAction#######################################");
		foreach (KeyValuePair<string, AdapterStatus> item in initializationStatus.getAdapterStatusMap())
		{
			if (item.Value.InitializationState == AdapterState.Ready)
			{
				Debug.LogFormat("{0}. {1}.", item.Value.Description, item.Value.InitializationState);
			}
			else
			{
				Debug.LogErrorFormat("{0}. {1}.", item.Value.Description, item.Value.InitializationState);
			}
		}
	}
}
