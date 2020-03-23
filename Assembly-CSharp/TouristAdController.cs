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
	private float timeBeforeAdShown = 180f;

	private float timer;

	private UnityAction<bool, bool> onPromotionWasPopped;

	private bool eligableForPromotion;

	private bool withAd;

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
		withAd = timer >= timeBeforeAdShown;
		TouristPromotion promotion = Object.Instantiate(touristPromotionPrefab);
		promotion.Initialize(withAd);
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
		{
			x.Push(promotion.gameObject, UIPushOption.Blocking, OnPromotionPopped, UIGroupFlags.Popup);
		});
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
