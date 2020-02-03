using Assets.Scripts.AdIntegration;
using MV.Common;
using UnityEngine;
using UnityEngine.EventSystems;

public class RegisteredPromotionController : MonoBehaviour, IRegisterPromotionAdController, IEventSystemHandler
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

	private bool isDead;

	private bool embedded;

	private bool subscriber;

	private float timeBeforeShownPromotion = 180f;

	private const float showAdDelay = 1.26f;

	public void Initialize()
	{
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

	private void Update()
	{
		if (MVGameControllerBase.GameMode != MVGameMode.Play)
		{
			return;
		}
		timer += Time.deltaTime;
		if (MVGameControllerBase.SpawnRoleDataMediatorLocal.SpawnRoleModeTypeWrapper.IsInMode(SpawnRoleModeType.Dead))
		{
			if (!isDead && timer >= timeBeforeShownPromotion && Time.time > MVGameControllerBase.LocalPlayer.RespawnTime - (MVGameControllerBase.LocalPlayer.RespawnDuration - 1.26f))
			{
				isDead = true;
				ShowRegisteredPromotionPopup();
			}
		}
		else
		{
			isDead = false;
		}
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
	}

	private void PushPromotionSlide(RegisteredPromotionPopup popupPrefab, bool isEmbeddedPromotion)
	{
		RegisteredPromotionPopup registeredPromotion = Object.Instantiate(popupPrefab);
		registeredPromotion.Initialize(isEmbeddedPromotion);
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
		{
			x.Push(registeredPromotion.gameObject, UIPushOption.InvisibleBlocker, null, UIGroupFlags.Popup);
		});
		timer = 0f;
	}
}
