using System.Collections.Generic;
using MV.Common;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

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

	private class PromotionDataManager
	{
		private const string baseAssetString = "Promotion/Promotion_{0}.png";

		private const int promotionCount = 4;

		private static int promotionIndex = 4;

		private UnityAction<Texture> OnTextureReadyCallback;

		private Dictionary<string, Texture2D> textureAssetCache = new Dictionary<string, Texture2D>(4);

		public void GetTextureDataToSet(UnityAction<Texture> OnTextureReady)
		{
			OnTextureReadyCallback = OnTextureReady;
			AsyncWWWManager.WWWRequest(new CachedGetRequest(Urls.StreamingAssets + GetPath(promotionIndex % 4 + 1), OnTextureReceived, WWWRequestPriority.WaitUntilSyncronizingIsDone));
		}

		public void GetRandomTextureData(UnityAction<Texture> OnTextureReady)
		{
			OnTextureReadyCallback = OnTextureReady;
			int num = promotionIndex % 4;
			string path = Urls.StreamingAssets + GetPath(Random.Range(num, num + 4) + 1);
			AsyncWWWManager.WWWRequest(new CachedGetRequest(path, OnTextureReceived, WWWRequestPriority.WaitUntilSyncronizingIsDone));
		}

		public void Destroy()
		{
			AsyncWWWManager.UnsubscribeWWWRequest(OnTextureReceived);
			foreach (KeyValuePair<string, Texture2D> item in textureAssetCache)
			{
				Object.Destroy(item.Value);
			}
			textureAssetCache.Clear();
		}

		private void OnTextureReceived(WWW www)
		{
			Texture2D value = null;
			if (!textureAssetCache.TryGetValue(www.url, out value))
			{
				if (!string.IsNullOrEmpty(www.error))
				{
					Debug.LogError("Tourist promotion 'OnTextureReceived' failed : " + www.error);
					return;
				}
				value = www.texture;
				textureAssetCache[www.url] = value;
			}
			OnTextureReadyCallback(value);
			promotionIndex++;
		}

		private string GetPath(int i)
		{
			return string.Format("Promotion/Promotion_{0}.png", i.ToString("D2"));
		}
	}

	private PromotionDataManager promotionDataManager;

	private ShowPromotionBookkeeping showPromotionBookkeeping;

	private bool touristPromotionActive;

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
		PushPromotionSlide(touristPromotionPrefab);
		promotionDataManager.GetTextureDataToSet(SetPromotionTexture);
	}

	public void ShowAnyAndroidPromotionSlide()
	{
		PushPromotionSlide(touristPromotionAndroidPrefab);
		promotionDataManager.GetTextureDataToSet(SetPromotionTexture);
	}

	public void ShowAdPromotionSlide()
	{
		PushPromotionSlide(touristPromotionWithAdPrefab);
		promotionDataManager.GetTextureDataToSet(SetPromotionTexture);
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
		promotionDataManager = new PromotionDataManager();
		showPromotionBookkeeping = new ShowPromotionBookkeeping();
		SetActive(active: false);
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
			x.Push(promotion.gameObject, UIPushOption.Blocking, PromitionPopped, UIGroupFlags.Popup);
		});
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
