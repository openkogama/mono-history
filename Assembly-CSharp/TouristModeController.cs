using System;
using System.Collections.Generic;
using MV.Common;
using UnityEngine;
using UnityEngine.EventSystems;

public class TouristModeController : MonoBehaviour
{
	private class ShowPromotionBookkeeping
	{
		private const int deathShowFrequence = 3;

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
					if (deaths % 3 == 0)
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

		private List<Texture> promotionDatas = new List<Texture>();

		private int currentSlideIndex;

		public bool PromotionDataReady => promotionDatas.Count > 0;

		public Texture NextPromotionData
		{
			get
			{
				if (promotionDatas.Count == 0)
				{
					Debug.LogWarning("PromotionDatas.Count == 0");
					return null;
				}
				Texture result = promotionDatas[currentSlideIndex % promotionDatas.Count];
				currentSlideIndex++;
				return result;
			}
		}

		public PromotionDataManager()
		{
			DownloadSlide();
		}

		private void DownloadSlide()
		{
			try
			{
				AsyncWWWManager.WWWRequest(new CachedGetRequest(Urls.StreamingAssets + GetPath(promotionDatas.Count + 1), StreamingAssetCallback, WWWRequestPriority.WaitUntilSyncronizingIsDone));
			}
			catch (Exception ex)
			{
				Debug.LogError("ex: " + ex.Message);
			}
		}

		private string GetPath(int i)
		{
			return string.Format(baseAssetString, i.ToString("D2"));
		}

		private void StreamingAssetCallback(WWW www)
		{
			if (www.error != null)
			{
				Debug.LogWarning("www.error != null: " + www.error);
			}
			else if (www.texture != null)
			{
				promotionDatas.Add(www.texture);
				DownloadSlide();
			}
			else
			{
				Debug.Log("Failed " + www.url);
			}
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
			UnityEngine.Object.Destroy(this);
		}
	}

	private void Update()
	{
		if (showPromotionBookkeeping.Show && promotionDataManager.PromotionDataReady)
		{
			TouristPromotion promotion = UnityEngine.Object.Instantiate(touristPromotionPrefab);
			SetToPromotionData(promotion);
			showPromotionBookkeeping.Continue();
			ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
			{
				x.Push(promotion.gameObject, UIPushOption.Blocking);
			});
		}
	}

	private void SetToPromotionData(TouristPromotion promotion)
	{
		if (promotionDataManager != null)
		{
			Texture nextPromotionData = promotionDataManager.NextPromotionData;
			if (nextPromotionData == null)
			{
				Debug.LogWarning("PromotionData is null");
			}
			else
			{
				promotion.SetPromotionTexture(nextPromotionData);
			}
		}
	}
}
