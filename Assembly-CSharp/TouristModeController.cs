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

		public void GetRandomTextureData(UnityAction<Texture> OnTextureReady)
		{
			OnTextureReadyCallback = OnTextureReady;
			int num = promotionIndex % promotionCount;
			AsyncWWWManager.WWWRequest(new CachedGetRequest(Urls.StreamingAssets + GetPath(Random.Range(num, num + promotionCount) + 1), StreamingTextureLoaded, WWWRequestPriority.WaitUntilSyncronizingIsDone));
		}

		public void Destroy()
		{
			AsyncWWWManager.UnsubscribeWWWRequest(StreamingTextureLoaded);
		}

		private void StreamingTextureLoaded(WWW www)
		{
			if (www != null && www.texture != null)
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

	[SerializeField]
	private TouristPromotion touristPromotionWithAdPrefab;

	private TouristPromotion promotion;

	public void Awake()
	{
		touristPromotionActive = MVGameControllerBase.IsTouristSession && MVClientSettings.ShowTouristPromotion;
		if (!touristPromotionActive)
		{
			Object.Destroy(this);
			return;
		}
		promotionDataManager = new PromotionDataManager();
		showPromotionBookkeeping = new ShowPromotionBookkeeping();
		SetActive(active: false);
	}

	public void SetActive(bool active)
	{
		enabled = active;
	}

	private void Update()
	{
		if (showPromotionBookkeeping.Show)
		{
			PushPromotionSlide(touristPromotionPrefab);
			promotionDataManager.GetTextureDataToSet(SetPromotionTexture);
			showPromotionBookkeeping.Continue();
		}
	}

	private void PushPromotionSlide(TouristPromotion prefab)
	{
		promotion = Object.Instantiate(prefab);
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
		{
			x.Push(promotion.gameObject, UIPushOption.Blocking, PromitionPopped);
		});
	}

	public void ShowAnyPromotionSlide()
	{
		PushPromotionSlide(touristPromotionPrefab);
		promotionDataManager.GetTextureDataToSet(SetPromotionTexture);
	}

	public void ShowAdPromotionSlide()
	{
		PushPromotionSlide(touristPromotionWithAdPrefab);
		promotionDataManager.GetTextureDataToSet(SetPromotionTexture);
	}

	private void PromitionPopped()
	{
		promotion = null;
	}

	private void SetPromotionTexture(Texture promotionTexture)
	{
		if (!(promotion == null))
		{
			promotion.SetPromotionTexture(promotionTexture);
		}
	}
}
