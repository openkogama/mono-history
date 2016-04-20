using MV.Common;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class TouristModeController : MonoBehaviour
{
	private class ShowPromotionBookkeeping
	{
		private const int deathShowFrequence = 1;

		private bool isDead;

		private bool deathConditionTriggered = true;

		private float deathTime;

		private float deathTimeDelay = 1f;

		private bool gameEntered = true;

		private bool continuedClicked;

		private bool showTouristPromotion;

		private int deaths;

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
				if (!isDead && MVGameControllerBase.WOCM.AvatarLocal.IsDead)
				{
					isDead = true;
					deaths++;
					if (deaths % 1 == 0)
					{
						deathTime = Time.time;
						deathConditionTriggered = false;
					}
				}
				else if (isDead && !MVGameControllerBase.WOCM.AvatarLocal.IsDead)
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

	private class PromotionDataManager
	{
		private string baseAssetString = "Promotion/Promotion_{0}.png";

		private static int promotionIndex = 5;

		private static readonly int promotionCount = 5;

		private UnityAction<Texture> OnTextureReadyCallback;

		public void GetTextureDataToSet(UnityAction<Texture> OnTextureReady)
		{
			OnTextureReadyCallback = OnTextureReady;
			AsyncWWWManager.WWWRequest(new CachedGetRequest(Urls.StreamingAssets + GetPath(promotionIndex % promotionCount + 1), StreamingTextureLoaded, WWWRequestPriority.WaitUntilSyncronizingIsDone));
		}

		private void StreamingTextureLoaded(WWW www)
		{
			if (string.IsNullOrEmpty(www.error))
			{
				promotionIndex++;
				OnTextureReadyCallback(www.texture);
			}
			else
			{
				Debug.LogError("Tourist promotion 'StreamingTextureLoaded' failed : " + www.error);
			}
		}

		private string GetPath(int i)
		{
			return string.Format(baseAssetString, i.ToString("D2"));
		}
	}

	private PromotionDataManager promotionDataManager;

	private ShowPromotionBookkeeping showPromotionBookkeeping;

	private bool touristPromotionActive;

	[SerializeField]
	private TouristPromotion touristPromotionPrefab;

	public void Awake()
	{
		touristPromotionActive = MVGameControllerBase.IsTouristSession && MVClientSettings.ShowTouristPromotion;
		if (touristPromotionActive)
		{
			showPromotionBookkeeping = new ShowPromotionBookkeeping();
			promotionDataManager = new PromotionDataManager();
		}
	}

	private void Start()
	{
		if (!touristPromotionActive)
		{
			Object.Destroy(this);
		}
	}

	private void Update()
	{
		if (showPromotionBookkeeping.Show)
		{
			promotionDataManager.GetTextureDataToSet(SetPromotionTexture);
			showPromotionBookkeeping.Continue();
		}
	}

	private void SetPromotionTexture(Texture promotionTexture)
	{
		TouristPromotion promotion = Object.Instantiate(touristPromotionPrefab);
		promotion.SetPromotionTexture(promotionTexture);
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
		{
			x.Push(promotion.gameObject, UIPushOption.Blocking);
		});
	}
}
