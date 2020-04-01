using Assets.Scripts.AdIntegration;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class RegisteredPromotionController : MonoBehaviour, IRegisterPromotionAdController, IPromotionController, IEventSystemHandler
{
	[SerializeField]
	private RegisteredPromotionPopup registeredPromotionPopupPrefab;

	[SerializeField]
	private RegisteredPromotionPopup registeredElitePromotionPopupPrefab;

	[SerializeField]
	private float playFromKogamaPromoInterval = 60f;

	[SerializeField]
	private float joinTheElitePromoInterval = 180f;

	private float timer;

	private bool embedded;

	private bool subscriber;

	private float timeBeforeShownPromotion = 180f;

	private UnityAction<bool, bool> onPromotionWasPopped;

	public bool ReadyForAd => true;

	public bool IsPromotionAvailable => timer >= timeBeforeShownPromotion;

	public void Initialize()
	{
		enabled = true;
		joinTheElitePromoInterval = MVGameControllerBase.Game.EliteSettings.ElitePromotionInterval;
		embedded = MVGameControllerBase.GameSessionData.embedded;
		subscriber = MVClientSettings.IsSubscriber();
		timeBeforeShownPromotion = Mathf.Min(playFromKogamaPromoInterval, joinTheElitePromoInterval);
	}

	public void ShowRegisteredPromotionAd()
	{
		AdContext context = ((!embedded) ? AdContext.RegisteredElitePromotion : AdContext.RegisteredEmbeddedPromotion);
		MVGameControllerBase.AdManager.RequestInterstitial(InterstitialAdResult, context);
	}

	public void InterstitialAdResult(InterstitialAdResult obj)
	{
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
		{
			x.Pop();
		});
	}

	private void ShowRegisteredPromotionPopup()
	{
		if (embedded && !subscriber && timer > playFromKogamaPromoInterval)
		{
			PushPromotionSlide(registeredPromotionPopupPrefab, isEmbeddedPromotion: true);
		}
		else if (!subscriber && timer > joinTheElitePromoInterval && MVGameControllerBase.Game.EliteSettings.ElitePromotionEnabled)
		{
			PushPromotionSlide(registeredElitePromotionPopupPrefab, isEmbeddedPromotion: false);
		}
		else if (onPromotionWasPopped != null)
		{
			onPromotionWasPopped(arg0: false, arg1: false);
		}
	}

	private void OnPromotionPop()
	{
		if (onPromotionWasPopped != null)
		{
			onPromotionWasPopped(arg0: true, arg1: true);
		}
	}

	private void PushPromotionSlide(RegisteredPromotionPopup popupPrefab, bool isEmbeddedPromotion)
	{
		RegisteredPromotionPopup registeredPromotion = Object.Instantiate(popupPrefab);
		registeredPromotion.Initialize(isEmbeddedPromotion, MVGameControllerBase.AdManager.ReadyForInterstitialAdRequest);
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
		{
			x.Push(registeredPromotion.gameObject, UIPushOption.InvisibleBlocker, OnPromotionPop, UIGroupFlags.Popup);
		});
		timer = 0f;
	}

	private void Update()
	{
		if (MVGameControllerBase.JoinState == MVJoinState.Playing)
		{
			timer += Time.deltaTime;
		}
	}

	public void ShowPromotion(UnityAction<bool, bool> onPop)
	{
		onPromotionWasPopped = onPop;
		ShowRegisteredPromotionPopup();
	}
}
