using MV.Common;
using UnityEngine;
using UnityEngine.EventSystems;

public class TouristModeController : MonoBehaviour
{
	private class ShowPromotionBookkeeping
	{
		private bool isDead;

		private bool deathConditionTriggered = true;

		private float deathTime;

		private float deathTimeDelay = 1f;

		private bool gameEntered = true;

		private bool continuedClicked;

		private bool showTouristPromotion;

		private int deaths;

		private const int deathShowFrequence = 1;

		private static bool TouristPromotionAllowed => MVGameControllerBase.IsTouristSession && MVGameControllerBase.Game.IsPlaying && MVGameControllerBase.JoinState == MVJoinState.Playing;

		public bool Show => ShowPromotion();

		public void Continue()
		{
			continuedClicked = true;
		}

		private bool ShowPromotion()
		{
			showTouristPromotion |= DeadShowCondition();
			showTouristPromotion |= showTouristPromotion;
			bool flag = ContinuedClicked();
			showTouristPromotion = showTouristPromotion && !flag;
			return showTouristPromotion;
		}

		private bool ContinuedClicked()
		{
			if (continuedClicked)
			{
				Debug.Log("continuedClicked " + continuedClicked);
				continuedClicked = false;
				return true;
			}
			return false;
		}

		private bool GameEnteredCondition()
		{
			if (TouristPromotionAllowed && gameEntered)
			{
				gameEntered = false;
				return true;
			}
			return false;
		}

		private bool DeadShowCondition()
		{
			if (TouristPromotionAllowed)
			{
				if (!isDead && MVGameControllerBase.SpawnRoleDataMediatorLocal.SpawnRoleModeTypeWrapper.IsInMode(SpawnRoleModeType.Dead))
				{
					isDead = true;
					deaths++;
					if (deaths % 1 == 0)
					{
						deathTime = Time.time;
						deathConditionTriggered = false;
					}
				}
				else if (isDead && !MVGameControllerBase.SpawnRoleDataMediatorLocal.SpawnRoleModeTypeWrapper.IsInMode(SpawnRoleModeType.Dead))
				{
					isDead = false;
				}
				if (!deathConditionTriggered && Time.time - deathTime > deathTimeDelay)
				{
					deathConditionTriggered = true;
					Debug.Log("Returning true " + deaths);
					return true;
				}
			}
			return false;
		}
	}

	private ShowPromotionBookkeeping showPromotionBookkeeping;

	private bool touristPromotionActive;

	[SerializeField]
	private TouristPromotionExternalEvaluator touristPromotionExternalEvaluator;

	[SerializeField]
	private TouristPromotion touristPromotionPrefab;

	[SerializeField]
	private TouristPromotion touristPromotionWithAdPrefab;

	[SerializeField]
	private TouristPromotion touristPromotionWithAndroidAdPrefab;

	[SerializeField]
	private TouristPromotion touristPromotionAndroidPrefab;

	private TouristPromotion promotion;

	public void SetActive(bool active)
	{
		enabled = active;
	}

	public void PopPromotions()
	{
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
		{
			x.PopToGroup(UIGroupFlags.MainUI);
		});
	}

	public void ShowAnyPromotionSlide()
	{
		if (touristPromotionExternalEvaluator.TryGetExternalPromotion(out var externalPromotion))
		{
			PushPromotionSlide(externalPromotion, withAd: false);
		}
		else
		{
			PushPromotionSlide(touristPromotionPrefab, withAd: false);
		}
	}

	public void ShowAnyAndroidPromotionSlide()
	{
		PushPromotionSlide(touristPromotionAndroidPrefab, withAd: false);
	}

	public void ShowAdPromotionSlide()
	{
		if (touristPromotionExternalEvaluator.TryGetExternalPromotion(out var externalPromotion))
		{
			PushPromotionSlide(externalPromotion, withAd: true);
		}
		else
		{
			PushPromotionSlide(touristPromotionWithAdPrefab, withAd: true);
		}
	}

	private void Awake()
	{
		touristPromotionActive = MVGameControllerBase.IsTouristSession && MVClientSettings.ShowTouristPromotion;
		touristPromotionActive &= !MVGameControllerBase.GameSessionData.IsPlayedFromPoki;
		if (!touristPromotionActive)
		{
			Object.Destroy(this);
			return;
		}
		showPromotionBookkeeping = new ShowPromotionBookkeeping();
		SetActive(active: false);
	}

	private void Update()
	{
		if (showPromotionBookkeeping.Show)
		{
			PushPromotionSlide(touristPromotionPrefab, withAd: false);
			showPromotionBookkeeping.Continue();
		}
	}

	private void PushPromotionSlide(TouristPromotion prefab, bool withAd)
	{
		promotion = Object.Instantiate(prefab);
		promotion.Initialize(withAd);
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
		{
			x.Push(promotion.gameObject, UIPushOption.Blocking, PromitionPopped, UIGroupFlags.Popup);
		});
	}

	private void PromitionPopped()
	{
		promotion = null;
	}
}
