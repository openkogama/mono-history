using Assets.Scripts.AdIntegration;
using MV.Common;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class TouristAdController : MonoBehaviour, ITouristAdController, IPromotionController, IEventSystemHandler
{
	[SerializeField]
	private TouristPromotion touristPromotionPrefab;

	[SerializeField]
	private EmbeddedPlayerConfig embeddedPlayerConfig;

	[SerializeField]
	private TouristPromotionExternalEvaluator touristPromotionExternalEvaluator;

	[SerializeField]
	private float timeBeforeAdShown = 180f;

	private float timer;

	private UnityAction<bool, bool> onPromotionWasPopped;

	private bool eligableForPromotion;

	private bool withAd;

	public bool ReadyForAd => MVClientSettings.InterstitialsAdsEnabled && timer >= timeBeforeAdShown;

	public bool IsPromotionAvailable => !MVGameControllerBase.SpawnRoleDataMediatorLocal.SpawnRoleModeTypeWrapper.IsInMode(SpawnRoleModeType.Playing) && TouristPromotionAllowed() && eligableForPromotion;

	public void Initialize()
	{
		enabled = true;
		MVGameControllerBase.SpawnRoleDataMediatorLocal.SpawnRoleMode.OnChange += OnChangeMode;
	}

	private void OnChangeMode(SpawnRoleModeType type)
	{
		if (type == SpawnRoleModeType.Dead)
		{
			eligableForPromotion = true;
		}
	}

	private void OnPromotionPopped()
	{
		if (onPromotionWasPopped != null)
		{
			onPromotionWasPopped(arg0: true, withAd);
		}
	}

	public void ShowPromotion(UnityAction<bool, bool> onPop)
	{
		eligableForPromotion = false;
		onPromotionWasPopped = onPop;
		withAd = MVClientSettings.InterstitialsAdsEnabled && timer >= timeBeforeAdShown && MVGameControllerBase.AdManager.ReadyForInterstitialAdRequest;
		if (MVClientSettings.ShowTouristPromotion && embeddedPlayerConfig.GetCurrentSiteData().showTouristPromotion)
		{
			if (touristPromotionExternalEvaluator == null || !touristPromotionExternalEvaluator.TryGetExternalPromotion(out var externalPromotion))
			{
				externalPromotion = touristPromotionPrefab;
			}
			TouristPromotion createdPromotion = Object.Instantiate(externalPromotion);
			createdPromotion.Initialize(withAd);
			ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
			{
				x.Push(createdPromotion.gameObject, UIPushOption.Blocking, OnPromotionPopped, UIGroupFlags.Popup);
			});
		}
		else if (withAd)
		{
			MVGameControllerBase.AdManager.RequestInterstitial(ShowAdWithoutPromotion, AdContext.TouristInterstitialWithoutPromotion);
		}
		else
		{
			OnPromotionPopped();
		}
	}

	public void ShowAdWithoutPromotion(InterstitialAdResult obj)
	{
		timer = 0f;
		if (onPromotionWasPopped != null)
		{
			onPromotionWasPopped(arg0: false, withAd);
		}
	}

	private bool TouristPromotionAllowed()
	{
		return MVGameControllerBase.IsTouristSession && MVGameControllerBase.Game.IsPlaying && MVGameControllerBase.JoinState == MVJoinState.Playing;
	}

	public void ShowAd()
	{
		MVGameControllerBase.AdManager.RequestInterstitial(InterstitialAdResult, AdContext.TouristPromotion);
	}

	public void InterstitialAdResult(InterstitialAdResult obj)
	{
		timer = 0f;
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
		{
			x.Pop();
		});
	}

	private void Update()
	{
		if (MVGameControllerBase.JoinState == MVJoinState.Playing)
		{
			timer += Time.deltaTime;
		}
	}
}
