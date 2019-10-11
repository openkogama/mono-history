using Assets.Scripts.AdIntegration;
using MV.Common;
using UnityEngine;
using UnityEngine.EventSystems;

public class TouristAdController : MonoBehaviour, ITouristAdController, IEventSystemHandler
{
	[SerializeField]
	private float timeBeforeAdShown = 180f;

	private float timer;

	private bool isDead;

	private const float showAdDelay = 1.26f;

	private TouristModeController promotionSliderCreator;

	private TouristAdStateHandler adHandler = new TouristAdStateHandler();

	public void Initialize(TouristModeController promotionSliderController)
	{
		promotionSliderCreator = promotionSliderController;
		gameObject.SetActive(value: true);
		enabled = true;
	}

	public void ShowAd()
	{
		MVGameControllerBase.AdManager.RequestInterstitial(InterstitialAdResult, AdContext.TouristPromotion);
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
		if (MVGameControllerBase.JoinState != MVJoinState.Playing)
		{
			return;
		}
		timer += Time.deltaTime;
		if (MVGameControllerBase.SpawnRoleDataMediatorLocal.SpawnRoleModeTypeWrapper.IsInMode(SpawnRoleModeType.Dead))
		{
			if (!isDead)
			{
				if (timer >= timeBeforeAdShown && Time.time > MVGameControllerBase.LocalPlayer.RespawnTime - (MVGameControllerBase.LocalPlayer.RespawnDuration - 1.26f))
				{
					isDead = true;
					promotionSliderCreator.ShowAnyPromotionSlide();
					timer = 0f;
				}
				else if (Time.time > MVGameControllerBase.LocalPlayer.RespawnTime - (MVGameControllerBase.LocalPlayer.RespawnDuration - 1.26f))
				{
					isDead = true;
					MVGameControllerDesktop.LockCursorManager.CursorLock = false;
					promotionSliderCreator.ShowAnyPromotionSlide();
				}
			}
		}
		else
		{
			isDead = false;
		}
	}
}
