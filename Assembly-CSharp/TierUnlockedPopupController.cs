using System.Collections.Generic;
using MV.Common;
using MV.WorldObject;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TierUnlockedPopupController : MonoBehaviour
{
	[SerializeField]
	private Image Background;

	[SerializeField]
	private NotificationFade closeButtonFader;

	[SerializeField]
	private TierUnlockedPopupContentTierUnlocked PopupContentTierUnlockedPrefab;

	[SerializeField]
	private TierUnlockedPopupContentXP PopupContentXPPrefab;

	[SerializeField]
	private TierUnlockedPopupContentSpawnRole popupContentSpawnRolePrefab;

	[SerializeField]
	private TierUnlockedPopupContentBase PopupContentCreatorSupportPrefab;

	[SerializeField]
	private TierTempUnlockedInformationPopupContent popupContentTempUnlockInformationPrefab;

	[SerializeField]
	private TierUnlockedPopupContentTierTempUnlocked popupContentTierTempUnlockPrefab;

	[SerializeField]
	private TierUnlockedPopupContentLoot PopupContentLootPrefab;

	[SerializeField]
	private TierUnlockedPopupContentAccess PopupContentAccessPrefab;

	[SerializeField]
	private float fadeDuration;

	[SerializeField]
	private float bounceEffectDuration;

	[SerializeField]
	private float colorInterpolationDuration;

	[SerializeField]
	private AnimationCurve bounceEffect;

	[SerializeField]
	private AnimationCurve fadeEffect;

	private List<TierUnlockedPopupContentBase> popupContentList;

	private int currentContentBeingShowed;

	private GamePassTier unlockedTier;

	private bool isPoppingCountdownStarted;

	private float popTime;

	private Color interpolateToColor;

	private float interpolateColorStartTime;

	private float bounceEffectStartTime;

	private float fadeEffectStartTime;

	public static GamePassTier HighestTierRewardShown;

	public void Initialize(GamePassTier unlockedTier, bool wasPurchased, bool wasTempUnlocked)
	{
		this.unlockedTier = unlockedTier;
		popupContentList = new List<TierUnlockedPopupContentBase>();
		if (!wasTempUnlocked)
		{
			TierUnlockedPopupContentTierUnlocked tierUnlockedPopupContentTierUnlocked = Object.Instantiate(PopupContentTierUnlockedPrefab);
			tierUnlockedPopupContentTierUnlocked.transform.SetParent(transform, worldPositionStays: false);
			popupContentList.Add(tierUnlockedPopupContentTierUnlocked);
			TierUnlockedPopupContentXP tierUnlockedPopupContentXP = Object.Instantiate(PopupContentXPPrefab);
			tierUnlockedPopupContentXP.transform.SetParent(transform, worldPositionStays: false);
			tierUnlockedPopupContentXP.gameObject.SetActive(value: false);
			popupContentList.Add(tierUnlockedPopupContentXP);
		}
		else
		{
			TierUnlockedPopupContentTierTempUnlocked tierUnlockedPopupContentTierTempUnlocked = Object.Instantiate(popupContentTierTempUnlockPrefab);
			tierUnlockedPopupContentTierTempUnlocked.transform.SetParent(transform, worldPositionStays: false);
			popupContentList.Add(tierUnlockedPopupContentTierTempUnlocked);
		}
		if (ShouldShowLootPopup())
		{
			TierUnlockedPopupContentLoot tierUnlockedPopupContentLoot = Object.Instantiate(PopupContentLootPrefab);
			tierUnlockedPopupContentLoot.transform.SetParent(transform, worldPositionStays: false);
			tierUnlockedPopupContentLoot.gameObject.SetActive(value: false);
			popupContentList.Add(tierUnlockedPopupContentLoot);
		}
		if (ShouldShowAccessPopup())
		{
			TierUnlockedPopupContentAccess tierUnlockedPopupContentAccess = Object.Instantiate(PopupContentAccessPrefab);
			tierUnlockedPopupContentAccess.transform.SetParent(transform, worldPositionStays: false);
			tierUnlockedPopupContentAccess.gameObject.SetActive(value: false);
			popupContentList.Add(tierUnlockedPopupContentAccess);
		}
		List<MVWorldObjectClient> worldObjectsByType = MVGameControllerBase.Game.WorldObjectClientManager.GetWorldObjectsByType(WorldObjectType.AvatarSpawnRoleCreator);
		for (int i = 0; i < worldObjectsByType.Count; i++)
		{
			if (worldObjectsByType[i] is MVAvatarSpawnRoleCreator && ((MVAvatarSpawnRoleCreator)worldObjectsByType[i]).Tier == unlockedTier)
			{
				TierUnlockedPopupContentSpawnRole tierUnlockedPopupContentSpawnRole = Object.Instantiate(popupContentSpawnRolePrefab);
				tierUnlockedPopupContentSpawnRole.SetupPreviewImage(((MVAvatarSpawnRoleCreator)worldObjectsByType[i]).GetSpawnRolePreviewObject());
				tierUnlockedPopupContentSpawnRole.SetupColor(((MVAvatarSpawnRoleCreator)worldObjectsByType[i]).Team);
				tierUnlockedPopupContentSpawnRole.transform.SetParent(transform, worldPositionStays: false);
				tierUnlockedPopupContentSpawnRole.gameObject.SetActive(value: false);
				popupContentList.Add(tierUnlockedPopupContentSpawnRole);
			}
		}
		if (wasPurchased)
		{
			TierUnlockedPopupContentBase tierUnlockedPopupContentBase = Object.Instantiate(PopupContentCreatorSupportPrefab);
			tierUnlockedPopupContentBase.transform.SetParent(transform, worldPositionStays: false);
			tierUnlockedPopupContentBase.gameObject.SetActive(value: false);
			popupContentList.Add(tierUnlockedPopupContentBase);
		}
		if (wasTempUnlocked)
		{
			TierTempUnlockedInformationPopupContent tierTempUnlockedInformationPopupContent = Object.Instantiate(popupContentTempUnlockInformationPrefab);
			tierTempUnlockedInformationPopupContent.transform.SetParent(transform, worldPositionStays: false);
			tierTempUnlockedInformationPopupContent.gameObject.SetActive(value: false);
			popupContentList.Add(tierTempUnlockedInformationPopupContent);
		}
		StartNewPopupContent(0);
		Background.color = popupContentList[0].BackgroundColor;
		HighestTierRewardShown = unlockedTier;
	}

	public void ShowCloseButton()
	{
		closeButtonFader.ShouldHideWhenDone = false;
		closeButtonFader.Activate();
	}

	private void Update()
	{
		if (isPoppingCountdownStarted && Time.time >= popTime)
		{
			ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
			{
				x.Pop();
			});
		}
		if (currentContentBeingShowed < popupContentList.Count)
		{
			if (currentContentBeingShowed > 0)
			{
				float t = (Time.time - interpolateColorStartTime) / colorInterpolationDuration;
				Background.color = Color.Lerp(popupContentList[currentContentBeingShowed - 1].BackgroundColor, popupContentList[currentContentBeingShowed].BackgroundColor, t);
			}
			float newScale = bounceEffect.Evaluate((Time.time - bounceEffectStartTime) / bounceEffectDuration);
			popupContentList[currentContentBeingShowed].UpdateScale(newScale);
			float newAlpha = fadeEffect.Evaluate((Time.time - fadeEffectStartTime) / fadeDuration);
			popupContentList[currentContentBeingShowed].UpdateAlpha(newAlpha);
		}
	}

	private void OnStartingToDissappear()
	{
		currentContentBeingShowed++;
		if (currentContentBeingShowed < popupContentList.Count)
		{
			StartNewPopupContent(currentContentBeingShowed);
			return;
		}
		isPoppingCountdownStarted = true;
		popTime = Time.time + fadeDuration;
	}

	private void StartNewPopupContent(int index)
	{
		popupContentList[index].gameObject.SetActive(value: true);
		popupContentList[index].Initialize(unlockedTier, OnStartingToDissappear);
		interpolateColorStartTime = Time.time;
		bounceEffectStartTime = Time.time;
		fadeEffectStartTime = Time.time + (popupContentList[index].DisplayTime - fadeDuration);
		float newScale = bounceEffect.Evaluate((Time.time - bounceEffectStartTime) / bounceEffectDuration);
		popupContentList[currentContentBeingShowed].UpdateScale(newScale);
		float newAlpha = fadeEffect.Evaluate((Time.time - fadeEffectStartTime) / fadeDuration);
		popupContentList[currentContentBeingShowed].UpdateAlpha(newAlpha);
	}

	private bool ShouldShowLootPopup()
	{
		Dictionary<MVWorldObjectDocumentationType, List<MVWorldObjectClient>> tierItemData = MVGameControllerBase.Game.GameTierShopRepository.GetTierItemData(unlockedTier);
		if (tierItemData == null)
		{
			return false;
		}
		Dictionary<MVWorldObjectDocumentationType, List<MVWorldObjectClient>> dictionary = new Dictionary<MVWorldObjectDocumentationType, List<MVWorldObjectClient>>();
		foreach (KeyValuePair<MVWorldObjectDocumentationType, List<MVWorldObjectClient>> item in tierItemData)
		{
			if (item.Value.Count > 0 && IsTierItemALootItem(item.Value[0]))
			{
				dictionary.Add(item.Key, item.Value);
			}
		}
		return dictionary.Count > 0;
	}

	private bool IsTierItemALootItem(MVWorldObjectClient item)
	{
		return item is MVPickupItemBase || item is MVWorldObjectSpawnerVehicle;
	}

	private bool ShouldShowAccessPopup()
	{
		Dictionary<MVWorldObjectDocumentationType, List<MVWorldObjectClient>> tierItemData = MVGameControllerBase.Game.GameTierShopRepository.GetTierItemData(unlockedTier);
		if (tierItemData == null)
		{
			return false;
		}
		Dictionary<MVWorldObjectDocumentationType, List<MVWorldObjectClient>> dictionary = new Dictionary<MVWorldObjectDocumentationType, List<MVWorldObjectClient>>();
		foreach (KeyValuePair<MVWorldObjectDocumentationType, List<MVWorldObjectClient>> item in tierItemData)
		{
			if (item.Value.Count > 0 && IsTierItemAnAccessItem(item.Key, item.Value[0]))
			{
				dictionary.Add(item.Key, item.Value);
			}
		}
		return dictionary.Count > 0;
	}

	private bool IsTierItemAnAccessItem(MVWorldObjectDocumentationType worldObjectType, MVWorldObjectClient item)
	{
		return worldObjectType == MVWorldObjectDocumentationType.Lever || worldObjectType == MVWorldObjectDocumentationType.PressurePlate || item is MVTeleporter;
	}
}
